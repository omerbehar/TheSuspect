using System.Threading.Tasks;
using DataLayer;
using Screens.Bases;
using Screens.Interfaces;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Screens
{
    public class Screen4 : ScreenBase, ILoadData, ISaveData
    {
        [SerializeField] private InputField teamNameInputField;
        [SerializeField] private string teamName;

        private async void Start()
        {
            base.Start();
            await LoadData();
            AddListeners();
            IsAssignmentCompleted();
        }

        private void AddListeners()
        {
            teamNameInputField.onValueChanged.AddListener(delegate { UpdateTeamName(); });
        }
        
        private async void UpdateTeamName()
        {
            teamName = teamNameInputField.text;
            IsAssignmentCompleted();
            await SaveData();
        }

        private void IsAssignmentCompleted()
        {
            if (teamName != "")
            {
                EventManager.AssignmentCompleted.Invoke();
            }
        }

        public Task LoadData()
        {
            Data.LoadData();
            teamNameInputField.text = Data.TeamName;
            teamName = Data.TeamName;
            return Task.CompletedTask;
        }

        public async Task SaveData()
        {
            Data.TeamName = teamName;
            Data.SaveData();
            await Database.SaveDataToDatabase();
        }
    }
}