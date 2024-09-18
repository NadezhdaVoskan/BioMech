namespace BioMech.Models
{
    public class TrainingsModel
    {
        public Training? Training { get; set; }

        public List<TrainingCategory>? TrainingCategories { get; set; } = new List<TrainingCategory>();

        public List<Training>? TrainingsList { get; set; }

        public List<TrainingProgram>? TrainingsProgramsList { get; set; }
    }
}
