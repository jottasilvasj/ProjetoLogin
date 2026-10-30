using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.ComponentModel.DataAnnotations;

namespace ProjetoLogin.Models
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Informe o Email")]
        [EmailAddress(ErrorMessage = "Email Invàlido")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Informe a Senha")]
        [DataType(DataType.Password)]
        public string Senha { get; set; } = string.Empty;

    }
}
