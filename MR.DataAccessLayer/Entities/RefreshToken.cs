using Microsoft.AspNetCore.Identity;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace MR.DataAccessLayer.Entities
{
    public class RefreshToken : BaseEntity
    {
        public string UserId { get; set; }

        public string Token { get; set; }

        public string JwtId { get; set; }
        public string IsUsed { get; set; }
        public string IsRevoked { get; set; }

        public DateTime ExpiryDate { get; set; }

        [ForeignKey(nameof(UserId))]
        public IdentityUser User { get; set; }




    }
}