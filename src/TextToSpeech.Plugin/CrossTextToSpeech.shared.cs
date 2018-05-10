using Plugin.TextToSpeech.Abstractions;
using System;

namespace Plugin.TextToSpeech
{
	/// <summary>
	/// Cross platform TTS implemenations
	/// </summary>
	public class CrossTextToSpeech
	{
		static Lazy<ITextToSpeech> implementation = new Lazy<ITextToSpeech>(() => CreateTextToSpeech(), System.Threading.LazyThreadSafetyMode.PublicationOnly);


		/// <summary>
		/// Gets if the plugin is supported on the current platform.
		/// </summary>
		public static bool IsSupported => implementation.Value == null ? false : true;

		/// <summary>
		/// Current plugin implementation to use
		/// </summary>
		public static ITextToSpeech Current
		{
			get
			{
				var ret = implementation.Value;
				if (ret == null)
				{
					throw NotImplementedInReferenceAssembly();
				}
				return ret;
			}
		}

		static ITextToSpeech CreateTextToSpeech()
		{
#if NETSTANDARD1_0 || NETSTANDARD2_0
			return null;
#else
            return new TextToSpeech();
#endif
		}

		internal static Exception NotImplementedInReferenceAssembly() =>
			new NotImplementedException("This functionality is not implemented in the portable version of this assembly.  You should reference the NuGet package from your main application project in order to reference the platform-specific implementation.");


		/// <summary>
		/// Dispose of TTS, reset lazy load
		/// </summary>
		public static void Dispose()
		{
			if (implementation.Value != null && implementation.IsValueCreated)
			{
				implementation.Value.Dispose();
				implementation = new Lazy<ITextToSpeech>(() => CreateTextToSpeech(), System.Threading.LazyThreadSafetyMode.PublicationOnly);
			}
		}
	}
}
