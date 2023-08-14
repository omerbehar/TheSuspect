using DefaultNamespace;
using Screens.Interfaces;
using UnityEngine;

// This attribute makes the Question appear in the Create Asset menu
[CreateAssetMenu(fileName = "New Question", menuName = "Quiz/Question")]
public class Question : ScriptableObject, IQuestion
{
    public string questionText;
    public string[] answers;
    public int correctAnswerIndex;
    public string clue;
    public QuestionType type;

    public string QuestionText { get => questionText; set => questionText = value; }
    public QuestionType Type { get => type; set => type = value; }

    // You will implement these methods according to the type of question.
    public void Display()
    {
        throw new System.NotImplementedException();
    }

    public bool CheckAnswer(int answerIndex)
    {
        throw new System.NotImplementedException();
    }
}