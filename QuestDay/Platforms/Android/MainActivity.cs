using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Views;
using Android.Widget;
using AndroidX.Core.App;
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

        CreateNotificationChannel();
    }

    private void CreateNotificationChannel()
    {
        if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
        {
            var channelId = "questday_channel";
            var channelName = "QuestDay Уведомления";
            var channelDescription = "Уведомления о выполнении привычек";

            var channel = new NotificationChannel(channelId, channelName, NotificationImportance.High);
            channel.Description = channelDescription;

            var notificationManager = (NotificationManager)GetSystemService(NotificationService);
            notificationManager.CreateNotificationChannel(channel);
        }
    }
}
