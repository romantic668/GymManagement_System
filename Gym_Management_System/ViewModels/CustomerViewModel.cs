// CustomerViewModel.cs
namespace GymManagement.ViewModels
{
  public class CustomerViewModel
  {
    public required int CustomerId { get; set; }
    public string Name { get; set; } = string.Empty;
  }
}
