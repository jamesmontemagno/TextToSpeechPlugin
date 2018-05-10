## Text To Speech Plugin for Xamarin and Windows

Simple and elegant way of performing Text To Speech across Xamarin.iOS, Xamarin.tvOS, Xamarin.macOS Xamarin.Android, Windows, and Xamarin.Forms projects.

Preview: http://screencast.com/t/voW1P48Ka


### Setup
* Available on NuGet: https://www.nuget.org/packages/Xam.Plugins.TextToSpeech/ [![NuGet](https://img.shields.io/nuget/v/Xam.Plugins.TextToSpeech.svg?label=NuGet)](https://www.nuget.org/packages/Xam.Plugins.TextToSpeech/)
* **Install into your PCL project and Client projects**.

Build status: 
* ![](https://jamesmontemagno.visualstudio.com/_apis/public/build/definitions/6b79a378-ddd6-4e31-98ac-a12fcd68644c/23/badge)
* CI NuGet Feed: http://myget.org/F/xamarin-plugins

**Platform Support**

|Platform|Version|
| ------------------- | :------------------: |
|Xamarin.iOS|iOS 7+|
|Xamarin.Android|API 10+|
|Windows 10 UWP|10+|
|Xamarin.Mac|All|
|Xamarin tvOS|All|

### Features
* Speak back text
* Pitch
* Volume
* Speak Rate
* Locale/Language of Speech
* Gather all available languages to speak in

### Usage

**Simple Text**
```csharp
await CrossTextToSpeech.Current.Speak("Text to speak");
```

**Advanced speech API**
```csharp
/// <summary>
/// Speack back text
/// </summary>
/// <param name="text">Text to speak</param>
/// <param name="crossLocale">Locale of voice</param>
/// <param name="pitch">Pitch of voice (All 0.0 - 2.0f)</param>
/// <param name="speakRate">Speak Rate of voice (All) (0.0 - 2.0f)</param>
/// <param name="volume">Volume of voice (0.0-1.0)</param>
/// <param name="cancelToken">Cancel the current speech</param>
public async Task Speak(string text, CrossLocale crossLocale = null, float? pitch = null, float? speakRate = null, float? volume = null, CancellationToken cancelToken = default(CancellationToken))
```  

**CrossLocale**
I developed the CrossLocale struct mostly to support Android, but is nice because I added a Display Name.

You can query a list of current support CrossLocales on the device:

```csharp
/// <summary>
/// Get all installed and valide lanaguages
/// </summary>
/// <returns>List of CrossLocales</returns>
public Task<IEnumerable<CrossLocale>> GetInstalledLanguages()
```

Each local has the Language and Display Name. The Country code is only used in Android. If you pass in null to Speak it will use the default.

#### Implementation

* iOS: AVSpeechSynthesizer
* Android: Android.Speech.Tts.TextToSpeech
* Windows: SpeechSynthesizer + Ssml support for advanced playback


**Windows Phone**
You must add ID_CAP_SPEECH_RECOGNITION permission


### Branches
Main branch is always the current development beta branch. This means that all new work can be done off this branch. For each release of a stable NuGet package a new release branch will be created so any additional hot fixes could be done off of that branch.

#### License
Under MIT, see LICENSE file.
