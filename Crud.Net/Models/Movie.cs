using System.ComponentModel.DataAnnotations;
using System.Reflection;


namespace Crud.Net.Models
{
    public class Movie
    {
        
        public int Id { get; set; }

        [Required, MaxLength(250)]
        public string ? Name { get; set; }

        public int year { get; set; }

        [Required, MaxLength(2500)]
        public string? History { get; set; }
        public float Rate { get; set; }

        [Required]
        public byte[] ?Poster { get; set; } 

                         // define relationship between tables
        public int? GenreId { get; set; } // 

         public Genre? Genre { get; set; }
         

    }
}
