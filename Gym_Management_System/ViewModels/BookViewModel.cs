// BookViewModel.cs
namespace GymManagement.ViewModels
{
  public class BookViewModel
  {
    public int BookId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string AuthorName { get; set; } = string.Empty;
    public string BranchName { get; set; } = string.Empty;
  }
}
