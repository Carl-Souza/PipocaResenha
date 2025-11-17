using System;
using System.ComponentModel.DataAnnotations;

namespace PipocaResenha.Models
{
    public class Review
    {
        public int Codigo { get; set; }
        [Required] public int CodigoFilme { get; set; }
        public Filmes Filme { get; set; }
        [Required] public int CodigoUsuario { get; set; }
        public Usuarios Usuario { get; set; }
        [Range(0, 10)] public byte Nota { get; set; }
        public string Text { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}