using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xamarin.Forms;
using Plugin.TextToSpeech;
using Plugin.TextToSpeech.Abstractions;

namespace TextToSpeechSample
{
	public class App : Application
	{
		static CrossLocale? locale = null;
		static ContentPage page;
		public App()
		{
			var speakButton = new Button
			{
				Text = "Speak"
			};

			var languageButton = new Button
			{
				Text = "Default Language"
			};

			languageButton.Clicked += async (sender, args) =>
			{
				var locales = await CrossTextToSpeech.Current.GetInstalledLanguages();
				var items = locales.Select(a => a.ToString()).ToArray();


				var selected = await page.DisplayActionSheet("Language", "OK", null, items);
				if (string.IsNullOrWhiteSpace(selected) || selected == "OK")
					return;
				languageButton.Text = selected;

				if (Device.RuntimePlatform == Device.Android)
					locale = locales.FirstOrDefault(l => l.ToString() == selected);
				else
					locale = new CrossLocale { Language = selected };//fine for iOS/WP

			};

			var sliderPitch = new Slider(0, 2.0, 1.0);
			var sliderRate = new Slider(0, 2.0, Device.OnPlatform(.25, 1.0, 1.0));
			var sliderVolume = new Slider(0, 1.0, 1.0);

			var useDefaults = new Switch
			{
				IsToggled = false
			};

			speakButton.Clicked += async (sender, args) =>
			{
				speakButton.IsEnabled = false;
				var text = "The quick brown fox jumped over the lazy dog.";
				if (useDefaults.IsToggled)
				{
					await CrossTextToSpeech.Current.Speak(text);
					speakButton.IsEnabled = true;
					return;
				}

				await CrossTextToSpeech.Current.Speak(text,
					pitch: (float)sliderPitch.Value,
					speakRate: (float)sliderRate.Value,
					volume: (float)sliderVolume.Value,
					crossLocale: locale);

				speakButton.IsEnabled = true;
			};


			// The root page of your application
			MainPage = page = new ContentPage
			{
				Content = new StackLayout
				{
					Padding = 50,
					VerticalOptions = LayoutOptions.Center,
					Children =
					{
						new Label{ Text = "Pitch"},
						  sliderPitch,
						  new Label{ Text = "Speak Rate"},
						  sliderRate,
						  new Label{ Text = "Volume"},
						  sliderVolume,
						  new Label{ Text = "Use Defaults"},
						  useDefaults,
						  languageButton,
						  speakButton,
					}
				}
			};
		}

		protected override void OnStart()
		{
			// Handle when your app starts
		}

		protected override void OnSleep()
		{
			// Handle when your app sleeps
		}

		protected override void OnResume()
		{
			// Handle when your app resumes
		}
	}
}
