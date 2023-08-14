using DefaultNamespace;

namespace Screens.Interfaces
{
    public interface IQuestion
    {
        string QuestionText { get; set; }
        QuestionType Type { get; set; }

        void Display();
        bool CheckAnswer(int answerIndex);
    }
}