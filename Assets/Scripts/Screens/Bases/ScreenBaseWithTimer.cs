

namespace Screens.Bases
{
    public class ScreenBaseWithTimer : ScreenBase
    {
        public bool[] SelectedAnswers {get; set;}
        private bool scoreReduction;
        public bool AnswerLocked { get; set; }    

        protected override void Start()
        {
            Init();
        }

        private void Init()
        {
            base.Start();
            EventManager.TimerFinished.AddListener(OnTimerFinished);
            InitAnswerData();
        }

        private void InitAnswerData()
        {
            SelectedAnswers = new bool[answersCount];
        }

        public void OnTimerFinished()
        {
            NextButton.interactable = true;
            scoreReduction = true;
        }

    }
}