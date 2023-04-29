using System.ComponentModel.DataAnnotations;

namespace Crud.Net.Models
{
    public class Genre
    {
        public int Id { get; set; }

        [Required]
        //Used ? to check attribute if nullable
        public string? Name { get; set; }
    }
}
