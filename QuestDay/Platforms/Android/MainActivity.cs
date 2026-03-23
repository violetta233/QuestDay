using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Views;
using Android.Widget;
using Microsoft.Maui.Controls.Platform;
using Microsoft.Maui.Platform;

namespace QuestDay;

[Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
public class MainActivity : MauiAppCompatActivity
{
    protected override void OnCreate(Bundle savedInstanceState)
    {
        base.OnCreate(savedInstanceState);

        Microsoft.Maui.Handlers.EntryHandler.Mapper.AppendToMapping("NoBorder", (handler, view) =>
        {
            if (handler.PlatformView is AndroidX.AppCompat.Widget.AppCompatEditText editText)
            {
                editText.Background = null;
                editText.SetPadding(0, 0, 0, 0);
            }
        });

        Microsoft.Maui.Handlers.EditorHandler.Mapper.AppendToMapping("NoBorder", (handler, view) =>
        {
            if (handler.PlatformView is AndroidX.AppCompat.Widget.AppCompatEditText editText)
            {
                editText.Background = null;
                editText.SetPadding(0, 0, 0, 0);
            }
        });
    }
}
