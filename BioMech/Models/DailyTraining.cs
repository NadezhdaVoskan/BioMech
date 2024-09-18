using System;
using System.Collections.Generic;

namespace BioMech.Models
{
    public partial class DailyTraining
    {
        public int? IdDailyTraining { get; set; }
        public int VideoSerialNumber { get; set; }
        public int? OneDayTrainingProgramId { get; set; }
        public int? TrainingId { get; set; }

    }
}
