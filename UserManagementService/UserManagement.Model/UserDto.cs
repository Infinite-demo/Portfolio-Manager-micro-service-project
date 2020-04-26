using System.ComponentModel.DataAnnotations;

namespace UserManagement.Model
{
    public class UserDto
    {
        [Required(ErrorMessage = "{0} is required")]
        public string Name { get; set; }
        [Required(ErrorMessage = "{0} is required")]
        public string Password { get; set; }
    }
}
