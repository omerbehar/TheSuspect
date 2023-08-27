using DataLayer;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Screens.Bases
{
    public class ScreenBaseWithTimer : ScreenBase
    {
        public bool[] SelectedAnswers {get; set;}
        [SerializeField] protected int scoreIfCorrect = 5;
        [SerializeField] private int scoreReductionIfTimeOut = 2; 
        [SerializeField] private int scoreReductionIfHintUsed = 2; 
        protected bool timeoutScoreReduction;
        protected bool hintUsedScoreReduction;
        protected int answerScore;
        public bool AnswerLocked { get; set; }    

        protected override void Start()
        {
            Init();
        }

        private void Init()
        {
            base.Start();
            EventManager.TimerFinished.AddListener(OnTimerFinished);
            EventManager.HintUsed.AddListener(OnHintUsed);
            InitAnswerData();
        }

        private void InitAnswerData()
        {
            SelectedAnswers = new bool[answersCount];
        }

        public void OnTimerFinished()
        {
            NextButton.interactable = true;
            timeoutScoreReduction = true;
        }
        public void OnHintUsed()
        {
            hintUsedScoreReduction = true;
        }
        
        public override void OnNextButtonClicked()
        {
            CalculateScore();
            base.OnNextButtonClicked();
        }

        private async void CalculateScore()
        {
            int score = 0;
            if (answerScore > 0)
            {
                score += answerScore;
                if (timeoutScoreReduction)
                    score -= scoreReductionIfTimeOut;
                if (hintUsedScoreReduction)
                    score -= scoreReductionIfHintUsed;
                score = Mathf.Max(score, 0);
            }
            await GameManager.instance.AddScoreToManager(SceneManager.GetActiveScene().name, score);
            Debug.Log("Score: " + Data.Score);
        }
    }
}