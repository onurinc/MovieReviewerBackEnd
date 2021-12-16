

using System.ComponentModel.DataAnnotations;

namespace MR.DataAccessLayer.Entities.DTOs.Incoming
{
    public class UserLoginRequestDto
    {
        [Required]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

    }
}
