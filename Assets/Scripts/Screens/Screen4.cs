using System.Threading.Tasks;
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

        private async void Start()
        {
            base.Start();
            await LoadData();
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

        public Task LoadData()
        {
            Data.LoadData();
            teamNameInputField.text = Data.TeamName;
            teamName = Data.TeamName;
            return Task.CompletedTask;
        }

        public void SaveData()
        {
            Data.TeamName = teamName;
            Data.SaveData();
        }
    }
}