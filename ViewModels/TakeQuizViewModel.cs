using oblig.Models;
namespace oblig.ViewModels;

public class TakeQuizViewModel
{
    public int QuizId { get; set; }
    public int QuestionIndex { get; set; }
    public Question Question { get; set; }
    public int TotalQuestions { get; set; }
}
