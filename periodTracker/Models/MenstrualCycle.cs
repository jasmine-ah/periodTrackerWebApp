using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace periodTracker.Models
{
    public class MenstrualCycle
    {
        [Key]
        public int Id { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
       
       public int CycleLength { get; set; }
       public int PeriodLength { get; set; }

        [ForeignKey("UserId")]
        public User? User { get; set; }
        public string? UserId { get; set; }


        //public DateTime NextPeriodPrediction { get; set; }
    }
}



