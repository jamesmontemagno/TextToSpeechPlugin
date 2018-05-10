using System;
using System.Threading;
using System.Threading.Tasks;
using AppKit;
using Plugin.TextToSpeech;

namespace TextToSpeechSample.Mac
{
    public partial class ViewController : NSViewController
    {
        CancellationTokenSource cancelSrc;
        const string MSG = "This is a test of the emergency broadcast system.  Had this had been an actual emergency, I'm sure something else important would have happened";


        public ViewController(IntPtr handle) : base(handle)
        {
        }


        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            Label.TextColor = NSColor.Red;
            Label.StringValue = String.Empty;

            btnTestQueue.Activated += async (sender, args) =>
            {
                Label.StringValue = String.Empty;
                btnTestQueue.State = NSCellStateValue.Off;
                try
                {
                    cancelSrc = new CancellationTokenSource();
                    await Task.WhenAll(
                        CrossTextToSpeech.Current.Speak("Queue 1", cancelToken: cancelSrc.Token),
                        CrossTextToSpeech.Current.Speak("Queue 2", cancelToken: cancelSrc.Token),
                        CrossTextToSpeech.Current.Speak("Queue 3", cancelToken: cancelSrc.Token),
                        CrossTextToSpeech.Current.Speak("Queue 4", cancelToken: cancelSrc.Token),
                        CrossTextToSpeech.Current.Speak("Queue 5", cancelToken: cancelSrc.Token)
                    );
                }
                catch (Exception ex)
                {
                    Label.StringValue = ex.ToString();
                }
                finally
                {
                    btnTestQueue.State = NSCellStateValue.On;
                }
            };
            btnSayHello.Activated += async (sender, args) =>
            {
                Label.StringValue = String.Empty;
                btnSayHello.Title = "Cancel Speech";
                try
                {
                    if (cancelSrc == null)
                    {
                        cancelSrc = new CancellationTokenSource();
                        await CrossTextToSpeech.Current.Speak(MSG, cancelToken: cancelSrc.Token);
                        cancelSrc = null;
                    }
                    else
                    {
                        cancelSrc.Cancel();
                        cancelSrc = null;
                    }
                }
                catch (OperationCanceledException)
                {
                    btnSayHello.Title = "You cancelled it";
                }
                catch (Exception ex)
                {
                    Label.StringValue = ex.ToString();
                }
                finally
                {
                    btnSayHello.Title = "Say Hello";
                    cancelSrc = null;
                }
            };
        }
    }
}
