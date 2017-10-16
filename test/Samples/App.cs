using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Plugin.TextToSpeech;
using Plugin.TextToSpeech.Abstractions;

using Xamarin.Forms;

namespace Samples
{
    public class App : Application
    {
        CancellationTokenSource cancelSrc;
        const string MSG = "This is a test of the emergency broadcast system.  Had this had been an actual emergency, I'm sure something else important would have happened";

		CrossLocale? lang = null;

        public App()
        {
            // The root page of your application
            var btn = new Button {Text = "Say Hello" };
            var queueBtn = new Button { Text = "Test Queue" };
            var lbl = new Label { TextColor = Color.Red };

            queueBtn.Command = new Command(async () =>
            {
	            lbl.Text = "";
	            try
	            {
		            if (cancelSrc == null)
		            {
			            queueBtn.Text = "Cancel Test Queue";
			            cancelSrc = new CancellationTokenSource();
			            await Task.WhenAll(
				            CrossTextToSpeech.Current.Speak("Queue 1", lang, cancelToken: cancelSrc.Token),
				            CrossTextToSpeech.Current.Speak("Queue 2", lang, cancelToken: cancelSrc.Token),
				            CrossTextToSpeech.Current.Speak("Queue 3", lang, cancelToken: cancelSrc.Token),
				            CrossTextToSpeech.Current.Speak("Queue 4", lang, cancelToken: cancelSrc.Token),
				            CrossTextToSpeech.Current.Speak("Queue 5", lang, cancelToken: cancelSrc.Token)
			            );
		            }
		            else
		            {
			            cancelSrc.Cancel();
		            }
	            }
	            catch (OperationCanceledException)
	            {
		            lbl.Text = "You cancelled it";
	            }
	            catch (Exception ex)
	            {
		            lbl.Text = ex.ToString();
	            }
	            finally
	            {
		            queueBtn.Text = "Test Queue";
		            cancelSrc = null;
	            }
            });
            btn.Command = new Command(async () =>
            {
                lbl.Text = string.Empty;
                btn.Text = "Cancel Speech";
                try
                {
                    if (cancelSrc == null)
                    {
                        cancelSrc = new CancellationTokenSource();
                        await CrossTextToSpeech.Current.Speak(MSG, lang, cancelToken: cancelSrc.Token);
                        cancelSrc = null;
                    }
                    else
                    {
                        cancelSrc.Cancel();
                    }
                }
                catch (OperationCanceledException)
                {
                    lbl.Text = "You cancelled it";
                }
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



			var language = new Label
			{
				Text = "Default language"
			};

			var pickLanguage = new Button
			{
				Text = "Pick Language",
				Command = new Command(async () =>
				{
					var items = await CrossTextToSpeech.Current.GetInstalledLanguages();
					var result = await MainPage.DisplayActionSheet("Pick", "OK", null, items.Select(i => i.DisplayName).ToArray());
					lang = items.FirstOrDefault(i => i.DisplayName == result);
					language.Text = (result == "OK" || string.IsNullOrEmpty(result)) ? "Default" : result;
				})
			};


            var content = new ContentPage
            {
                Title = "Samples",
                Content = new StackLayout
                {
                    VerticalOptions = LayoutOptions.Center,
                    Children = {
                        btn,
                        queueBtn,
                        lbl,
						language,
						pickLanguage
                    }
                }
            };

            MainPage = new NavigationPage(content);
        }
    }
}
