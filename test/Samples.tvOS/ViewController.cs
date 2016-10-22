using System;
using System.Threading;
using System.Threading.Tasks;
using Plugin.TextToSpeech;
using UIKit;


namespace Samples.tvOS
{
    public partial class ViewController : UIViewController
    {
        CancellationTokenSource cancelSrc;
        const string MSG = "This is a test of the emergency broadcast system.  Had this had been an actual emergency, I'm sure something else important would have happened";


        public ViewController(IntPtr handle) : base(handle)
        {
        }


        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            Label.TextColor = UIColor.Red;
            Label.Text = String.Empty;
            btnSayHello.Enabled = true;
            btnTestQueue.Enabled = true;

            btnSayHello.AllEvents += async (sender, args) =>
            {
                Label.Text = String.Empty;
                btnSayHello.TitleLabel.Text = "Cancel Speech";
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
                    btnSayHello.TitleLabel.Text = "You cancelled it";
                }
                catch (Exception ex)
                {
                    Label.Text = ex.ToString();
                }
                finally
                {
                    btnSayHello.TitleLabel.Text = "Say Hello";
                    cancelSrc = null;
                }
            };
        }


        partial void btnTestQueue_Pressed(UIButton sender)
        {
            CallTestQueue();
        }


        async Task CallTestQueue() 
        {
            Label.Text = String.Empty;
            btnTestQueue.Enabled = false;
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
                Label.Text = ex.ToString();
            }
            finally
            {
                btnTestQueue.Enabled = true;
            }            
        }
    }
}

