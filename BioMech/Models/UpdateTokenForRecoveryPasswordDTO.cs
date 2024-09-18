namespace BioMech.Models
{
    public class UpdateTokenForRecoveryPasswordDTO
    {
        public string? TokenForRecoveryPassword { get; set; } = null!;
        public DateTime? TimeGenerateTokenForRecoveryPassword { get; set; } = null!;
    }
}
