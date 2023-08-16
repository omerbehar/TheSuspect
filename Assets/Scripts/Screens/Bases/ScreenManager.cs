using System;
using System.Collections.Generic;
using System.Linq;
using DefaultNamespace;
using UnityEngine;
using UnityEngine.UIElements;

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

    private void DisplayCurrentQuestion()
    {
        var root = FindObjectOfType<UIDocument>().rootVisualElement;
        var questionLabel = root.Q<Label>("QuestionLabel");
        questionLabel.text = questions[currentQuestionIndex].QuestionText;

        answerListView.itemsSource = questions[currentQuestionIndex].GetAnswerOptions();
        answerListView.makeItem = () => new Label();
        answerListView.bindItem = (e, i) => ((Label) e).text = questions[currentQuestionIndex].GetAnswerOptions()[i];
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
}