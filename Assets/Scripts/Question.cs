using System.Collections.Generic;
using DefaultNamespace;
using Screens.Interfaces;
using Screens.Interfaces.Screens.Interfaces;
using UnityEngine;
using UnityEngine.UIElements;

// This attribute makes the Question appear in the Create Asset menu
[CreateAssetMenu(fileName = "New Question", menuName = "Quiz/Question")]
public abstract class Question : ScriptableObject, IQuestion
{
    [SerializeField] protected string questionText;
    [SerializeField] protected VisualTreeAsset layoutAsset;
    [SerializeField] protected List<string> answers = new List<string>(); 
    [SerializeField] public List<int> correctAnswerIndices = new List<int>(); // Made up correct answer indices 
    
    public string QuestionText { get => questionText; }
    [SerializeField]
    private QuestionType _type;

    public QuestionType Type { get { return _type; } }

    public abstract VisualElement DisplayQuestionUI();
    public List<string> GetAnswerOptions()
    {
        return answers;
    }
    public VisualTreeAsset RadioButtonTemplate
    {
        get { return layoutAsset; }
    }

    public int TotalCorrectAnswers { get; set; }

    public abstract bool CheckAnswer(int[] selectedAnswers);
    public VisualTreeAsset GetLayoutAsset() 
    {
        return layoutAsset;
    }


    public int GetCorrectAnswerCount()
    {
        return answers.Count;
    }
}