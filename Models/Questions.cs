namespace oblig.Models;

public class Question
{
    public int Id { get; set; }
    public string Text { get; set; } = string.Empty;

    public int QuizId { get; set; }
    public Quiz? Quiz { get; set; }

    public List<Answer> Answers { get; set; } = new List<Answer>();
}
