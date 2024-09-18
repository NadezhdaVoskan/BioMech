using System;
using System.Collections.Generic;

namespace BioMech.Models
{
    public partial class User
    {
        public int? IdUser { get; set; }
        public string? FirstName { get; set; } = null!;
        public string? SecondName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string? SaltPassword { get; set; } = null!;
        public string? PhotoProfile { get; set; }
        public int? RoleId { get; set; }
        public string? TokenForRecoveryPassword { get; set; } = null!;
        public DateTime? TimeGenerateTokenForRecoveryPassword { get; set; } = null!;
        public bool? DeletedUser { get; set; }

    }
}
