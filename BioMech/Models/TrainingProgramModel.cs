namespace BioMech.Models
{
    public class TrainingProgramModel
    {
        public TrainingProgram TrainingProgram { get; set; }
        public List<OneDayTrainingProgram>? OneDayTrainingProgram { get; set; }
        public List<DailyTraining>? DailyTraining { get; set; }
        public List<Training>? Training { get; set; }
    }
}
