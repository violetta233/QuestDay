using System;

namespace QuestDay.Models
{
    public class HouseState
    {
        public int DirtyLevel { get; set; }
        public string CurrentBackgroundImage { get; set; } = "background_normal.png";

        /// <summary>
        /// Возвращает true, если кролик должен быть грязным (чистота ≤ 29%)
        /// </summary>
        public bool IsRabbitDirty => (100 - DirtyLevel) <= 29;

        /// <summary>
        /// Фон для главного экрана (MainPage)
        /// </summary>
        public string GetMainPageBackground()
        {
            int cleanliness = 100 - DirtyLevel;

            if (cleanliness >= 70)
                return "background_normal.png";
            else if (cleanliness >= 30)
                return "background_bad.png";
            else
                return "background_very_bad.png";
        }

        /// <summary>
        /// Фон для страницы гардероба (userPage)
        /// </summary>
        public string GetUserPageBackground()
        {
            int cleanliness = 100 - DirtyLevel;

            if (cleanliness >= 70)
                return "background2.png";
            else if (cleanliness >= 30)
                return "background2_1.png";
            else
                return "background2_2.png";
        }

        // Для обратной совместимости
        public string GetBackgroundByDirtyLevel() => GetMainPageBackground();
    }
}