using System;
using System.Threading;
using System.Threading.Tasks;
using Foundation;
using Plugin.TextToSpeech;
using Plugin.TextToSpeech.Abstractions;
using UIKit;


namespace Samples.tvOS
{
	public partial class ViewController : UIViewController
	{
        CancellationTokenSource cancelSrc;
        const string MSG = "This is a test of the emergency broadcast system.  Had this had been an actual emergency, I'm sure something else important would have happened";

        CrossLocale? lang = null;


        public ViewController(IntPtr handle) : base(handle)
        {
        }


        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            btnChangeLanguage.Enabled = true;
            btnSayHello.Enabled = true;
            btnQueue.Enabled = true;
            lblAction.Text = "";
            lblAction.TextColor = UIColor.Red;
            lblLanguage.Text = "Default Language";

            //btnQueue.TouchUpInside += (sender, e) => DoQueue();
            btnQueue.AllEvents += (sender, e) => DoQueue();
            //btnSayHello.TouchUpInside += (sender, e) => SayHello();
            btnSayHello.AllEvents += (sender, e) => SayHello();
            //btnChangeLanguage.TouchUpInside += (sender, e) => LanguageChange();
            btnChangeLanguage.AllEvents += (sender, e) => LanguageChange();
        }


        async void LanguageChange() 
        {
            var items = await CrossTextToSpeech.Current.GetInstalledLanguages();
            //        var result = await MainPage.DisplayActionSheet("Pick", "OK", null, items.Select(i => i.DisplayName).ToArray());
            //        lang = items.FirstOrDefault(i => i.DisplayName == result);
            //        language.Text = (result == "OK" || string.IsNullOrEmpty(result)) ? "Default" : result;              
        }


        async void DoQueue() 
        {
            btnQueue.Enabled = false;
            try
            {

                await Task.WhenAll(
                    CrossTextToSpeech.Current.Speak("Queue 1", lang),
                    CrossTextToSpeech.Current.Speak("Queue 2", lang),
                    CrossTextToSpeech.Current.Speak("Queue 3", lang),
                    CrossTextToSpeech.Current.Speak("Queue 4", lang),
                    CrossTextToSpeech.Current.Speak("Queue 5", lang)
                );
            }
            catch (Exception ex)
            {
                lblAction.Text = ex.ToString();
            }
            finally
            {
                btnQueue.Enabled = true;
            }            
        }

        async void SayHello() {
            lblAction.Text = "";
            btnSayHello.SetTitle("Cancel Speech", UIControlState.Normal);
            try
            {
                if (cancelSrc == null)
                {
                    cancelSrc = new CancellationTokenSource();
                    await CrossTextToSpeech.Current.Speak(MSG, crossLocale: lang, cancelToken: cancelSrc.Token);
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
                lblAction.Text = "You cancelled it";
            }
            catch (Exception ex)
            {
                lblAction.Text = ex.ToString();
            }
            finally
            {
                btnSayHello.SetTitle("Say Hello", UIControlState.Normal);
                cancelSrc = null;
            }            
        }
    }
}