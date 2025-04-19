// ViewModels/PagedSessionViewModel.cs
using GymManagement.Models;
using GymManagement.ViewModels;
using System.Collections.Generic;

public class PagedSessionViewModel
{
    public List<SessionViewModel> Sessions { get; set; } = new ();
    public int CurrentPage { get; set; }
    public int TotalPages { get; set; }
   
}
