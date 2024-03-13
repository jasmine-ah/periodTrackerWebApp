using System;
namespace periodTracker.Models
{
	public class Period
	{
		public int Id { get; set; }
		public int start_date { get; set; }
		public int end_date { get; set; }
		public string? symptoms { get; set; }
		public string? mood { get; set; }
		public int UserId { get; set; }
		public ICollection<User>? Users { get; set; }


	}
}

