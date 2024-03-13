using System;
using System.ComponentModel.DataAnnotations;

namespace periodTracker.Models
{
    public class Symptom
    {
        [Key]
        public int Id { get; set; }
        public string? Name { get; set; }
        public int Severity { get; set; }
        public DateTime Timestamp { get; set; }
    }

}

