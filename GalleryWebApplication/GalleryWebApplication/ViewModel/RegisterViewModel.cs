using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace GalleryWebApplication.ViewModel
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "You must enter your email!")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        //[Required(ErrorMessage = "You must enter your name!")]
        public string Name { get; set; }

        //[Required(ErrorMessage = "You must enter your username!")]
        //public string Username { get; set; }

        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [Required(ErrorMessage = "You must enter password!")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage = "You must confirm password!")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Passwords don't match!")]
        [DisplayName("Confirm password")]
        public string PasswordConfirm { get; set; }
    }
}
