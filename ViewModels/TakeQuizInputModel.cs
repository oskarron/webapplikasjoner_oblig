namespace oblig.ViewModels;

public class TakeQuizInputModel
{
    public int QuizId { get; set; }
    public int QuestionId { get; set; }
    public int SelectedAnswerId { get; set; }
    public int QuestionIndex { get; set; }
}
