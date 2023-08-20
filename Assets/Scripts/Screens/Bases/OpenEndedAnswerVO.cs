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

            answerInputField.RegisterValueChangedCallback(evt => 
            {
                onSubmit?.Invoke(evt.newValue);
                Debug.Log("Open-ended answer received: " + evt.newValue);
            });
        }
    }
}