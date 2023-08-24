using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace Screens.Bases
{
    public class MultiChoiceAnswerVO : VisualElement
    {
        public Toggle checkbox;
        public Label answerLabel;
        public Action<int> onAnswerMultipleChoiceSelected;
        public int OptionIndex { get; set; }

        public MultiChoiceAnswerVO()
        {
            VisualTreeAsset original = Resources.Load<VisualTreeAsset>("UXML/MultiAnswerTemplate");
            Add(original.Instantiate());
            checkbox = this.Q<Toggle>();

            checkbox.RegisterValueChangedCallback(evt =>
            {
                onAnswerMultipleChoiceSelected?.Invoke(OptionIndex);
                Debug.Log((evt.newValue ? "Selected" : "Deselected") + " Checkbox with index: " + OptionIndex);
            });

            answerLabel = this.Q<Label>();
        }
    }
}