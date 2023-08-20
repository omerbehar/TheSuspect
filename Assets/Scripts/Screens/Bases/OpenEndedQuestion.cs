using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace Screens.Bases
{
    [CreateAssetMenu(fileName = "New Open Ended Question", menuName = "Quiz/Open Ended Question")]
    public class OpenEndedQuestion : Question
    {
        [SerializeField]
        private string correctAnswer; // This is the correct answer for open-ended question.

        public override bool CheckAnswer(int[] selectedAnswers)
        {
            // This method is irrelevant for open-ended questions
            // as we don't select answers by indices.
            throw new NotImplementedException();
        }

        public bool CheckAnswer(string userAnswer)
        {
            // Basic string comparison - you might want to extend this
            // to ignore case, leading/trailing spaces, etc.
            return userAnswer == correctAnswer;
        }

        public override VisualElement DisplayQuestionUI()
        {
            // Load the asset
            VisualTreeAsset original = GetLayoutAsset();
            // Clone the tree
            VisualElement visualElement = original.CloneTree();
        
            // Wire up your UI - set question text, set up answer input field, etc.
       
            return visualElement;
        }
    }
}