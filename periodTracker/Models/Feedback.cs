using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace periodTracker.Models
{
    public class Feedback
    {


        [Key]
        public int Id { get; set; }
        [ForeignKey("UserId")]
        public User? User { get; set; }
        public string? UserId { get; set; }
        public string? userName { get; set; }
        public string? Message { get; set; }
        public DateTime SubmissionTime { get; set; }
    }
}


