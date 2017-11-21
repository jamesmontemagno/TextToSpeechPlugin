using Plugin.TextToSpeech.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Tizen.Uix.Tts;

namespace Plugin.TextToSpeech
{
	/// <summary>
	/// Text to speech implemenation Tizen
	/// </summary>
	public class TextToSpeech : ITextToSpeech, IDisposable
	{
		readonly SemaphoreSlim semaphore = new SemaphoreSlim(1, 1);
		static TtsClient ttsInst;
		static List<SupportedVoice> list = null;
		uint maxLength = 0;

		public TextToSpeech()
		{
			ttsInst = new TtsClient();
			ttsInst.Prepare();
			ttsInst.StateChanged += TtsStateChanged;
			try
			{
				list = (List<SupportedVoice>)ttsInst.GetSupportedVoices();
			}
			catch (Exception ex)
			{
				Console.Write(ex.Message);
			}
		}

		void TtsStateChanged(object sender, StateChangedEventArgs e)
		{
			if (e.Current == State.Ready)
			{
				maxLength = ttsInst.MaxTextSize;
			}
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
		public async Task Speak(string text, CrossLocale? crossLocale = null, float? pitch = null, float? speakRate = null, float? volume = null, CancellationToken cancelToken = default(CancellationToken))
		{
			if (text == null)
				throw new ArgumentNullException(nameof(text), "Text can not be null");

			try
			{
				await semaphore.WaitAsync(cancelToken);
				ttsInst.AddText(text, crossLocale.Value.Language, 0, 0);
				ttsInst.Play();
			}
			catch (Exception ex)
			{
				Console.Write(ex.Message);
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
		public Task<IEnumerable<CrossLocale>> GetInstalledLanguages()
		{
			return Task.FromResult(list.Select(a => new CrossLocale { Language = a.Language }));
		}

		/// <summary>
		/// Gets the max string length of the speech engine
		/// -1 means no limit
		/// </summary>
		public int MaxSpeechInputLength =>  (int)maxLength;

		/// <summary>
		/// Dispose of TTS
		/// </summary>
		public void Dispose()
		{
			if (ttsInst != null)
			{
				ttsInst.Stop();
				ttsInst.Unprepare();
				ttsInst.StateChanged -= TtsStateChanged;
				ttsInst = null;
			}
		}
	}
}
