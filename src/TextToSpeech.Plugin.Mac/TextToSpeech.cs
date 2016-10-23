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


        public async Task Speak(string text, CrossLocale? crossLocale, float? pitch, float? speakRate, float? volume, CancellationToken? cancelToken)
        {
            if (String.IsNullOrWhiteSpace(text))
                throw new ArgumentException("Text is empty");
              
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


        public IEnumerable<CrossLocale> GetInstalledLanguages()
        {
            return NSSpeechSynthesizer
                .AvailableVoices
                .OrderBy(x => x)
                .Select(x => new CrossLocale { Language = x, DisplayName = x });
        }
                /// <summary>
        /// Gets the max string length of the speech engine
        /// -1 means no limit
        /// </summary>
        public int MaxSpeechInputLength => -1;


        public void Dispose()
        {
            speechSynthesizer.Dispose();
            semaphore.Dispose();
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
