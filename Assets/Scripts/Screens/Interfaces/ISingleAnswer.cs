using UnityEngine;

namespace Screens.Interfaces
{
    public interface ISingleAnswer
    {
        GameObject CorrectAnswer { get; set; }
        int PotentialScore { get; set; }
        int CorrectAnswerScore { get; set; }
        bool[] SelectedAnswers { get; set; }
        void OnAnswer();
        void OnSelect(GameObject selectedGameObject);
    }
}