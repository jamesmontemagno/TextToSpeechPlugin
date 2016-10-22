using System;
using AppKit;


namespace Plugin.TextToSpeech
{
    public class TtsSpeechSynthesizerDelegate : NSSpeechSynthesizerDelegate
    {
        public event EventHandler FinishedSpeaking;


        public override void DidFinishSpeaking(NSSpeechSynthesizer sender, bool finishedSpeaking)
        {
            if (finishedSpeaking)
                this.FinishedSpeaking?.Invoke(this, EventArgs.Empty);
        }
    }
}
