using TMPro;
using UnityEngine.UI;

namespace Screens.Interfaces
{
    public interface ITimer
    {
        Slider TimerSlider { get; set; }
        TMP_Text TimeText { get; set; }
        float ScreenTime { get; set; }
        bool IsTimerRunning { get; set; }
        float TimerDuration { get; set; }
        void StartTimer();
        void StopTimer();
        void ResetTimer();
        void UpdateTimer();
    }
}