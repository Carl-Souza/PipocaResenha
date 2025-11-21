
using PipocaResenha.Models;

namespace PipocaResenha.ViewModels
{
    public class HomeViewModel
    {
        public IEnumerable<Filmes> Lancamentos { get; set; }
        public IEnumerable<Filmes> TopBemAvaliados { get; set; } 
        public IEnumerable<Filmes> MaisComentados { get; set; } 
    }
}