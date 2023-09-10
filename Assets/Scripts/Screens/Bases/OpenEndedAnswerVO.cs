using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace Screens.Bases
{
    public class OpenEndedAnswerVO : VisualElement
    {
        public TextField answerInputField;
        public Action<string> onSubmit;

        public OpenEndedAnswerVO()
        {
            VisualTreeAsset original = Resources.Load<VisualTreeAsset>("UXML/OpenEndedAnswer");
            Add(original.Instantiate());
            answerInputField = this.Q<TextField>();

            // Reverse the string for Hebrew or other RTL languages
            string hebrewText = "נסיון טקסט"; // Replace with your text
            string reversedText = ReverseHebrewText(hebrewText);

            answerInputField.label = reversedText;
            answerInputField.style.unityTextAlign = UnityEngine.TextAnchor.MiddleRight;

            answerInputField.RegisterValueChangedCallback(evt => 
            {
                var originalText = ReverseHebrewText(evt.newValue);
                onSubmit?.Invoke(originalText);
                //Debug.Log("Open-ended answer received: " + originalText);
            });
        }

        private string ReverseHebrewText(string text)
        {
            // Text reversal logic for Hebrew or RTL languages
            var reversedTextArray = text.ToCharArray();
            Array.Reverse(reversedTextArray);
            return new string(reversedTextArray);
        }
    }
}