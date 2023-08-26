using DefaultNamespace;
using Screens.Bases;
using UnityEngine;
using UnityEngine.UI;

namespace Screens
{
    public class Screen6 : ScreenBase 
    {
        [SerializeField] CanvasGroup canvasGroup;
        [SerializeField] private Button continueButton;

        void OnEnable()
        {
            EventManager.AssignmentCompleted.AddListener(OnAssigmentCompleted);
        }

        void OnDisable()
        {
            EventManager.AssignmentCompleted.RemoveListener(OnAssigmentCompleted);
        }

        void OnAssigmentCompleted()
        {
            // Change the alpha of the CanvasGroup to desired value
            // As an example, this line will make it fully opaque.
            canvasGroup.alpha = 1f;
            continueButton.interactable = true;
        }
    }
}