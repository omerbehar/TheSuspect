using UnityEngine.UI;

namespace Screens
{
    public interface IBackButton
    {
        Button BackButton { get; set; }
        void OnBackButtonClicked();
    }
}