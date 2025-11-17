namespace PipocaResenha.Models
{
    public class Cinemas {
        public int Codigo { get; set; }
        public string Nome { get; set; }
        public string Cidade { get; set; }
        public string Endereco { get; set; }
    }

    public class FilmesCinema {
        public int CodigoFilme { get; set; }
        public Filmes Filme { get; set; }
        public int CodigoCinema { get; set; }
        public Cinemas Cinema { get; set; }
    }
}