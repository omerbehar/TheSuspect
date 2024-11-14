using UnityEngine;
using UnityEngine.EventSystems;

namespace DefaultNamespace
{
    public class clickedInput : MonoBehaviour, IPointerClickHandler
    {
        private static readonly int KeyboardIn = Animator.StringToHash("keyboardIn");
        [SerializeField] private Animator animator;
        public void OnPointerClick(PointerEventData eventData)
        {
            animator.SetBool(KeyboardIn, true);
        }
    }
}