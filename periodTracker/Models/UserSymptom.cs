using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace periodTracker.Models
{
    public class UserSymptom
    {
        [Key]
        public int Id { get; set; }

        //[ForeignKey("SymptomTypeId")]
        //public int SymptomTypeId { get; set; }
        //public SymptomType? SymptomType { get; set; }

        [ForeignKey("SubTypeId")]
        public int SubTypeId { get; set; }
        public Subtype? Subtype { get; set; }

        [ForeignKey("SeverityId")]
        public int SeverityId { get; set; }
        public Severity? Severities { get; set; } 

        [ForeignKey("UserId")]
        public string UserId { get; set; } // Foreign key to the User table
        public User? User { get; set; } // Navigation property
        public DateTime Timestamp { get; set; }
    }

}

