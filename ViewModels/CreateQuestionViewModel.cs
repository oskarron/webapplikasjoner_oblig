using System.ComponentModel.DataAnnotations;

namespace oblig.ViewModels;

public class CreateQuestionViewModel
{
    [Required]
    public int QuizId { get; set; }

    [Required(ErrorMessage = "Question text is required.")]
    public string QuestionText { get; set; } = string.Empty;


    [Required(ErrorMessage = "Please provide answers.")]
    [MinLength(4, ErrorMessage = "You must provide 4 answers.")]
    [MaxLength(4, ErrorMessage = "You must provide 4 answers.")]
    public List<string> Answers { get; set; } = new List<string> { "", "", "", "" };

    [Range(0, 3, ErrorMessage = "Select which answer is correct.")]
    public int CorrectAnswerIndex { get; set; }
}
