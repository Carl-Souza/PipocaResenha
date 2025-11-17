namespace PipocaResenha.Models
{
    public class Cinema {
        public int Id { get; set; }
        public string Name { get; set; }
        public string City { get; set; }
        public string Address { get; set; }
    }

    public class MovieCinema {
        public int MovieId { get; set; }
        public Movie Movie { get; set; }
        public int CinemaId { get; set; }
        public Cinema Cinema { get; set; }
    }
}