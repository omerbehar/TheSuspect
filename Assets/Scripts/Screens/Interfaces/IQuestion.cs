using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine.UIElements;

namespace Screens.Interfaces
{
    namespace Screens.Interfaces
    {
        public interface IQuestion
        {
            string QuestionText { get; }
            QuestionType Type { get; }
            VisualTreeAsset GetLayoutAsset();
            List<string> GetAnswerOptions();
            bool CheckAnswer(int[] selectedAnswers);
        }
    }
}