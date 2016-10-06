using System;
using System.Threading;
using System.Threading.Tasks;
using Plugin.TextToSpeech;
using Xamarin.Forms;

namespace Samples
{
    public class App : Application
    {
        CancellationTokenSource cancelSrc;
        const string MSG = "This is a test of the emergency broadcast system.  Had this had been an actual emergency, I'm sure something else important would have happened";


        public App()
        {
            // The root page of your application
            var btn = new Button {Text = "Say Hello" };
            var queueBtn = new Button { Text = "Test Queue" };
            var lbl = new Label { TextColor = Color.Red };

            queueBtn.Command = new Command(async () =>
            {
                queueBtn.IsEnabled = false;
                try
                {

                    await Task.WhenAll(
                        CrossTextToSpeech.Current.Speak("Queue 1"),
                        CrossTextToSpeech.Current.Speak("Queue 2"),
                        CrossTextToSpeech.Current.Speak("Queue 3"),
                        CrossTextToSpeech.Current.Speak("Queue 4"),
                        CrossTextToSpeech.Current.Speak("Queue 5")
                    );
                }
                catch (Exception ex)
                {
                    lbl.Text = ex.ToString();
                }
                finally
                {
                    queueBtn.IsEnabled = true;
                }
            });
            btn.Command = new Command(async () =>
            {
                lbl.Text = String.Empty;
                btn.Text = "Cancel Speech";
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
                catch (OperationCanceledException) {}
                catch (Exception ex)
                {
                    lbl.Text = ex.ToString();
                }
                finally
                {
                    btn.Text = "Say Hello";
                    cancelSrc = null;
                }
            });



            var content = new ContentPage
            {
                Title = "Samples",
                Content = new StackLayout
                {
                    VerticalOptions = LayoutOptions.Center,
                    Children = {
                        btn,
                        queueBtn,
                        lbl
                    }
                }
            };

            MainPage = new NavigationPage(content);
        }
    }
}
