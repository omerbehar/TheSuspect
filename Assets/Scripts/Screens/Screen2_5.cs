using System;
using System.Threading.Tasks;
using DataLayer;
using Screens.Bases;
using Screens.Interfaces;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Screens
{
    public class Screen2_5 : ScreenBaseWithTimer, ISaveData, ILoadData
    {
        [SerializeField] private Button hintButton;
        [SerializeField] private GameObject hintPopup;
        string correctAnswer1 = "68000000";
        string correctAnswer2 = "770";
        string correctAnswer3 = "24";
        string[] answersText = new string[3];
        InputField[] inputFields = new InputField[3];
        protected override void Start()
        {
            base.Start();
            inputFields = GetComponentsInChildren<InputField>();
            Debug.Log(inputFields.Length);
            LoadData();
            if (Data.AnswersText.ContainsKey(SceneManager.GetActiveScene().name)) Debug.Log(Data.AnswersText[SceneManager.GetActiveScene().name][0]);
            hintButton.onClick.AddListener(OnHintButtonClicked);
            foreach (InputField inputField in inputFields)
            {
                inputField.onValueChanged.AddListener(delegate { OnValueChanged(inputField); });
            }
            IsAssignmentCompleted();
            IsLocked();
        }

        private void IsLocked()
        {
            if (AnswerLocked)
            {
                foreach (InputField inputField in inputFields)
                {
                    inputField.interactable = false;
                }
            };
        }

        private async void OnValueChanged(InputField inputField)
        {
            Debug.Log(inputField.text);
            answersText = new [] {inputFields[0].text, inputFields[1].text, inputFields[2].text};
            await SaveData();
            IsAssignmentCompleted();

        }

        private void IsAssignmentCompleted()
        {
            foreach (string answerText in answersText)
            {
                if (answerText == "")
                {
                    EventManager.AssignmentNotCompleted.Invoke();
                    return;
                }
            }
            EventManager.AssignmentCompleted.Invoke();
        }

        private void OnHintButtonClicked()
        {
            EventManager.HintUsed.Invoke();
            hintPopup.SetActive(true);
        }

        public override void OnNextButtonClicked()
        {
            CheckAnswers();
            base.OnNextButtonClicked();
        }

        private void CheckAnswers()
        {
            if (inputFields[0].text == correctAnswer1)
                answerScore += scoreIfCorrect / 3;
            if (inputFields[1].text == correctAnswer2)
                answerScore += scoreIfCorrect / 3;
            if (inputFields[2].text == correctAnswer3)
                answerScore += scoreIfCorrect / 3;
            AnswerLocked = true;
            answersText = new [] {inputFields[0].text, inputFields[1].text, inputFields[2].text};
        }

        public async Task SaveData()
        {
            if (Data.AnswersText.ContainsKey(SceneManager.GetActiveScene().name))
                Data.AnswersText[SceneManager.GetActiveScene().name] = answersText;
            else
                Data.AnswersText.Add(SceneManager.GetActiveScene().name, answersText);
            if (Data.AnswerLocked.ContainsKey(SceneManager.GetActiveScene().name + "isLocked"))
                Data.AnswerLocked[SceneManager.GetActiveScene().name + "isLocked"] = AnswerLocked;
            else
                Data.AnswerLocked.Add(SceneManager.GetActiveScene().name + "isLocked", AnswerLocked);
            Data.SaveData();
            
            await Database.SaveDataToDatabase();
        }

        public Task LoadData()
        {
            Data.LoadData();
            if (Data.AnswersText.ContainsKey(SceneManager.GetActiveScene().name))
            {
                answersText = Data.AnswersText[SceneManager.GetActiveScene().name];
                inputFields[0].text = answersText[0];
                inputFields[1].text = answersText[1];
                inputFields[2].text = answersText[2];
            }
            if (Data.AnswerLocked.ContainsKey(SceneManager.GetActiveScene().name + "isLocked"))
                AnswerLocked = Data.AnswerLocked[SceneManager.GetActiveScene().name + "isLocked"];
            Debug.Log(AnswerLocked);
            return Task.CompletedTask;
        }
    }
}