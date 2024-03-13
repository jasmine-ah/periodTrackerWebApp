using System;
namespace periodTracker.Models.ViewModels
{
    public class MenstrualCycleViewModel
    {
        public MenstrualCycle? CurrentCycle { get; set; }
        public string? UserName { get; set; } // Name of the user associated with the cycle
        public double? AverageCycleLength { get; set; }
        public List<MenstrualCycle>? PreviousCycles { get; set; }
        public DateTime? OvulationDay { get; set; }
        public List<DateTime>? FertileWindow { get; set; }
        public int? DaysLeftForNextPeriod { get; set; }
        public List<MenstrualCycle>? FutureCycles { get; set; }
        public DateTime? PredictedNextPeriod { get; set; }
        public string? UserId { get; set; }

        //public bool ReminderSent { get; set; }

        // Additional properties for the view
        //public bool IsEditMode { get; set; }
        //public string Message { get; set; }
    }

}

