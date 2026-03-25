using System.Collections.Generic;

namespace QuestDay.Models
{
    public class HouseState
    {
        public int DirtyLevel { get; set; }
        public string CurrentBackgroundImage { get; set; } = "habit_home_clean.png";

        public static readonly Dictionary<int, string> Backgrounds = new()
        {
            { 0, "background_normal.png" },
            { 1, "background_bad.png" },
            { 2, "background_very_bad.png" }
        };

        public string GetBackgroundByDirtyLevel()
        {
            if (DirtyLevel <= 30) return Backgrounds[0];
            if (DirtyLevel <= 70) return Backgrounds[1];
            return Backgrounds[2];
        }
    }
}