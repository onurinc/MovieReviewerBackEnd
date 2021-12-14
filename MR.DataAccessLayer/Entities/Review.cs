using Microsoft.AspNetCore.Identity;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace MR.DataAccessLayer.Entities
{
    public class Review : BaseEntity
    {
        public string UserId { get; set; }

        public int MovieId { get; set; }

        public string Body { get; set; }

        public decimal Rating { get; set; }

        [ForeignKey(nameof(UserId))]
        public IdentityUser User { get; set; }

    }
}
