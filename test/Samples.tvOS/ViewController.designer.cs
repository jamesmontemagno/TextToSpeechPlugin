// WARNING
//
// This file has been generated automatically by Xamarin Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;
using UIKit;

namespace Samples.tvOS
{
    [Register ("ViewController")]
    partial class ViewController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton btnSayHello { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton btnTestQueue { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel Label { get; set; }

        [Action ("btnTestQueue_Pressed:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void btnTestQueue_Pressed (UIKit.UIButton sender);

        void ReleaseDesignerOutlets ()
        {
            if (btnSayHello != null) {
                btnSayHello.Dispose ();
                btnSayHello = null;
            }

            if (btnTestQueue != null) {
                btnTestQueue.Dispose ();
                btnTestQueue = null;
            }

            if (Label != null) {
                Label.Dispose ();
                Label = null;
            }
        }
    }
}