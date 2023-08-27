using Screens.Bases;

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
            answerScore = scoreIfCorrect;
        }
        void OnAssignmentNotCompleted()
        {
            NextButton.interactable = false;
            answerScore = 0;
        }
    }
}