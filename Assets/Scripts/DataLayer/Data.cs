using System;
using UnityEngine;

namespace DataLayer
{
    [Serializable]
    public static class Data
    {
        private const int MAX_PLAYERS = 8;
        public static string TeamName { get; set; }
        public static string InstructorName { get; set; }
        public static string[] PlayerNames { get; set; }
        public static Texture2D TeamPhoto { get; set; } = new(1, 1, TextureFormat.ARGB32, false);
        
        public static void ResetData()
        {
            TeamName = "";
            InstructorName = "";
            PlayerNames = new string[MAX_PLAYERS];
            TeamPhoto = null;
        }
        //save data to player prefs
        public static void SaveData()
        {
            PlayerPrefs.SetString("TeamName", TeamName);
            PlayerPrefs.SetString("InstructorName", InstructorName);
            PlayerPrefs.SetString("PlayerNames", string.Join(",", PlayerNames));
            PlayerPrefs.SetString("TeamPhoto", Convert.ToBase64String(TeamPhoto.EncodeToPNG()));
        }
        //load data from player prefs
        public static void LoadData()
        {
            TeamName = PlayerPrefs.GetString("TeamName");
            InstructorName = PlayerPrefs.GetString("InstructorName");
            PlayerNames = PlayerPrefs.GetString("PlayerNames").Split(',');
            TeamPhoto.LoadImage(LoadImage());
        }

        private static byte[] LoadImage()
        {
            byte[] imageBytes = Convert.FromBase64String(PlayerPrefs.GetString("TeamPhoto"));
            if (imageBytes.Length > 0)
            {
                return imageBytes;
            }
            else
            {
                return null;
            }
        }
    }
}