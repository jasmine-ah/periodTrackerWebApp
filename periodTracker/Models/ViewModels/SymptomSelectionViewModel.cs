using System;

namespace periodTracker.Models.ViewModels
{
    //public class SymptomViewModel
    //{
    //	public List<SymptomType>? SymptomTypes { get; set; }
    //       public Dictionary<int, int> SymptomTypeIds { get; set; }
    //       public string UserId { get; set; }
    //       public Dictionary<string, List<Subtype>> GroupedSubtypes { get; set; }
    //       public List<Severity>? Severities { get; set; }
    //       public List<int> SelectedSubtypes { get; set; }
    //       public Dictionary<int, int> SelectedSeverities { get; set; }

    //       //public IEnumerable<int> SelectedSeverities { get; internal set; }
    //       //public Dictionary<int, int> SelectedSeverities { get; set; } = new Dictionary<int, int>();
    //   }
    public class SymptomSelectionViewModel
    {
        public int SelectedSymptomTypeId { get; set; }
        public List<SymptomType> SymptomTypes { get; set; }
        public List<Subtype> Subtypes { get; set; }
        public List<Severity> Severities { get; set; }
    }

}

