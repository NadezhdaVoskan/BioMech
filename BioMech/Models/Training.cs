using System;
using System.Collections.Generic;

namespace BioMech.Models
{
    public partial class Training
    {

        public int? IdTraining { get; set; }
        public string NameTraining { get; set; } = null!;
        public string DescriptionTraining { get; set; } = null!;
        public string LinkVideo { get; set; } = null!;
        public int? TrainingCategoryId { get; set; }
        public string? Photo_Training_Video { get; set; }
        public bool? DeletedTraining { get; set; }


    }
}
