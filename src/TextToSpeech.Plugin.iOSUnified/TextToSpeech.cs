#if __UNIFIED__
using AVFoundation;
using UIKit;
#else
using MonoTouch.AVFoundation;
using MonoTouch.UIKit;
#endif
using Plugin.TextToSpeech.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Plugin.TextToSpeech
{
    /// <summary>
    /// Text to speech implemenation iOS
    /// </summary>
    public class TextToSpeech : ITextToSpeech, IDisposable
    {
        readonly AVSpeechSynthesizer speechSynthesizer;
        readonly SemaphoreSlim semaphore;


        /// <summary>
        /// Default contstructor. Creates new AVSpeechSynthesizer
        /// </summary>
        public TextToSpeech()
        {
            speechSynthesizer = new AVSpeechSynthesizer();
            semaphore = new SemaphoreSlim(1, 1);
        }


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
        public async Task Speak(string text, CrossLocale? crossLocale = null, float? pitch = null, float? speakRate = null, float? volume = null, CancellationToken? cancelToken = null)
        {
            if (text == null)
                throw new ArgumentNullException(nameof(text), "Text can not be null");


            try 
            {                
                await semaphore.WaitAsync(cancelToken ?? CancellationToken.None);
                var speechUtterance = GetSpeechUtterance(text, crossLocale, pitch, speakRate, volume);
                await SpeakUtterance(speechUtterance, cancelToken);
            }
            finally 
            {
                semaphore.Release();
            }
        }

        /// <summary>
        /// Get all installed and valid languages
        /// </summary>
        /// <returns></returns>
        public IEnumerable<CrossLocale> GetInstalledLanguages()
        {
            return AVSpeechSynthesisVoice.GetSpeechVoices()
              .OrderBy(a => a.Language)
              .Select(a => new CrossLocale { Language = a.Language, DisplayName = a.Language });
        }

        private AVSpeechUtterance GetSpeechUtterance(string text, CrossLocale? crossLocale, float? pitch, float? speakRate, float? volume)
        {
            AVSpeechUtterance speechUtterance;

            var voice = GetVoiceForLocaleLanguage(crossLocale);

            if (UIDevice.CurrentDevice.CheckSystemVersion(8, 0))
            {
                //speechUtterance = new AVSpeechUtterance(" ");
                speechUtterance = new AVSpeechUtterance(text);
                speechUtterance.Voice = voice;
            }
            else
            {
                speakRate = NormalizeSpeakRate(speakRate);
                volume = NormalizeVolume(volume);
                pitch = NormalizePitch(pitch);

                speechUtterance = new AVSpeechUtterance(text)
                {
                    Rate = speakRate.Value,
                    Voice = voice,
                    Volume = volume.Value,
                    PitchMultiplier = pitch.Value
                };
            }

            return speechUtterance;
        }

        private AVSpeechSynthesisVoice GetVoiceForLocaleLanguage(CrossLocale? crossLocale)
        {
            var localCode = crossLocale.HasValue &&
                                        !string.IsNullOrWhiteSpace(crossLocale.Value.Language) ?
                                        crossLocale.Value.Language :
                                        AVSpeechSynthesisVoice.CurrentLanguageCode;

            var voice = AVSpeechSynthesisVoice.FromLanguage(localCode);
            if (voice == null)
            {
                Console.WriteLine("Locale not found for voice: " + localCode + " is not valid. Using default.");
                voice = AVSpeechSynthesisVoice.FromLanguage(AVSpeechSynthesisVoice.CurrentLanguageCode);
            }

            return voice;
        }

        private float? NormalizeSpeakRate(float? speakRate)
        {
            var divid = 4.0f;
            if (UIDevice.CurrentDevice.CheckSystemVersion(9, 0)) //use default .5f
                divid = 2.0f;
            else if (UIDevice.CurrentDevice.CheckSystemVersion(8, 0)) //use .125f
                divid = 8.0f;
            else
                divid = 4.0f; //use .25f

            if (!speakRate.HasValue)
                speakRate = AVSpeechUtterance.MaximumSpeechRate / divid; //normal speech, default is fast
            else if (speakRate.Value > AVSpeechUtterance.MaximumSpeechRate)
                speakRate = AVSpeechUtterance.MaximumSpeechRate;
            else if (speakRate.Value < AVSpeechUtterance.MinimumSpeechRate)
                speakRate = AVSpeechUtterance.MinimumSpeechRate;

            return speakRate;
        }

        private static float? NormalizeVolume(float? volume)
        {
            if (!volume.HasValue)
                volume = 1.0f;
            else if (volume > 1.0f)
                volume = 1.0f;
            else if (volume < 0.0f)
                volume = 0.0f;

            return volume;
        }

        private static float? NormalizePitch(float? pitch)
        {
            return pitch.GetValueOrDefault(1.0f);
        }


        TaskCompletionSource<object> currentSpeak;
        async Task SpeakUtterance(AVSpeechUtterance speechUtterance, CancellationToken? cancelToken)
        {
            try 
            {
                currentSpeak = new TaskCompletionSource<object>();
                cancelToken?.Register(() => TryCancel());

                speechSynthesizer.DidFinishSpeechUtterance += this.OnFinishedSpeechUtterance;
                speechSynthesizer.SpeakUtterance(speechUtterance);

                await currentSpeak.Task;
            }
            finally 
            {
                speechSynthesizer.DidFinishSpeechUtterance -= this.OnFinishedSpeechUtterance;
            }
        }


        void OnFinishedSpeechUtterance(object sender, AVSpeechSynthesizerUteranceEventArgs args) 
        {
            currentSpeak?.TrySetResult(null);
        }


        void TryCancel()
        {
            speechSynthesizer.StopSpeaking(AVSpeechBoundary.Word);
            currentSpeak?.TrySetCanceled();
        }

        /// <summary>
        /// Gets the max string length of the speech engine
        /// -1 means no limit
        /// </summary>
        public int MaxSpeechInputLength => -1;

        /// <summary>
        /// Dispose of TTS
        /// </summary>
        public void Dispose()
        {
            speechSynthesizer?.Dispose();
        }
    }
}
