using System.ComponentModel.DataAnnotations;

namespace oblig.ViewModels
{
    public class CreateQuizViewModel
    {
        [Required(ErrorMessage = "Title is required.")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Description is required.")]
        public string Description { get; set; } = string.Empty;
    }
}
