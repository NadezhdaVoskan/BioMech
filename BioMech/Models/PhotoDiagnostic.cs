using System;
using System.Collections.Generic;

namespace BioMech.Models
{
    public partial class PhotoDiagnostic
    {
        public int? IdPhotoDiagnostic { get; set; }
        public string Photo { get; set; } = null!;
        public DateTime DateDownload { get; set; }
        public int? UserId { get; set; }
        public int? DiagnosticsCategoryId { get; set; }
        public int ProblemCategoryId { get; set; }

        public string? NameProblemCategory { get; set; }
    }
}