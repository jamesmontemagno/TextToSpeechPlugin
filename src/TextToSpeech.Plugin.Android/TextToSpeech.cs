using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Plugin.TextToSpeech.Abstractions;
using Java.Util;
using Android.Speech.Tts;
using Android.App;

namespace Plugin.TextToSpeech
{
    /// <summary>
    /// Text to speech implementation Android
    /// </summary>
    public class TextToSpeech : Java.Lang.Object, ITextToSpeech, Android.Speech.Tts.TextToSpeech.IOnInitListener, IDisposable
    {
        readonly SemaphoreSlim semaphore = new SemaphoreSlim(1, 1);
        Android.Speech.Tts.TextToSpeech textToSpeech;
        string text;
        CrossLocale? language;
        float pitch, speakRate;
        bool initialized;
		int count;


        TaskCompletionSource<bool> initTcs;
        Task Init()
        {
            if (initialized)
                return Task.FromResult(true);

            initTcs = new TaskCompletionSource<bool>();

            Console.WriteLine("Current version: " + (int)global::Android.OS.Build.VERSION.SdkInt);
            Android.Util.Log.Info("CrossTTS", "Current version: " + (int)global::Android.OS.Build.VERSION.SdkInt);
            textToSpeech = new Android.Speech.Tts.TextToSpeech(Application.Context, this);

            return initTcs.Task;
        }


        #region IOnInitListener implementation
        /// <summary>
        /// OnInit of TTS
        /// </summary>
        /// <param name="status"></param>
        public void OnInit(OperationResult status)
        {
            if (status.Equals(OperationResult.Success))
            {
                initTcs.TrySetResult(true);
                initialized = true;
            }
            else
            {
                initTcs.TrySetException(new ArgumentException("Failed to initialize TTS engine"));
            }
        }
        #endregion

        /// <summary>
        /// Speak back text
        /// </summary>
        /// <param name="text">Text to speak</param>
        /// <param name="crossLocale">Locale of voice</param>
        /// <param name="pitch">Pitch of voice</param>
        /// <param name="speakRate">Speak Rate of voice (All) (0.0 - 2.0f)</param>
        /// <param name="volume">Volume of voice (iOS/WP) (0.0-1.0)</param>
        /// <param name="cancelToken">Canelation token to stop speak</param>
        /// <exception cref="ArgumentNullException">Thrown if text is null</exception>
        /// <exception cref="ArgumentException">Thrown if text length is greater than maximum allowed</exception>
        public async Task Speak(string text, CrossLocale? crossLocale = null, float? pitch = null, float? speakRate = null, float? volume = null, CancellationToken cancelToken = default(CancellationToken))
        {
            if (text == null)
                throw new ArgumentNullException(nameof(text), "Text can not be null");

            if (text.Length > MaxSpeechInputLength)
                throw new ArgumentException(nameof(text), "Text length is over the maximum speech input length.");

            try
            {
                await semaphore.WaitAsync(cancelToken);
                this.text = text;
				language = crossLocale;
                this.pitch = pitch == null ? 1.0f : pitch.Value;
                this.speakRate = speakRate == null ? 1.0f : speakRate.Value;

                // TODO: need to wait lock so not to break people using queuing mechanism
                await Init();
                await Speak(cancelToken);
            }
            finally
            {
                if (!cancelToken.IsCancellationRequested)
                    semaphore.Release();
            }
        }


        private void SetDefaultLanguage() => SetDefaultLanguageNonLollipop();

        
        private void SetDefaultLanguageNonLollipop()
        {
            //disable warning because we are checking ahead of time.
#pragma warning disable 0618
            var sdk = (int)global::Android.OS.Build.VERSION.SdkInt;
            if (sdk >= 18)
            {

                try
                {

#if __ANDROID_18__
                    if (textToSpeech.DefaultLanguage == null && textToSpeech.Language != null)
                        textToSpeech.SetLanguage(textToSpeech.Language);
                    else if (textToSpeech.DefaultLanguage != null)
                        textToSpeech.SetLanguage(textToSpeech.DefaultLanguage);
#endif
                }
                catch
                {

                    if (textToSpeech.Language != null)
                        textToSpeech.SetLanguage(textToSpeech.Language);
                }
            }
            else
            {
                if (textToSpeech.Language != null)
                    textToSpeech.SetLanguage(textToSpeech.Language);
            }
#pragma warning restore 0618
        }


