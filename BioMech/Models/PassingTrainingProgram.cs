namespace BioMech.Models
{
    public partial class PassingTrainingProgram
    {
        public int? IdPassingTrainingProgram { get; set; }
        public bool? StatusStart { get; set; }
        public int? UserId { get; set; }
        public int? TrainingProgramId { get; set; }
    }
}
