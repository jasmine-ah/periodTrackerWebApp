using System;
namespace periodTracker.Models.ViewModels
{
	public class AdminDashboardViewModel
	{
		public int TotalUsers { get; set; }
		public int TotalMenstruations { get; set; }
		public double PercentageUsers20to35 { get; set; }
	}
}

