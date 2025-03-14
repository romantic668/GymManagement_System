// LibraryBranchViewModel.cs
namespace GymManagement.ViewModels
{
  public class LibraryBranchViewModel
  {
    public int LibraryBranchId { get; set; }
    public required string BranchName { get; set; } = string.Empty;
  }
}
