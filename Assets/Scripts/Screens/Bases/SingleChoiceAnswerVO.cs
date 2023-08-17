using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

namespace Screens.Bases
{
    public class SingleChoiceAnswerVO : VisualElement
    {
        public RadioButton radioButton;
        public Label answerLabel;
        
        
        public SingleChoiceAnswerVO()
        {
            VisualTreeAsset original = Resources.Load<VisualTreeAsset>($"UXML/SingleAnswerTemplate");
            Add(original.Instantiate());
            radioButton = this.Q<RadioButton>("RadioButton");
           var radioLable = this.Q<Label>();
           answerLabel = radioLable;

        }
        
        
    }
}