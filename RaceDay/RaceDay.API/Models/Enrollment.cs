
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RaceDay.API.Models
{
    [Table("Enrollment")]
    public class Enrollment
    {
        [Key]
        public int EnrollmentID { get; set; }

        [Required]
        public int ParticipantID { get; set; }

        [Required]
        public int EventID { get; set; }

        [Required]
        public int CategoryID { get; set; }

        public DateTime EnrollmentDate { get; set; } = DateTime.Now;

        [ForeignKey(nameof(ParticipantID))]
        public Participant Participant { get; set; } = null!;

        [ForeignKey(nameof(EventID))]
        public Event Event { get; set; } = null!;

        [ForeignKey(nameof(CategoryID))]
        public Category Category { get; set; } = null!;
    }
}
