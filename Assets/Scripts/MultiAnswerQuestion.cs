using System.Collections.Generic;
using Screens.Bases;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class MultiAnswerQuestion : ScreenBaseWithTimer
{
    [SerializeField] int correctAnswerCount = 0;

    [SerializeField]
    private List<Toggle> toggles;
    
    [SerializeField]
    private List<int> correctAnswerIndices;

    [SerializeField]
    public UnityEvent AllCorrectAnswerEvent;

    private int incorrectAnswerCount;

    protected override void Start()
    {
        base.Start(); 

        foreach (var toggle in toggles)
        {
            toggle.onValueChanged.AddListener(OnToggleValueChanged);
        }

        // This will not be interactable until at least one answer is selected
        NextButton.interactable = false;
    }

    public override void OnNextButtonClicked()
    {
        CalculateCorrectAnswers();

        if (AreAllCorrectSelected())
        {
            AllCorrectAnswerEvent?.Invoke();
            //Debug.Log("All correct answers have been selected.");
        }
        else
        {
            //Debug.Log("Not all correct answers have been selected.");
        }
        //Debug.Log($"Correct answers selected: {correctAnswerCount}");
        base.OnNextButtonClicked();
    }
    

    private void OnToggleValueChanged(bool isOn)
    {
        // If any toggle is clicked, set the NextButton as interactable
        NextButton.interactable = true;
        CalculateCorrectAnswers();
    }

    private void CalculateCorrectAnswers()
    {
        correctAnswerCount = 0;
        foreach (var correctIndex in correctAnswerIndices)
        {
            if (toggles[correctIndex].isOn)
            {
                correctAnswerCount++;
            }
        }

        foreach (Toggle toggle in toggles)
        {
            if (toggle.isOn && !correctAnswerIndices.Contains(toggles.IndexOf(toggle)))
            {
                incorrectAnswerCount++;
            }
        }

        answerScore = scoreIfCorrect / correctAnswerIndices.Count * correctAnswerCount - incorrectAnswerCount;
        answerScore = Mathf.Clamp(answerScore, 0, scoreIfCorrect);
    }

    private bool AreAllCorrectSelected()
    {
        foreach (var correctIndex in correctAnswerIndices)
        {
            if (!toggles[correctIndex].isOn)
            {
                return false;
            }
        }

        for (int i = 0; i < toggles.Count; i++)
        {
            if (!correctAnswerIndices.Contains(i) && toggles[i].isOn)
            {
                //Debug.Log("Incorrect answer selected at index: " + i);
                return false;
            }
        }

        return true;
    }
}