namespace oblig.ViewModels;

public class CreateQuestionViewModel
{
    public int QuizId { get; set; }
    public string QuestionText { get; set; } = null!;
    public List<string> Answers { get; set; } = new() { "", "", "", "" }; // fixed 4 answers
    public int CorrectAnswerIndex { get; set; } // 0-3
}