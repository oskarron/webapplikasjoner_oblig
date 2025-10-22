using System.ComponentModel.DataAnnotations;
namespace oblig.ViewModels;

public class ValidateAnswersAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        var answers = value as List<string>;
        if (answers == null || answers.Any(string.IsNullOrWhiteSpace))
            return new ValidationResult("All answer fields must be filled in.");

        return ValidationResult.Success;
    }
}

public class CreateQuestionViewModel
{
    [Required]
    public string QuestionText { get; set; } = string.Empty;

    [ValidateAnswers]
    public List<string> Answers { get; set; } = new List<string> { "", "", "", "" };

    public int CorrectAnswerIndex { get; set; }
    public int QuizId { get; set; }
}
