using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GymManagement.Areas.Admin.Models
{
    public class CreateSessionViewModel
    {
        public DateTime SessionDateTime { get; set; }
        public int GymClassId { get; set; }
        public string TrainerId { get; set; }
        public int RoomId { get; set; }
        public int Capacity { get; set; }

        public string BranchName { get; set; } = string.Empty;

        public List<SelectListItem> GymClassList { get; set; } = new();
        public List<SelectListItem> TrainerList { get; set; } = new();
        public List<SelectListItem> RoomList { get; set; } = new();
    }
}
