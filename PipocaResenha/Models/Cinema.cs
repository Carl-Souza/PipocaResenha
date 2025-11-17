using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PipocaResenha.Models
{
    public class Cinemas {
        [Key] public int Codigo { get; set; }
        public string Nome { get; set; }
        public string Cidade { get; set; }
        public string Endereco { get; set; }
    }

    public class FilmesCinemas {
        [Key] public int CodigoFilme { get; set; }
        [ForeignKey("CodigoFilme")] public Filmes Filmes { get; set; }
        [Key] public int CodigoCinema { get; set; }
        [ForeignKey("CodigoCinema")] public Cinemas Cinema { get; set; }
    }
}