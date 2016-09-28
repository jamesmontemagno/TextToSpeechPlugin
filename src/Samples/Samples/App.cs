using System;
using System.Threading;
using Plugin.TextToSpeech;
using Xamarin.Forms;

namespace Samples
{
    public class App : Application
    {
        CancellationTokenSource cancelSrc;


        public App()
        {
            // The root page of your application
            var btn = new Button {Text = "Say Hello" };
            var lbl = new Label { TextColor = Color.Red };

            btn.Command = new Command(async () =>
            {
                lbl.Text = String.Empty;
                btn.Text = "Cancel Speech";
                try
                {
                    if (cancelSrc == null)
                    {
                        cancelSrc = new CancellationTokenSource();
                        await CrossTextToSpeech.Current.Speak("Hello. Testing Testing 1 2 3", cancelToken: cancelSrc.Token);
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
                        lbl
                    }
                }
            };

            MainPage = new NavigationPage(content);
        }
    }
}
