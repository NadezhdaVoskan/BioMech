using System;
using System.Collections.Generic;

namespace BioMech.Models
{
    public partial class ProblemCategory
    {
        public int IdProblemCategory { get; set; }
        public string NameProblemCategory { get; set; } = null!;
        public string? DescriptionProblemCategory { get; set; } = null!;
        public string? RecommendationCorrection { get; set; } = null!;
      
    }
}
