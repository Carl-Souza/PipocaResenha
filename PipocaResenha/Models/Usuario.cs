using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PipocaResenha.Models
{
    public class Usuarios
    {
        public int Codigo { get; set; }
        [Required] public string Nome { get; set; }
        [Required, EmailAddress] public string Email { get; set; }
        [Required] public string PasswordHash { get; set; }
        public string PhotoUrl { get; set; }
        public ICollection<Review> Reviews { get; set; }
    }
}