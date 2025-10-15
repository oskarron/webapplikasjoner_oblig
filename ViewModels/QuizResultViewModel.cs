using oblig.Models;
namespace oblig.ViewModels;

public class QuizResultViewModel
{
    public Quiz Quiz { get; set; }
    public int Score { get; set; }
    public int Total { get; set; }
}
