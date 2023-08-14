using DefaultNamespace;
using UnityEngine;

// This attribute makes the Question appear in the Create Asset menu
[CreateAssetMenu(fileName = "New Question", menuName = "Quiz/Question")]
public class Question : ScriptableObject
{
    public string questionText;
    public string[] answers;
    public int correctAnswerIndex;
    public string clue;
    public QuestionType type;
}