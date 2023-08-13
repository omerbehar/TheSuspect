using UnityEngine.UI;

namespace Screens.Interfaces
{
    public interface INextButton
    {
        public Button NextButton { get; set; }
        void OnNextButtonClicked();
    }
}