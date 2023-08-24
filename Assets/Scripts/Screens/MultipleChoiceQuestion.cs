using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace Screens
{[CreateAssetMenu(fileName = "New Multiple Choice Question", menuName = "Quiz/Multiple Choice Question")]
    public class MultipleChoiceQuestion : Question
    {
        public override VisualElement DisplayQuestionUI()
        {
            var questionUI = new VisualElement();
            var questionLabel = new Label(questionText);
            questionUI.Add(questionLabel);

            for (int i = 0; i < answers.Count; i++)
            {
                var checkbox = new Toggle { text = answers[i] };
                questionUI.Add(checkbox);
            }

            return questionUI;
        }

        public override bool CheckAnswer(int[] selectedAnswers)
        {
            // For multiple choice questions, check that all correct answers are selected and no incorrect ones are selected
            var selectedAnswerSet = new HashSet<int>(selectedAnswers);
            var correctAnswerSet = new HashSet<int>(correctAnswerIndices);

            return selectedAnswerSet.SetEquals(correctAnswerSet);
        }
        
        public int GetCorrectAnswerCount()
        {
            return correctAnswerIndices.Count;
        }
    }
}