namespace OdeToFood.Core
{
    using System.ComponentModel.DataAnnotations;

    public class Restaurant
    {
        [Required]
        [MinLength(3)]
        [MaxLength(80)]
        public string Name { get; set; }

        [Required]
        [MinLength(3)]
        [MaxLength(255)]
        public string Location { get; set; }

        public int Id { get; set; }

        public CuisineType Cuisine { get; set; }
    }
}