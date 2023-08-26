using System;
using System.Collections;
using System.Collections.Generic;
using Screens.Interfaces;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TimerScript : MonoBehaviour, ITimer
{
        [field: SerializeField] public Slider TimerSlider { get; set; }
        [field: SerializeField] public TMP_Text TimeText { get; set; }
        [field: SerializeField] public float TimerDuration { get; set; }
        public float ScreenTime { get; set; }
        public bool IsTimerRunning { get; set; }
        private bool scoreReduction;

        protected void Start()
        {
            Init();
        }

        private void Init()
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
            throw new NotImplementedException();
        }

        public void ResetTimer()
        {
            throw new NotImplementedException();
        }
}
