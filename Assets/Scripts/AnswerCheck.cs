using Screens.Bases;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class AnswerCheck : ScreenBaseWithTimer
{
    [SerializeField]
    private UnityEvent CorrectAnswerEvent;

    [SerializeField]
    private UnityEvent IncorrectAnswerEvent;

    [SerializeField]
    private InputField inputField;

    [SerializeField]
    private string correctAnswer;

    protected override void Start()
    {
        base.Start();
        // This will immediately disable the button until an answer is input.
        NextButton.interactable = false;
        inputField.onValueChanged.AddListener(delegate { OnInputFieldValueChanged(); });
    }

    private void OnInputFieldValueChanged()
    {
        // If any field input is changed, set the NextButton as interactable.
        NextButton.interactable = true;
    }

    public override void OnNextButtonClicked()
    {
        if (inputField.text == correctAnswer)
        {
            //Debug.Log("The correct answer was input.");
            CorrectAnswerEvent?.Invoke();
        }
        else
        {
            //Debug.Log("The incorrect answer was input.");
            IncorrectAnswerEvent?.Invoke();
        }
        
        base.OnNextButtonClicked();
    }
    public void LinkClicked()
    {
        Application.OpenURL("https://levana.org.il/%D7%A9%D7%A4%D7%AA-%D7%94%D7%A1%D7%99%D7%9E%D7%A0%D7%99%D7%9D/");
    }

   
}