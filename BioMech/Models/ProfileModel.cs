namespace BioMech.Models
{
    public class ProfileModel
    {
        public User? User { get; set; }
        public List<PhotoDiagnostic>? PhotoDiagnostic { get; set; }
        public Dictionary<int, string>? DiagnosticsProblems { get; internal set; }

        public string? CategoryName {  get; set; }
        public List<PassingTrainingProgram>? PassingTrainingPrograms { get; set; }
        public List<TrainingLog>? TrainingLogs { get; set; }

        public List<TrainingProgram>? TrainingProgram {  get; set; }
    }
}
