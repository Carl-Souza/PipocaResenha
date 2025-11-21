// Pasta: ViewModels
// Arquivo: HomeViewModel.cs
using PipocaResenha.Models;

namespace PipocaResenha.ViewModels
{
    public class HomeViewModel
    {
        public IEnumerable<Filmes> Lancamentos { get; set; }
        public IEnumerable<Filmes> TopBemAvaliados { get; set; } // Substitui "Em Destaque"
        public IEnumerable<Filmes> MaisComentados { get; set; } // Substitui "Mais Assistidos"
    }
}