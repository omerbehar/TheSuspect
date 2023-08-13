using System;
using DefaultNamespace;
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

        private void Start()
        {
            InitTimer();
        }

        private void InitTimer()
        {
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

        private void Update()
        {
            UpdateTimer();
        }

        public void UpdateTimer()
        {
            if (IsTimerRunning)
            {
                if (ScreenTime >= TimerDuration)
                {
                    OnTimerFinished();
                }
                else
                {
                    ScreenTime += Time.deltaTime;
                    TimerSlider.value = ScreenTime;
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
            EventManager.TimerFinished.Invoke();
        }
    }
}