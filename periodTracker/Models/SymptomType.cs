using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace periodTracker.Models
{
	public class SymptomType
	{
		[Key]
		public int Id { get; set; }
		public string? Name { get; set; }
		public virtual ICollection<Subtype>? Subtypes { get; set; }
		//public Severity? Severity { get; set; }
		
	}
}

