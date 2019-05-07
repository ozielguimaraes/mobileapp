using System;
using Android.App;
using Android.Runtime;
using MvvmCross.Droid.Support.V7.AppCompat;
using Toggl.Core.UI;
using Toggl.Core.UI.ViewModels;

namespace Toggl.Droid
{
    [Application(AllowBackup = false)]
    public class TogglApplication : MvxAppCompatApplication<Setup, App<LoginViewModel>>
    {
        public TogglApplication(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {
        }

        public override void OnCreate()
        {
            base.OnCreate();
            Firebase.FirebaseApp.InitializeApp(this);
#if USE_APPCENTER
            Microsoft.AppCenter.AppCenter.Start(
                "{TOGGL_APP_CENTER_ID_DROID}",
                typeof(Microsoft.AppCenter.Crashes.Crashes),
                typeof(Microsoft.AppCenter.Analytics.Analytics));
#endif
        }
    }
}
