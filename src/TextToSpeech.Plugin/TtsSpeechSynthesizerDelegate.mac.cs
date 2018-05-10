using System;
using AppKit;


namespace Plugin.TextToSpeech
{
	/// <summary>
	/// TTS delegate to listen to speach ending
	/// </summary>
    public class TtsSpeechSynthesizerDelegate : NSSpeechSynthesizerDelegate
    {
		/// <summary>
		/// Event to be triggered when speaking finishes.
		/// </summary>
        public event EventHandler FinishedSpeaking;


		/// <summary>
		/// Invoke method when finished speaking.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="finishedSpeaking"></param>
        public override void DidFinishSpeaking(NSSpeechSynthesizer sender, bool finishedSpeaking)
        {
            if (finishedSpeaking)
                FinishedSpeaking?.Invoke(this, EventArgs.Empty);
        }
    }
}
