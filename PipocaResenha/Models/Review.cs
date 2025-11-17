using System;
using System.ComponentModel.DataAnnotations;

namespace PipocaResenha.Models
{
    public class Review
    {
        public int Id { get; set; }
        [Required] public int MovieId { get; set; }
        public Movie Movie { get; set; }
        [Required] public int UserId { get; set; }
        public User User { get; set; }
        [Range(0, 10)] public byte Rating { get; set; }
        public string Text { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}