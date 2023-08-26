using UnityEngine;
using UnityEngine.UIElements;
using System;
using System.Collections;

public class NewTimerUI : MonoBehaviour
{
    [field: SerializeField] public UIDocument UIDocument { get; set; }
    [field: SerializeField] public Label TimeText { get; set; }  
    [field: SerializeField] public float TimerDuration { get; set; } 
    public float ScreenTime { get; set; }
    public bool IsTimerRunning { get; set; }
    private Slider TimerSlider;
    private VisualElement TrackerFill;

    private void Start()
    {
        PrepareUI();
        StartCoroutine(UpdateSliderCoroutine());
    }
    
    private void PrepareUI()
    {
        var rootVisualElement = UIDocument.rootVisualElement;
        TimerSlider = rootVisualElement.Q<Slider>("TimerSlider"); 
        TimeText = rootVisualElement.Q<Label>("TimeTextLabel");

        if (TimerSlider != null) 
        {
            TrackerFill = TimerSlider.Q<VisualElement>("unity-tracker"); 
            var dragger = TimerSlider.Q<VisualElement>("unity-dragger"); 
            if (dragger != null)
            {
                dragger.style.display = DisplayStyle.None; 
            }
        }

        if (TrackerFill == null || TimerSlider == null || TimeText == null)
        {
            Debug.LogWarning("Slider, TimeText or Tracker Fill is not found!");
        }

        TimerSlider.lowValue = 0;
        TimerSlider.highValue = TimerDuration;
        TimerSlider.value = TimerDuration;
        TimeText.text = TimerDuration.ToString("F2");
        IsTimerRunning = true;
    }

    private IEnumerator UpdateSliderCoroutine()
    {
        float startTime = Time.time;
    
        while (IsTimerRunning)
        {
            float timeRatio = (Time.time - startTime) / TimerDuration;
            float remainingTime = Mathf.Lerp(TimerDuration, 0, timeRatio);

            TimerSlider.value = remainingTime;
            TimeText.text = TimeSpan.FromSeconds(remainingTime).ToString(@"mm\:ss");

            if (TrackerFill != null)
            {
                TrackerFill.style.width = new StyleLength(Length.Percent(remainingTime / TimerDuration * 100));
                TrackerFill.style.backgroundColor = Color.Lerp(Color.red, Color.green, remainingTime / TimerDuration);
            }

            yield return null;
        }
    }
}