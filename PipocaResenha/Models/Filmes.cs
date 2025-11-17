using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PipocaResenha.Models
{
    public class Filmes
    {
        [Key] public int Codigo { get; set; }
        [Required] public string Titulo { get; set; }
        public string SinopseCurta { get; set; }
        public string Sinopse { get; set; }
        public string FaixaEtaria { get; set; }
        public string PosterUrl { get; set; }
        public DateTime? DataLancamento { get; set; }
        public bool EmCartaz { get; set; } = true;
        public string UrlIngreso { get; set; }
        public ICollection<Review> Reviews { get; set; }
        public ICollection<FilmesCinema> FilmesCinemas { get; set; }
    }
}