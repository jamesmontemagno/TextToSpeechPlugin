// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace TextToSpeechSample.Mac
{
	[Register ("ViewController")]
	partial class ViewController
	{
		[Outlet]
		AppKit.NSButton btnSayHello { get; set; }

		[Outlet]
		AppKit.NSButton btnTestQueue { get; set; }

		[Outlet]
		AppKit.NSTextField Label { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (btnTestQueue != null) {
				btnTestQueue.Dispose ();
				btnTestQueue = null;
			}

			if (btnSayHello != null) {
				btnSayHello.Dispose ();
				btnSayHello = null;
			}

			if (Label != null) {
				Label.Dispose ();
				Label = null;
			}
		}
	}
}
