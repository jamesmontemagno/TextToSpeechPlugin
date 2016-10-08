using System;
using System.Threading.Tasks;
using Android.Speech.Tts;


namespace Plugin.TextToSpeech
{
    public class TtsProgressListener : UtteranceProgressListener
    {
        readonly TaskCompletionSource<object> completionSource;

        public TtsProgressListener(TaskCompletionSource<object> tcs)
        {
            this.completionSource = tcs;
        }


        public override void OnDone(string utteranceId)
        {
            this.completionSource.TrySetResult(null);
        }


        public override void OnError(string utteranceId)
        {
            this.completionSource.TrySetException(new ArgumentException("Error with TTS engine on progress listener"));
        }


        public override void OnStart(string utteranceId)
        {
        }
    }
}