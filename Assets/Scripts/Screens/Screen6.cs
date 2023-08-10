using DefaultNamespace;
using UnityEngine;
using UnityEngine.UI;

namespace Screens
{
    public class Screen6 : MonoBehaviour
    {
        [SerializeField] CanvasGroup canvasGroup;
        [SerializeField] private Button continueButton;

        void OnEnable()
        {
            EventManager.OnAssignmentCompleted.AddListener(OnAssigmentCompleted);
        }

        void OnDisable()
        {
            EventManager.OnAssignmentCompleted.RemoveListener(OnAssigmentCompleted);
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