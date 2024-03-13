using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace periodTracker.Models
{
	public class Subtype
	{
		[Key]
		public int Id { get; set; }
		public string? Name { get; set; }
		//public int SeverityId { get; set; }
		public Severity? Severity { get; set; }
		[ForeignKey("SymptomTypeId")]
		public int SymptomTypeId { get; set; }
		public virtual SymptomType? SymptomType { get; set; }

	}
}

