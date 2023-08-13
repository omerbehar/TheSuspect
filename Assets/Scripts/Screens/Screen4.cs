using DataLayer;
using DefaultNamespace;
using Screens.Bases;
using Screens.Interfaces;
using TMPro;
using UnityEngine;

namespace Screens
{
    public class Screen4 : ScreenBase, ILoadData, ISaveData
    {
        [SerializeField] private TMP_InputField teamNameInputField;
        [SerializeField] private string teamName;

        private void Start()
        {
            base.Start();
            LoadData();
        }
        private void Update()
        {
            UpdateTeamName();
        }

        private void UpdateTeamName()
        {
            teamName = teamNameInputField.text;
            if (teamName != "")
            {
                EventManager.AssignmentCompleted.Invoke();
            }

            SaveData();
        }

        public void LoadData()
        {
            Data.LoadData();
            teamNameInputField.text = Data.TeamName;
            teamName = Data.TeamName;
        }

        public void SaveData()
        {
            Data.TeamName = teamName;
            Data.SaveData();
        }
    }
}