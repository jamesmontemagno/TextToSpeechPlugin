using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;


namespace Plugin.TextToSpeech.Abstractions
{
	/// <summary>
	/// Interface for TTS
	/// </summary>
	public interface ITextToSpeech : IDisposable
	{
		/// <summary>
		/// Speak back text
		/// </summary>
		/// <param name="text">Text to speak</param>
		/// <param name="crossLocale">Locale of voice</param>
		/// <param name="pitch">Pitch of voice</param>
		/// <param name="speakRate">Speak Rate of voice (All) (0.0 - 2.0f)</param>
		/// <param name="volume">Volume of voice (iOS/WP) (0.0-1.0)</param>
		/// <param name="cancelToken">Canelation token to stop speak</param> 
		Task Speak(string text, CrossLocale? crossLocale = null, float? pitch = null, float? speakRate = null, float? volume = null, CancellationToken? cancelToken = null);

		/// <summary>
		/// Get avalid list of installed languages for TTS
		/// </summary>
		/// <returns></returns>
		IEnumerable<CrossLocale> GetInstalledLanguages();
	}
}

