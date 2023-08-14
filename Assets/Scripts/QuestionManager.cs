using DefaultNamespace;
using UnityEngine;
using UnityEngine.UI;

public class QuestionManager : MonoBehaviour
{
    public Text questionText;
    public GameObject[] answerButtons;
    public GameObject fillInTheBlankInputField;
    public GameObject clueButton;

    // We're now using an array of ScriptableObjects
    public Question[] questions;
    private int currentQuestionIndex = 0;

    private void Start()
    {
        DisplayQuestion(questions[currentQuestionIndex]);
    }

    public void DisplayQuestion(Question question)
    {
        questionText.text = question.questionText;

        foreach(var button in answerButtons)
        {
            button.SetActive(false);
        }

        fillInTheBlankInputField.SetActive(false);
        
        switch(question.type)
        {
            case QuestionType.MultipleChoice:
                for (int i = 0; i < question.answers.Length; i++)
                {
                    answerButtons[i].SetActive(true);
                    answerButtons[i].GetComponentInChildren<Text>().text = question.answers[i];
                }
                break;
            case QuestionType.FillInTheBlank:
                fillInTheBlankInputField.SetActive(true);
                break;
            case QuestionType.Sentence:
                // Handle Sentence Type Question
                break;
        }
    }

    public void OnContinueButtonPressed() 
    {
        // Go to the next question if available
        if (currentQuestionIndex < questions.Length - 1)
        {
            currentQuestionIndex += 1;
            DisplayQuestion(questions[currentQuestionIndex]);
        }
    }

    // Add logic to handle user answers and checking against correct answers
}