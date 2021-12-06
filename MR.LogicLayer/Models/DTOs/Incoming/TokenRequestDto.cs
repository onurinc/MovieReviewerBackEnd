using System.ComponentModel.DataAnnotations;

namespace MR.LogicLayer.Models.DTOs.Incoming
{
    public class TokenRequestDto
    {
        [Required]
        public string Token { get; set; }

        [Required]
        public string RefreshToken { get; set; }
    }
}
