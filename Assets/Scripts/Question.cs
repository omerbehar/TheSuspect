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
    [SerializeField] protected List<int> correctAnswerIndices = new List<int>(); // Made up correct answer indices 
    
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

    public abstract bool CheckAnswer(int[] selectedAnswers);
    public VisualTreeAsset GetLayoutAsset() 
    {
        return layoutAsset;
    }


    public string GetQuestionText()
    {
        throw new System.NotImplementedException();
    }
}