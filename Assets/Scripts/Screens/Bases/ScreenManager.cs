using System;
using System.Collections.Generic;
using System.Linq;
using DefaultNamespace;
using UnityEngine;
using UnityEngine.UIElements;

namespace Screens.Bases
{
    public class ScreenManager : MonoBehaviour
    {
        public Question[] questions;
        private int currentQuestionIndex = 0;

        private Label questionText;
        private VisualElement answersContainer;
        private TextField fillInTheBlankInputField;
        private Button continueButton;

        private void Awake()
        {
            var root = FindObjectOfType<UIDocument>().rootVisualElement;

            questionText = root.Q<Label>("questionText");
            answersContainer = root.Q<VisualElement>("answersContainer");
            fillInTheBlankInputField = root.Q<TextField>("fillInTheBlankInputField");
            continueButton = root.Q<Button>("continueButton");

            continueButton.clicked += OnContinueButtonPressed;

            DisplayQuestion(questions[currentQuestionIndex]);
        }

        public void DisplayQuestion(Question question)
        {
            questionText.text = question.questionText;

            // Clear previous answers
            answersContainer.Clear();

            switch (question.type)
            {
                case QuestionType.MultipleChoice:
                    for (int i = 0; i < question.answers.Length; i++)
                    {
                        var answerToggle = new Toggle { text = question.answers[i] };
                        answersContainer.Add(answerToggle);
                    }
                    break;
            
                case QuestionType.SingleChoice:
                    for (int i = 0; i < question.answers.Length; i++)
                    {
                        var radioButton = new RadioButton { text = question.answers[i] };
                        answersContainer.Add(radioButton);
                    }
                    break;
            
                case QuestionType.FillInTheBlank:
                    fillInTheBlankInputField.visible = true;
                    break;
            
                case QuestionType.Sentence:
                    // Handle Sentance Type Question
                    break;
                default:
                    throw new System.NotSupportedException($"QuestionType {question.type} is not supported.");
            }
        }
        public void CheckAnswer()
        {
            Question currentQuestion = questions[currentQuestionIndex];
            bool isCorrect = false;

            switch (currentQuestion.Type)
            {
                case QuestionType.MultipleChoice:
                    int[] selectedAnswerIndices = GetSelectedAnswerIndices();
                    isCorrect = IsAnswerCorrect(selectedAnswerIndices, currentQuestion.correctAnswerIndices);
                    break;

                case QuestionType.SingleChoice:
                    int singleSelectedIndex = GetSelectedAnswerIndex(); // Assuming GetSelectedAnswerIndex is designed for SingleChoice questions
                    isCorrect = IsAnswerCorrect(new int[] { singleSelectedIndex }, currentQuestion.correctAnswerIndices);
                    break;

                case QuestionType.FillInTheBlank:
                    string selectedAnswerText = GetSelectedAnswerText();
                    isCorrect = selectedAnswerText.Equals(currentQuestion.answers[0]);
                    break;
            
                case QuestionType.Sentence:
                    // Handle Sentance Type Question
                    break;

                default:
                    throw new System.NotImplementedException($"Question type {currentQuestion.type} not supported.");
            }

            if (isCorrect)
            {
                Debug.Log("Correct answer!");
            }
            else
            {
                Debug.Log("Wrong answer. Try again.");
            }

            // If there are more questions, then show the next question
            if (currentQuestionIndex < questions.Length - 1)
            {
                currentQuestionIndex++;
                DisplayQuestion(questions[currentQuestionIndex]);
            }
            else
            {
                Debug.Log("Quiz completed!");
            }
        }

        private static bool IsAnswerCorrect(int[] selectedAnswerIndices, int[] correctAnswerIndices)
        {
            return selectedAnswerIndices.Length == correctAnswerIndices.Length &&
                   selectedAnswerIndices.OrderBy(i => i).SequenceEqual(correctAnswerIndices.OrderBy(i => i));
        }
        private int[] GetSelectedAnswerIndices()
        {
            var indices = new List<int>();

            switch (questions[currentQuestionIndex].type)
            {
                case QuestionType.MultipleChoice:
                    int i = 0;
                    foreach (VisualElement child in answersContainer.Children())
                    {
                        if (child is Toggle toggle && toggle.value)
                        {
                            indices.Add(i);
                        }
                        i++;
                    }
                    break;

                case QuestionType.SingleChoice:
                    int j = 0;
                    foreach (VisualElement child in answersContainer.Children())
                    {
                        if (child is Toggle toggle && toggle.value)
                        {
                            indices.Add(j);
                            break;
                        }
                        j++;
                    }
                    break;

                case QuestionType.FillInTheBlank:
                    /* For fill in the blank questions, we might not be dealing with indices, but with
                the actual content of an input field. This means we might need a separate method to
                get the input field content and a different approach in the CheckAnswer method. */
                    break;

                case QuestionType.Sentence:
                    // Similar situation as FillInTheBlank
                    break;

                default:
                    throw new System.NotImplementedException($"Question type {questions[currentQuestionIndex].type} not supported.");
            }

            return indices.ToArray();
        }
        private int GetSelectedAnswerIndex()
        {
            switch (questions[currentQuestionIndex].type)
            {
                case QuestionType.MultipleChoice:
                    var toggles = answersContainer.Children().OfType<Toggle>();
                    // Correction starts here
                    return toggles.Select((toggle, index) => new { Index = index, Toggle = toggle })
                        .FirstOrDefault(pair => pair.Toggle.value)
                        ?.Index ?? -1;
                // Correction ends here

                case QuestionType.SingleChoice:
                    var radioButtons = answersContainer.Children().OfType<RadioButton>();
                    // Correction starts here
                    return radioButtons.Select((radioButton, index) => new { Index = index, Radio = radioButton })
                        .FirstOrDefault(pair => pair.Radio.value)
                        ?.Index ?? -1;
                // Correction ends here

                case QuestionType.FillInTheBlank:
                    // For fill in the blank questions, you might need a different way to check answers
                    // For example, you could use string comparison instead of indices
                    break;

                case QuestionType.Sentence:
                    // Handle Sentance Type Question
                    break;
            }

            return -1;
        }
    
        private string GetSelectedAnswerText()
        {
            if (questions[currentQuestionIndex].type == QuestionType.FillInTheBlank)
            {
                // Assuming fillInTheBlankInputField is a TextField element in UI Toolkit
                return fillInTheBlankInputField.value.Trim(); 
            }
            return string.Empty;
        }
        public void OnContinueButtonPressed()
        {
            if (currentQuestionIndex < questions.Length - 1)
            {
                currentQuestionIndex++;
                DisplayQuestion(questions[currentQuestionIndex]);
            }
        }
    }
}