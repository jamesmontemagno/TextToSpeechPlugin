// WARNING
//
// This file has been generated automatically by Visual Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace Samples.tvOS
{
    [Register ("ViewController")]
    partial class ViewController
    {
        [Outlet]
        UIKit.UIButton btnChangeLanguage { get; set; }

        [Outlet]
        UIKit.UIButton btnQueue { get; set; }

        [Outlet]
        UIKit.UIButton btnSayHello { get; set; }

        [Outlet]
        UIKit.UILabel lblAction { get; set; }

        [Outlet]
        UIKit.UILabel lblLanguage { get; set; }

        [Action ("OnChangeLanguagePressed:")]
        partial void OnChangeLanguagePressed (Foundation.NSObject sender);

        [Action ("OnQueuePressed:")]
        partial void OnQueuePressed (Foundation.NSObject sender);

        [Action ("OnSayHelloPressed:")]
        partial void OnSayHelloPressed (Foundation.NSObject sender);
        
        void ReleaseDesignerOutlets ()
        {
            if (btnChangeLanguage != null) {
                btnChangeLanguage.Dispose ();
                btnChangeLanguage = null;
            }

            if (btnQueue != null) {
                btnQueue.Dispose ();
                btnQueue = null;
            }

            if (btnSayHello != null) {
                btnSayHello.Dispose ();
                btnSayHello = null;
            }

            if (lblLanguage != null) {
                lblLanguage.Dispose ();
                lblLanguage = null;
            }

            if (lblAction != null) {
                lblAction.Dispose ();
                lblAction = null;
            }
        }
    }
}
