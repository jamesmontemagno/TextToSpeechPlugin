using System;
using System.Threading.Tasks;
using Android.Speech.Tts;


namespace Plugin.TextToSpeech
{
	/// <summary>
	/// Tts progress listener.
	/// </summary>
    public class TtsProgressListener : UtteranceProgressListener
    {
        readonly TaskCompletionSource<object> completionSource;

		/// <summary>
		/// Initializes a new instance of the <see cref="T:Plugin.TextToSpeech.TtsProgressListener"/> class.
		/// </summary>
		/// <param name="tcs">Tcs.</param>
        public TtsProgressListener(TaskCompletionSource<object> tcs)
        {
            completionSource = tcs;
        }


		/// <summary>
		/// Ons the done.
		/// </summary>
		/// <param name="utteranceId">Utterance identifier.</param>
        public override void OnDone(string utteranceId)
        {
            completionSource.TrySetResult(null);
        }

		/// <summary>
		/// Ons the error.
		/// </summary>
		/// <param name="utteranceId">Utterance identifier.</param>
        public override void OnError(string utteranceId)
        {
            completionSource.TrySetException(new ArgumentException("Error with TTS engine on progress listener"));
        }

		/// <summary>
		/// Ons the start.
		/// </summary>
		/// <param name="utteranceId">Utterance identifier.</param>
        public override void OnStart(string utteranceId)
        {
        }
    }
}