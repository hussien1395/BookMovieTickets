using System.ComponentModel.DataAnnotations;
using BookMovieTickets.Validations;

namespace BookMovieTickets.ViewModel
{
    public class CreateCategoryVM
    {
        [Required(ErrorMessage = "هذا الحقل مطلوب")]
        [MinLength(3)]
        [MaxLength(20)]
        public string Name { get; set; }

        [AllowedExtention(new[] { ".png", ".jpg", ".jpeg", ".gif" })]
        public IFormFile FormImg { get; set; }
    }
}
