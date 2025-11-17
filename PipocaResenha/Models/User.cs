using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PipocaResenha.Models
{
    public class User
    {
        public int Id { get; set; }
        [Required] public string Name { get; set; }
        [Required, EmailAddress] public string Email { get; set; }
        [Required] public string PasswordHash { get; set; }
        public string PhotoUrl { get; set; }
        public ICollection<Review> Reviews { get; set; }
    }
}