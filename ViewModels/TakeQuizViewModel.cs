using oblig.Models;
using System.Collections.Generic;

namespace oblig.ViewModels
{
    public class TakeQuizViewModel
    {
        public int QuizId { get; set; }
        public int QuestionIndex { get; set; }
        public Question Question { get; set; } = null!;
        public int TotalQuestions { get; set; }
        public int CurrentScore { get; set; }

        public IEnumerable<Answer> Answers => Question.Answers;
    }
}
