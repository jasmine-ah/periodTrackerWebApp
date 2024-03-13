using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace periodTracker.Models
{
	public class MenstrualProfile
	{
		[Key]
		public int Id { get; set; }
		[ForeignKey("UserId")]
		public User? User { get; set; }
		public string? UserId { get; set; }
		public int CycleLength { get; set; }
		public int PeriodLength { get; set; }
		public double Weight { get; set; }
		public double Height { get; set; }
	}
}

