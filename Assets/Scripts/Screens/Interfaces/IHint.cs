using UnityEngine;
using UnityEngine.UI;

namespace Screens.Interfaces
{
    public interface IHint
    {
        Button HintButton { get; set; }
        GameObject HintPopup { get; set; }
        Button HintPopupCloseButton { get; set; }
        void OnHintButtonClicked();
    }
}