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
    [SerializeField] private float intemHight = 60f; 
    [SerializeField] private float SPACING = 10f; // Define spacing between items

    // Variable to control the spacing between items
    [SerializeField] private float itemSpacing = 10f;

    private void Start()
    {
        InitializeListView();
        DisplayCurrentQuestion();
    }

   
    private void InitializeListView()
    {
        var root = FindObjectOfType<UIDocument>().rootVisualElement;
    
        answerListView = new ListView(questions, intemHight + SPACING, MakeSingleChoiceAnswerVO, BindItemToSingleChoiceAnswerVO);
        
        //make listview not show scrollbar
        var scrollView = answerListView.Q<ScrollView>();
        scrollView.verticalScrollerVisibility = ScrollerVisibility.Hidden;
        var b = scrollView.touchScrollBehavior == ScrollView.TouchScrollBehavior.Clamped;
      
        answerListView.selectionType = SelectionType.None;
        

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

        // Define the main box (container) to hold your VO and a spacer
        var container = new VisualElement();
        container.Add(singleChoiceAnswerVO);

        // Define an additional VisualElement to act as a spacer between the items
        var spacer = new VisualElement();
        spacer.style.height = itemSpacing;  // Set the spacer's height as the desired spacing
        spacer.style.backgroundColor = Color.clear; // Set it transparent

        container.Add(spacer); // Add the spacer to the container
        return container;  // Return the main box with the VO and spacer
    }

    private void BindItemToSingleChoiceAnswerVO(VisualElement element, int index)
    {
        // Adjust this to work with your new container layout
        var singleChoiceAnswerVO = (SingleChoiceAnswerVO)element[0];
        singleChoiceAnswerVO.answerLabel.text = questions[currentQuestionIndex].GetAnswerOptions()[index];
        singleChoiceAnswerVO.OptionIndex = index;
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