using System.Runtime.InteropServices;
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
        public static void OpenURL(string url)
        {
            #if UNITY_WEBGL && !UNITY_EDITOR
                OpenTab(url);
            #endif 
        }
        
        [DllImport("__Internal")]
        private static extern void OpenTab(string url);
    }
}