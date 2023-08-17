using System;
using System.Collections.Generic;
using System.Linq;
using DefaultNamespace;
using UnityEngine;
using UnityEngine.UIElements;
using Screens.Bases;

public class ScreenManager : MonoBehaviour
{
    [SerializeField]
    private List<Question> questions = new List<Question>();
    private int currentQuestionIndex = 0;

    private ListView answerListView;

    private void Start()
    {
        InitializeListView();
        DisplayCurrentQuestion();
    }

    private void Update()
    {
        // If some condition met (e.g., user has selected an answer), move to next question
        if(CurrentQuestionIsAnswered())
        {
            currentQuestionIndex++;
            if(currentQuestionIndex >= questions.Count)
                currentQuestionIndex = 0; // Loop back to the first question
                
            DisplayCurrentQuestion();
        }
    }

    private void InitializeListView()
    {
        var root = FindObjectOfType<UIDocument>().rootVisualElement;
        answerListView = new ListView();
        answerListView.style.flexGrow = 1;
        root.Q<VisualElement>("Main").Add(answerListView);
    }

    public void DisplayCurrentQuestion()
    {
        var root = FindObjectOfType<UIDocument>().rootVisualElement;
        var questionLabel = root.Q<Label>("QuestionLabel");
        questionLabel.text = questions[currentQuestionIndex].QuestionText;

        answerListView.itemsSource = questions[currentQuestionIndex].GetAnswerOptions();
        answerListView.makeItem = MakeSingleChoiceAnswerVO;
        answerListView.bindItem = BindItemToSingleChoiceAnswerVO;
        
        answerListView.Rebuild();
    }

    private VisualElement MakeSingleChoiceAnswerVO()
    {
        var singleChoiceAnswerVO = new SingleChoiceAnswerVO();
        singleChoiceAnswerVO.onAnswerClicked = HandleAnswerClicked;
        return singleChoiceAnswerVO;
    }

    private void BindItemToSingleChoiceAnswerVO(VisualElement element, int index)
    {
        var singleChoiceAnswerVO = (SingleChoiceAnswerVO) element;
        singleChoiceAnswerVO.answerLabel.text = questions[currentQuestionIndex].GetAnswerOptions()[index];
        singleChoiceAnswerVO.OptionIndex = index; // Set the index
    }
    private bool CurrentQuestionIsAnswered()
    {
        Question currentQuestion = questions[currentQuestionIndex];

        switch (currentQuestion.Type)
        {
            case QuestionType.SingleChoice:
                RadioButton radioButton = answerListView.Q<RadioButton>();
                // If RadioButton exists and is selected
                if (radioButton != null && radioButton.value) 
                {
                    return true;
                }
                break;

            case QuestionType.MultipleChoice:
                foreach (Toggle checkbox in answerListView.Children().OfType<Toggle>())
                {
                    // If any Checkbox (Toggle) is checked
                    if (checkbox.value) 
                    {
                        return true;
                    }
                }
                break;

            // Handle other question types as necessary

            default:
                throw new NotImplementedException($"No answer check implemented for question type {currentQuestion.Type}");
        }

        // If no answer is selected
        return false;
    }
    
    public void HandleAnswerClicked(int answerIndex)
    {
        Question currentQuestion = questions[currentQuestionIndex];
        var isCorrectAnswer = currentQuestion.CheckAnswer(new int[] { answerIndex });
        if (isCorrectAnswer)
        {
            Debug.Log("Correct Answer! The selected index is: " + answerIndex);
            // Handle correct answer here
        }
        else
        {
            Debug.Log("Incorrect Answer! The selected index is: " + answerIndex);
            // Handle incorrect answer here
        }
        // Whatever other action you wish to perform when an answer is clicked
    }
}