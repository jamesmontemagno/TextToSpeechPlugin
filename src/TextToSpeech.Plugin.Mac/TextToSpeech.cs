using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AppKit;
using Plugin.TextToSpeech.Abstractions;


namespace Plugin.TextToSpeech
{
    public class TextToSpeech : ITextToSpeech, IDisposable
    {
        readonly NSSpeechSynthesizer speechSynthesizer;
        readonly SemaphoreSlim semaphore;


        public TextToSpeech()
        {
            speechSynthesizer = new NSSpeechSynthesizer();
            semaphore = new SemaphoreSlim(1, 1);
        }


        public async Task Speak(string text, CrossLocale? crossLocale, float? pitch, float? speakRate, float? volume, CancellationToken? cancelToken)
        {
            if (String.IsNullOrWhiteSpace(text))
                throw new ArgumentException("Text is empty");
                
            speechSynthesizer.StartSpeakingString(text);
        }


        public IEnumerable<CrossLocale> GetInstalledLanguages()
        {
            throw new NotImplementedException();
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
    }
}
