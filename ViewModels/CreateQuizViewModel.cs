namespace oblig.ViewModels
{
    public class CreateQuizViewModel
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public List<QuestionInputModel> Questions { get; set; } = new();
    }

    public class QuestionInputModel
    {
        public string Text { get; set; }
        public List<AnswerInputModel> Answers { get; set; } = new();
    }

    public class AnswerInputModel
    {
        public string Text { get; set; }
        public bool IsCorrect { get; set; }
    }
}
