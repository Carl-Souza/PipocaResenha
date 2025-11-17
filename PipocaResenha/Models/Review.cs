using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PipocaResenha.Models
{
    public class Review
    {
        [Key] public int Codigo { get; set; }
        [Required] public int CodigoFilme { get; set; }
        [ForeignKey("CodigoFilme")] public Filmes Filmes { get; set; }
        [Required] public int CodigoUsuario { get; set; }
        [ForeignKey("CodigoUsuario")] public Usuarios Usuario { get; set; }
        [Range(0, 10)] public byte Nota { get; set; }
        public string TextoReview { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}