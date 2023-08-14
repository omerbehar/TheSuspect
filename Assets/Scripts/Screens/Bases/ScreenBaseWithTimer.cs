using System;
using Screens.Interfaces;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Screens.Bases
{
    public class ScreenBaseWithTimer : ScreenBase, ITimer
    {
        [field: SerializeField] public Slider TimerSlider { get; set; }
        [field: SerializeField] public TMP_Text TimeText { get; set; }
        [field: SerializeField] public float TimerDuration { get; set; }
        public float ScreenTime { get; set; }
        public bool IsTimerRunning { get; set; }
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
            InitTimer();
            InitAnswerData();
        }

        private void InitAnswerData()
        {
            SelectedAnswers = new bool[answersCount];
        }

        private void InitTimer()
        {
            EventManager.TimerFinished.AddListener(OnTimerFinished);
            if (TimerSlider == null)
                Debug.LogWarning("TimerSlider is null");
            if (TimeText == null)
                Debug.LogWarning("TimeText is null");
            if (TimerDuration <= 0)
                Debug.LogWarning("TimerDuration is 0 or less");
            TimerSlider.maxValue = TimerDuration;
            TimerSlider.value = 0;
            TimeText.text = TimerDuration.ToString("F2");
            StartTimer();
        }

        public void Update()
        {
            UpdateTimer();
        }

        public void UpdateTimer()
        {
            if (IsTimerRunning)
            {
                if (ScreenTime >= TimerDuration)
                {
                    EventManager.TimerFinished.Invoke();
                }
                else
                {
                    ScreenTime += Time.deltaTime;
                    TimerSlider.value = TimerDuration - ScreenTime;
                    TimeText.text = TimeSpan.FromSeconds(TimerDuration - ScreenTime).ToString(@"mm\:ss");
                }
            }
        }

        public void StartTimer()
        {
            IsTimerRunning = true;
        }

        public void StopTimer()
        {
            IsTimerRunning = false;
        }

        public void ResetTimer()
        {
            ScreenTime = 0;
        }

        public void OnTimerFinished()
        {
            NextButton.interactable = true;
            scoreReduction = true;
        }

    }
}