using System;
namespace periodTracker.Models.ViewModels
{
	public class SymptomViewModel
	{
		public int SymptomTypeId { get; set; }
		public string SymptomTypeName { get; set; }
		
		public List<SubtypeViewModel> Subtypes { get; set; }
	}
}

