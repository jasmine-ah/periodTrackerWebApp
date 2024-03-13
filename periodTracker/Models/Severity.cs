using System;
using System.ComponentModel.DataAnnotations;

namespace periodTracker.Models
{
	public class Severity
	{
		[Key]
		public int Id { get; set; }
		public string? Level { get; set; }
	}
}

