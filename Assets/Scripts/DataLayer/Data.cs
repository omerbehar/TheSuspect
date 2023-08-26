using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DataLayer
{
    [Serializable]
    public static class Data
    {
        public const int MAX_PLAYERS = 5;
        public static string guid;
        public static string TeamName { get; set; }
        public static string InstructorName { get; set; }
        public static string[] PlayerNames { get; set; }
        public static Texture2D TeamPhoto { get; set; } = new(1, 1, TextureFormat.ARGB32, false);
        public static Dictionary<string, bool[]> SelectedAnswersData { get; set; } = new();
        public static int Score { get; set; }

        public static void ResetData()
        {
            TeamName = "testteamname";
            InstructorName = "";
            Score = 0;
            PlayerNames = new string[MAX_PLAYERS];
            TeamPhoto = null;
            guid = Guid.NewGuid().ToString();
        }
        //save data to player prefs
        public static void SaveData()
        {
            Debug.Log("Saving data");
            PlayerPrefs.SetString("TeamName", TeamName);
            PlayerPrefs.SetString("InstructorName", InstructorName);
            PlayerPrefs.SetString("PlayerNames", string.Join(",", PlayerNames));
            byte[] bytes = TeamPhoto == null ? null : TeamPhoto.EncodeToPNG();
            if (bytes != null) PlayerPrefs.SetString("TeamPhoto", Convert.ToBase64String(bytes));

            foreach (string key in SelectedAnswersData.Keys)
            {
                PlayerPrefs.SetString(key, string.Join(",", SelectedAnswersData[key]));
            }
            PlayerPrefs.SetInt("Score", Score);
        }
        //load data from player prefs
        public static bool LoadData()
        {
            //if no guid in playerpref, create one and reset data
            if (!PlayerPrefs.HasKey("guid"))
            {
                Debug.Log("No guid found, creating new one");
                ResetData();
                PlayerPrefs.SetString("guid", guid);
                return false;
            }
            else
            {
                Debug.Log("Guid found, loading data");
                guid = PlayerPrefs.GetString("guid");
                TeamName = PlayerPrefs.GetString("TeamName");
                InstructorName = PlayerPrefs.GetString("InstructorName");
                PlayerNames = PlayerPrefs.GetString("PlayerNames").Split(',');
                byte[] bytes = LoadImage();
                TeamPhoto = new Texture2D(1, 1, TextureFormat.ARGB32, false);
                if (bytes != null) TeamPhoto.LoadImage(bytes);
                foreach (string key in SelectedAnswersData.Keys.ToList())
                {
                    if (SelectedAnswersData[key].Length != 0 && PlayerPrefs.GetString(key).Split(',').Length != 0)
                    {
                        if (PlayerPrefs.GetString(key).Split(',')[0] != "")
                        {
                            //test if parsable
                            try
                            {
                                SelectedAnswersData[key] = Array.ConvertAll(PlayerPrefs.GetString(key).Split(','), bool.Parse);
                            }
                            catch (Exception exception)
                            {
                                Debug.LogWarning("Could not parse bool array from PlayerPrefs: " +  exception.Message);
                            }
                            //return true;
                        }
                    }
                }
                Score = PlayerPrefs.GetInt("Score");
            }

            return true;
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