using Plugin.TextToSpeech.Abstractions;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Media.SpeechSynthesis;
using System.Diagnostics;
using Windows.Media.Playback;
using System.Collections.Generic;

namespace Plugin.TextToSpeech
{
    /// <summary>
    /// Text To Speech Impelemenatation Windows
    /// </summary>
    public class TextToSpeech : ITextToSpeech, IDisposable
    {
        readonly SemaphoreSlim semaphore = new SemaphoreSlim(1, 1);
        SpeechSynthesizer speechSynthesizer;


        /// <summary>
        /// SpeechSynthesizer
        /// </summary>
        public TextToSpeech() => speechSynthesizer = new SpeechSynthesizer();


        /// <summary>
        /// Speak back text
        /// </summary>
        /// <param name="text">Text to speak</param>
        /// <param name="crossLocale">Locale of voice</param>
        /// <param name="pitch">Pitch of voice</param>
        /// <param name="speakRate">Speak Rate of voice (All) (0.0 - 2.0f)</param>
        /// <param name="volume">Volume of voice (0.0-1.0)</param>
        /// <param name="cancelToken">Canelation token to stop speak</param>
        /// <exception cref="ArgumentNullException">Thrown if text is null</exception>
        /// <exception cref="ArgumentException">Thrown if text length is greater than maximum allowed</exception>
        public async Task Speak(string text, CrossLocale? crossLocale = null, float? pitch = null, float? speakRate = null, float? volume = null, CancellationToken cancelToken = default(CancellationToken))
        {
            if (text == null)
                throw new ArgumentNullException(nameof(text), "Text can not be null");

            try
            {
                await semaphore.WaitAsync(cancelToken);
                var localCode = string.Empty;

                //nothing fancy needed here
                if (pitch == null && speakRate == null && volume == null)
                {
                    if (crossLocale.HasValue && !string.IsNullOrWhiteSpace(crossLocale.Value.Language))
                    {
                        localCode = crossLocale.Value.Language;

                        var voices = from voice in SpeechSynthesizer.AllVoices
                                     where (voice.Language == localCode
                                            && voice.Gender.Equals(VoiceGender.Female))
                                     select voice;
                        speechSynthesizer.Voice = (voices.Any() ? voices.ElementAt(0) : SpeechSynthesizer.DefaultVoice);


                    }
                    else
                    {
                        speechSynthesizer.Voice = SpeechSynthesizer.DefaultVoice;

                    }
                }


                if (crossLocale.HasValue && !string.IsNullOrWhiteSpace(crossLocale.Value.Language))
                {
                    localCode = crossLocale.Value.Language;
                    var voices = from voice in SpeechSynthesizer.AllVoices
                                 where (voice.Language == localCode
                                        && voice.Gender.Equals(VoiceGender.Female))
                                 select voice;

                    if (!voices.Any())
                    {
                        localCode = SpeechSynthesizer.DefaultVoice.Language;

                    }
                }
                else
                {
                    localCode = SpeechSynthesizer.DefaultVoice.Language;

                }


                if (!volume.HasValue)
                    volume = 100.0f;
                else if (volume.Value > 1.0f)
                    volume = 100.0f;
                else if (volume.Value < 0.0f)
                    volume = 0.0f;
                else
                    volume = volume.Value * 100.0f;

                var pitchProsody = "default";
                //var test = "x-low", "low", "medium", "high", "x-high", or "default";
                if (!pitch.HasValue)
                    pitchProsody = "default";
                else if (pitch.Value >= 1.6f)
                    pitchProsody = "x-high";
                else if (pitch.Value >= 1.1f)
                    pitchProsody = "high";
                else if (pitch.Value >= .9f)
                    pitchProsody = "medium";
                else if (pitch.Value >= .4f)
                    pitchProsody = "low";
                else
                    pitchProsody = "x-low";


                string ssmlText = "<speak version=\"1.0\" ";
                ssmlText += "xmlns=\"http://www.w3.org/2001/10/synthesis\" xml:lang=\"" + localCode + "\">";
                ssmlText += "<prosody pitch=\"" + pitchProsody + "\" volume=\"" + volume.Value + "\" rate=\"" +
                            speakRate ?? 1F + "\" >" + text + "</prosody>";
                ssmlText += "</speak>";

                var tcs = new TaskCompletionSource<object>();
                var handler = new TypedEventHandler<MediaPlayer, object>((sender, args) => tcs.TrySetResult(null));

                try
                {
                    var player = BackgroundMediaPlayer.Current;
                    var stream = await speechSynthesizer.SynthesizeSsmlToStreamAsync(ssmlText);

                    player.MediaEnded += handler;
                    player.SetStreamSource(stream);
                    player.Play();

	                void OnCancel()
	                {
						player.PlaybackRate = 0;
		                tcs.TrySetResult(null);
					}

					using (cancelToken.Register(OnCancel))
					{
						await tcs.Task;
					}

                    player.MediaEnded -= handler;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("Unable to playback stream: " + ex);
                }

            }
            finally
            {
	            if (semaphore.CurrentCount == 0)
                    semaphore.Release();
            }

        }

        /// <summary>
        /// Get all installed and valid languages
        /// </summary>
        /// <returns></returns>
        public Task<IEnumerable<CrossLocale>> GetInstalledLanguages() =>
			Task.FromResult(SpeechSynthesizer.AllVoices
              .OrderBy(a => a.Language)
              .Select(a => new CrossLocale { Language = a.Language, DisplayName = a.DisplayName })
              .GroupBy(c => c.ToString())
              .Select(g => g.First()));


        /// <summary>
        /// Gets the max string length of the speech engine
        /// -1 means no limit
        /// </summary>
        public int MaxSpeechInputLength => -1;

        /// <summary>
        /// Dispose of TTS
        /// </summary>
        public void Dispose() =>
            speechSynthesizer?.Dispose();
    }
}
