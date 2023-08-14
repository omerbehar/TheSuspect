using System.Linq;
using DefaultNamespace;
using UnityEngine;
using UnityEngine.UI;

public class QuestionManager : MonoBehaviour
{
    public Text questionText;
    public GameObject[] answerButtons;
    public GameObject fillInTheBlankInputField;
    public GameObject clueButton;

    // Using an array of Question ScriptableObjects
    public Question[] questions;
    private int currentQuestionIndex = 0;

    private void Start()
    {
        DisplayQuestion(questions[currentQuestionIndex]);
    }

    public void DisplayQuestion(Question question)
    {
        questionText.text = question.questionText;

        foreach (var button in answerButtons)
        {
            button.SetActive(false);
        }
        fillInTheBlankInputField.SetActive(false);

        switch (question.type)
        {
            case QuestionType.MultipleChoice:
                for (int i = 0; i < question.answers.Length; i++)
                {
                    answerButtons[i].SetActive(true);
                    answerButtons[i].GetComponentInChildren<Text>().text = question.answers[i];
                }
                break;
            
            case QuestionType.SingleChoice:
                // Assuming here that the first button is used for single choice answer
                answerButtons[0].SetActive(true);
                answerButtons[0].GetComponentInChildren<Text>().text = question.answers[0];
                break;
            
            case QuestionType.FillInTheBlank:
                fillInTheBlankInputField.SetActive(true);
                break;
            case QuestionType.Sentence:
                // Handle Sentance Type Question
                break;
        }
    }

    public void CheckAnswer(int answerIndex)
    {
        Question currentQuestion = questions[currentQuestionIndex];

        if (currentQuestion.correctAnswerIndices.Contains(answerIndex))
        {
            Debug.Log("Correct answer!");
        }
        else
        {
            Debug.Log("Incorrect answer.");
        }

        currentQuestionIndex++;

        if (currentQuestionIndex < questions.Length)
        {
            DisplayQuestion(questions[currentQuestionIndex]);
        }
        else
        {
            Debug.Log("Quiz completed!");
        }
    }
    public void OnContinueButtonPressed()
    {
        if (currentQuestionIndex < questions.Length - 1)
        {
            currentQuestionIndex += 1;
            DisplayQuestion(questions[currentQuestionIndex]);
        }
    }
}