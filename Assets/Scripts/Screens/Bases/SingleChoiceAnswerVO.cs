using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

namespace Screens.Bases
{
    public class SingleChoiceAnswerVO : VisualElement
    {
        public RadioButton radioButton;
        public Label answerLabel;
        public Action<int> onAnswerClicked;  // A callback when an answer is clicked.
        public int OptionIndex { get; set; }  // Property to store the answer index.
    
        public SingleChoiceAnswerVO()
        {
            VisualTreeAsset original = Resources.Load<VisualTreeAsset>("UXML/SingleAnswerTemplate");
            Add(original.Instantiate());
            radioButton = this.Q<RadioButton>();

            radioButton.RegisterValueChangedCallback(evt => {
                if (evt.newValue) // If the radio button was checked
                {
                    onAnswerClicked?.Invoke(OptionIndex); 
                    //Debug.Log("A RadioButton was clicked, its index is: " + OptionIndex);
                }
            });

            var lable = this.Q<Label>();
            answerLabel = lable;
        } 
    }
}