namespace QuestDay.Resources;

public static class AppResources
{
    public static void Initialize()
    {
        Application.Current.Resources = new ResourceDictionary
        {
            { "baseColor", Color.FromHex("#FD6C4C") },
            { "selectedColor", Color.FromHex("#F24822") }
        };
    }
}
