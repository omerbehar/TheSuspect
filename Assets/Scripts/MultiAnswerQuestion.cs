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
    [SerializeField] private GameObject failedGO;
    [SerializeField] private GameObject failedAgainGO;
    private int incorrectTries = 0; // Counter for incorrect tries

    private int incorrectAnswerCount;
    [SerializeField] private GameObject[] wrongAnswerMessage;
    [SerializeField] private GameObject[] correctAnswerMessage;

    protected override void Start()
    {
        base.Start(); 

        foreach (var toggle in toggles)
        {
            toggle.onValueChanged.AddListener(OnToggleValueChanged);
            Debug.Log(toggle.name + " add listener ");
        }

        // This will not be interactable until at least one answer is selected
        NextButton.interactable = false;
    }
    private void ActivateFailedMessage()
    {
        if (incorrectTries == 0) failedGO.SetActive(true);
        else
        {
            failedAgainGO.SetActive(true);
        }
        foreach (Toggle toggle in toggles)
        {
            
        }
    }

    public override void OnNextButtonClicked()
    {
        CalculateCorrectAnswers();

        if (AreAllCorrectSelected())
        {
            AllCorrectAnswerEvent?.Invoke();
            //Debug.Log("All correct answers have been selected.");
            base.OnNextButtonClicked();
        }
        else
        {
            ActivateFailedMessage();
            incorrectTries++;
            //Debug.Log("Not all correct answers have been selected.");
        }
        //Debug.Log($"Correct answers selected: {correctAnswerCount}");
    }

    public void FailedNextSceneButtonClicked()
    {
        base.OnNextButtonClicked();
    }
    private void OnToggleValueChanged(bool isOn)
    {
        // If any toggle is clicked, set the NextButton as interactable
        Debug.Log(" some toggle value changed ");

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
        int wrongAnswerCount = 0;
        foreach (int correctIndex in correctAnswerIndices)
        {
            if (!toggles[correctIndex].isOn)
            {
                wrongAnswerCount++;
            }
            else
            {
                correctAnswerMessage[correctIndex].SetActive(true);
            }
        }

        for (int i = 0; i < toggles.Count; i++)
        {
            if (!correctAnswerIndices.Contains(i) && toggles[i].isOn)
            {
                wrongAnswerCount++;
                wrongAnswerMessage[i].SetActive(true);
            }
        }

        return wrongAnswerCount == 0;
    }
}