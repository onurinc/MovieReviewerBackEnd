using System.ComponentModel.DataAnnotations;


namespace MR.LogicLayer.Models.DTOs.Generic
{
    public class TokenData
    {
        [Required]
        public string JwtToken { get; set; }

        [Required]
        public string RefreshToken { get; set; }
    }
}
