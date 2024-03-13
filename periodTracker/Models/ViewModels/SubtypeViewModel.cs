using System;
namespace periodTracker.Models.ViewModels
{
	public class SubtypeViewModel
	{
        public int SubtypeId { get; set; }
        public string SubtypeName { get; set; }
        public int SeverityId { get; set; }
        public string? SeverityLevel { get; set; }
        public string? UserId { get; set; }
        public bool IsSelected { get; set; }
    }
}

