using DefaultNamespace;
using Screens.Bases;
using UnityEngine;
using UnityEngine.UI;

namespace Screens
{
    public class Screen6 : ScreenBaseWithTimer 
    {

        void OnEnable()
        {
            EventManager.AssignmentCompleted.AddListener(OnAssigmentCompleted);
            EventManager.AssignmentNotCompleted.AddListener(OnAssignmentNotCompleted);
        }

        void OnDisable()
        {
            EventManager.AssignmentCompleted.RemoveListener(OnAssigmentCompleted);
            EventManager.AssignmentNotCompleted.RemoveListener(OnAssignmentNotCompleted);
        }

        void OnAssigmentCompleted()
        {
            NextButton.interactable = true;
        }
        void OnAssignmentNotCompleted()
        {
            NextButton.interactable = false;
        }
    }
}