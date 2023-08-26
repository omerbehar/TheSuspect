using Screens.Bases;

namespace Screens
{
    public class Screen2_2 : ScreenBase
    {
        protected override void Start()
        {
            base.Start();
            NextButton.interactable = true;
        }
    }
}