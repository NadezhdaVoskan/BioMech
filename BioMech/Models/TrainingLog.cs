using System;
using System.Collections.Generic;

namespace BioMech.Models
{
    public partial class TrainingLog
    {
        public int? IdTrainingLog { get; set; }
        public DateTime? DateTrainingLog { get; set; }
        public string? DescriptionTrainingLog { get; set; } = null!;
        public bool? StatusDone { get; set; }
        public int? UserId { get; set; }
        public int? OneDayTrainingProgramId { get; set; }
        public int? Rating { get; set; }

    }
}