        async Task Speak(CancellationToken cancelToken)
        {
            if (string.IsNullOrWhiteSpace(text))
                return;

            if (language.HasValue && !string.IsNullOrWhiteSpace(language.Value.Language))
            {
                Locale locale = null;
                if (!string.IsNullOrWhiteSpace(language.Value.Country))
                    locale = new Locale(language.Value.Language, language.Value.Country);
                else
                    locale = new Locale(language.Value.Language);

                var result = textToSpeech.IsLanguageAvailable(locale);
                if (result == LanguageAvailableResult.CountryAvailable)
                {
                    textToSpeech.SetLanguage(locale);
                }
                else
                {
                    Console.WriteLine("Locale: " + locale + " was not valid, setting to default.");
                    SetDefaultLanguage();
                }
            }
            else
            {
                SetDefaultLanguage();
            }

            var tcs = new TaskCompletionSource<object>();

            textToSpeech.SetPitch(pitch);
            textToSpeech.SetSpeechRate(speakRate);
            textToSpeech.SetOnUtteranceProgressListener(new TtsProgressListener(tcs));
#pragma warning disable CS0618 // Type or member is obsolete

			count++;
			var map = new Dictionary<string, string>
			{
				[Android.Speech.Tts.TextToSpeech.Engine.KeyParamUtteranceId] = count.ToString()
			};
			textToSpeech.Speak(text, QueueMode.Flush, map);
#pragma warning restore CS0618 // Type or member is obsolete


			void OnCancel()
			{
				textToSpeech.Stop();
				tcs.TrySetCanceled();
			}

			using (cancelToken.Register(OnCancel))
			{
				await tcs.Task;
			}
        }

        /// <summary>
        /// Get all installed and valide lanaguages
        /// </summary>
        /// <returns>List of CrossLocales</returns>
        public async Task<IEnumerable<CrossLocale>> GetInstalledLanguages()
        {
			await Init();
            if (textToSpeech != null && initialized)
            {
                var version = (int)global::Android.OS.Build.VERSION.SdkInt;
                var isLollipop = version >= 21;
                if (isLollipop)
                {
                    try
                    {
                        //in a different method as it can crash on older target/compile for some reason
                        return GetInstalledLanguagesLollipop();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Something went horribly wrong, defaulting to old implementation to get languages: " + ex);
                    }
                }

                var languages = new List<CrossLocale>();
                var allLocales = Locale.GetAvailableLocales();
                foreach (var locale in allLocales)
                {

                    try
                    {
                        var result = textToSpeech.IsLanguageAvailable(locale);

                        if (result == LanguageAvailableResult.CountryAvailable)
                        {
                            languages.Add(new CrossLocale { Country = locale.Country, Language = locale.Language, DisplayName = locale.DisplayName });
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error checking language; " + locale + " " + ex);
                    }
                }

                return languages.GroupBy(c => c.ToString())
                      .Select(g => g.First());
            }
            else
            {
                return Locale.GetAvailableLocales()
                  .Where(a => !string.IsNullOrWhiteSpace(a.Language) && !string.IsNullOrWhiteSpace(a.Country))
                  .Select(a => new CrossLocale { Country = a.Country, Language = a.Language, DisplayName = a.DisplayName })
                  .GroupBy(c => c.ToString())
                  .Select(g => g.First());
            }
        }

        /// <summary>
        /// In a different method as it can crash on older target/compile for some reason
        /// </summary>
        /// <returns></returns>
        private IEnumerable<CrossLocale> GetInstalledLanguagesLollipop()
        {
            var sdk = (int)global::Android.OS.Build.VERSION.SdkInt;
            if (sdk < 21)
                return new List<CrossLocale>();

#if __ANDROID_21__
            return textToSpeech.AvailableLanguages
              .Select(a => new CrossLocale { Country = a.Country, Language = a.Language, DisplayName = a.DisplayName });
#endif
        }

        /// <summary>
        /// Gets the max string length of the speech engine
        /// -1 means no limit
        /// </summary>
        public int MaxSpeechInputLength =>
            Android.Speech.Tts.TextToSpeech.MaxSpeechInputLength;

        void IDisposable.Dispose()
        {
            textToSpeech?.Stop();
            textToSpeech?.Dispose();
            textToSpeech = null;
        }
    }
}
