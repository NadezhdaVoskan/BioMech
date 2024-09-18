using System;
using System.Collections.Generic;

namespace BioMech.Models
{
    public partial class TrainingProgram
    {
        public int? IdTrainingProgram { get; set; }
        public string NameTrainingProgram { get; set; } = null!;
        public string DescriptionTrainingProgram { get; set; } = null!;
        public int? DurationProgram { get; set; }
        public int? ProblemCategoryId { get; set; }

    }
}
