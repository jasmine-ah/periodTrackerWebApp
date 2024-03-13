using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace periodTracker.Models.ViewModels
{
	public class ReportInfoViewModel
	{

		[Key]
		public int Id { get; set; }
		[ForeignKey("UserId")]
		public User? User { get; set; }
		public MenstrualCycleViewModel? MenstrualCycleViewModel { get; set; }
		public MenstrualProfile? menstrualProfile { get; set; }
		

	}
}

