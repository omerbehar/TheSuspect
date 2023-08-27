using System.Threading.Tasks;
using DataLayer;
using Screens.Bases;
using Screens.Interfaces;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Screens
{
    public class Screen2_6 : ScreenBaseWithTimer, ILoadData, ISaveData, IHint
    {
        //[field: SerializeField] public int CorrectAnswerScore { get; set; }
        //[field: SerializeField] public Button HintButton { get; set; }
        [field: SerializeField] public GameObject HintPopup { get; set; }
        [field: SerializeField] public Button HintPopupCloseButton { get; set; }
        [field: SerializeField] public GameObject CorrectAnswer { get; set; }
        public int PotentialScore { get; set; }
        private Toggle[] toggles;


        protected override void Start()
        {
            Init();
        }

        private async void Init()
        {
            InitToggles();
            base.Start();
            await LoadData();
            HintButton.onClick.AddListener(OnHintButtonClicked);
        }

        public async Task LoadData()
        {
            if (Data.SelectedAnswersData[SceneManager.GetActiveScene().name].Length != answersCount)
                Data.SelectedAnswersData[SceneManager.GetActiveScene().name] = new bool[answersCount];
            Data.LoadData();
            SelectedAnswers = Data.SelectedAnswersData[SceneManager.GetActiveScene().name];
            for (int i = 0; i < toggles.Length; i++)
            {
                toggles[i].isOn = SelectedAnswers[i];
            }
        }
        public async Task SaveData()
        {
            for (int i = 0; i < toggles.Length; i++)
            {
                 SelectedAnswers[i] = toggles[i].isOn;
            }
            Data.SelectedAnswersData[SceneManager.GetActiveScene().name] = SelectedAnswers;
            Data.SaveData();
            await Database.SaveDataToDatabase();
        }
        private void InitToggles()
        {
            toggles = GetComponentsInChildren<Toggle>();
            foreach (Toggle toggle in toggles)
            {
                toggle.onValueChanged.AddListener(delegate { SingleToggleSelection(toggle); });
            }
            answersCount = toggles.Length;
        }

        public void OnAnswer()
        {
            AnswerLocked = true;
            foreach (Toggle toggle in toggles)
            {
                toggle.interactable = false;
            }
            if (CorrectAnswer.GetComponentInChildren<Toggle>().isOn)
            {
                answerScore = scoreIfCorrect;
            }
            else
            {
                answerScore = 0;
            }
            Data.SaveData();
        }


        private void SingleToggleSelection(Toggle selectedToggle)
        {
            if (selectedToggle.isOn)
            {
                foreach (Toggle toggle in toggles)
                {
                    if (toggle != selectedToggle)
                        toggle.isOn = false;
                }

                OnSelect(selectedToggle.gameObject);
            }
            else PotentialScore = 0;
            SaveData();
        }
        public void OnSelect(GameObject selectedGameObject)
        {
            EventManager.AssignmentCompleted.Invoke();
            PotentialScore = selectedGameObject == CorrectAnswer ? scoreIfCorrect : 0;
        }

        public void OnHintButtonClicked()
        {
            hintUsedScoreReduction = true;
            HintPopup.SetActive(true);
            HintPopupCloseButton.onClick.AddListener(OnHintPopupCloseButtonClicked);
        }

        private void OnHintPopupCloseButtonClicked()
        {
            HintPopup.SetActive(false);
        }

        public override void OnNextButtonClicked()
        {
            OnAnswer();
            base.OnNextButtonClicked();
        }
    }
}