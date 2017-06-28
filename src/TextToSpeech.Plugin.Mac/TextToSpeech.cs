using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AppKit;
using Plugin.TextToSpeech.Abstractions;


namespace Plugin.TextToSpeech
{
    /// <summary>
    /// Text to speech
    /// </summary>
    public class TextToSpeech : ITextToSpeech, IDisposable
    {
        readonly NSSpeechSynthesizer speechSynthesizer;
        readonly TtsSpeechSynthesizerDelegate sdelegate;
        readonly SemaphoreSlim semaphore;

        /// <summary>
        /// Constructor for text to speech
        /// </summary>
        public TextToSpeech()
        {
            sdelegate = new TtsSpeechSynthesizerDelegate();
            speechSynthesizer = new NSSpeechSynthesizer { Delegate = sdelegate };
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
        public async Task Speak(string text, CrossLocale? crossLocale, float? pitch, float? speakRate, float? volume, CancellationToken? cancelToken)
        {
            if (text == null)
                throw new ArgumentNullException(nameof(text), "Text can not be null");

            var tcs = new TaskCompletionSource<object>();
            var handler = new EventHandler((sender, args) => tcs.TrySetResult(null));

            try
            {
                var ct = cancelToken ?? CancellationToken.None;
                await semaphore.WaitAsync(ct);


                using (ct.Register(() => 
                {
                    speechSynthesizer.StopSpeaking();
                    tcs.TrySetCanceled();
                })) 
                {
                    speechSynthesizer.Volume = NormalizeVolume(volume);

                    if (speakRate != null)
                        speechSynthesizer.Rate = speakRate.Value;
                    
                    if (crossLocale != null)
                        speechSynthesizer.Voice = crossLocale.Value.Language;


                    sdelegate.FinishedSpeaking += handler;
                    speechSynthesizer.StartSpeakingString(text);
                    await tcs.Task;
                }
            }
            finally 
            {
                semaphore.Release();
                sdelegate.FinishedSpeaking -= handler;
            }
        }

        /// <summary>
        /// Get a list of all installed languages
        /// </summary>
        /// <returns></returns>
        public Task<IEnumerable<CrossLocale>> GetInstalledLanguages() => 
			Task.FromResult(NSSpeechSynthesizer
                .AvailableVoices
                .OrderBy(x => x)
                .Select(x => new CrossLocale { Language = x, DisplayName = x }));
        
                /// <summary>
        /// Gets the max string length of the speech engine
        /// -1 means no limit
        /// </summary>
        public int MaxSpeechInputLength => -1;

		/// <summary>
		/// Dispose of object
		/// </summary>
        public void Dispose()
        {
            speechSynthesizer?.Dispose();
            semaphore?.Dispose();
        }


        static float NormalizeVolume(float? volume)
        {
            var v = volume ?? 1.0f;
            if (v > 1.0f)
                v = 1.0f;
            else if (v < 0.0f)
                v = 0.0f;

            return v;
        }
    }
}
