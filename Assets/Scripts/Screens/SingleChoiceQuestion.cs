using System;
using System.Collections.Generic;
using System.Linq;
using DefaultNamespace;
using UnityEngine;
using UnityEngine.UIElements;

namespace Screens
{
    [CreateAssetMenu(fileName = "New Single Choice Question", menuName = "Quiz/Single Choice Question")]
    public class SingleChoiceQuestion : Question
    {
        public override VisualElement DisplayQuestionUI()
        {
            var questionUI = new VisualElement();
            var questionLabel = new Label(questionText);
            questionUI.Add(questionLabel);

            for (int i = 0; i < answers.Count; i++)
            {
                var radioButton = new RadioButton { text = answers[i] };
                questionUI.Add(radioButton);
            }

            return questionUI;
        }

        public override bool CheckAnswer(int[] selectedAnswers)
        {
            // Assuming only one answer can be selected for single choice questions
            if (selectedAnswers.Length != 1) return false;

            return selectedAnswers[0] == correctAnswerIndices[0];
        }
    }
}