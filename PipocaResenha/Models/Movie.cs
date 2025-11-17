using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PipocaResenha.Models
{
    public class Movie
    {
        public int Id { get; set; }
        [Required] public string Title { get; set; }
        public string ShortSynopsis { get; set; }
        public string Synopsis { get; set; }
        public string AgeRating { get; set; }
        public string PosterUrl { get; set; }
        public DateTime? ReleaseDate { get; set; }
        public bool IsNowShowing { get; set; } = true;
        public string ExternalTicketUrl { get; set; }
        public ICollection<Review> Reviews { get; set; }
        public ICollection<MovieCinema> MovieCinemas { get; set; }
    }
}