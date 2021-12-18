using Microsoft.AspNetCore.Identity;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace MR.DataAccessLayer.Entities
{
    public class Review
    {
        public Guid ReviewId { get; set; }

        public Guid UserId { get; set; }

        public int MovieId { get; set; }

        public string Body { get; set; }

        public decimal Rating { get; set; }


    }
}
