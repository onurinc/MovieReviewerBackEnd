using Microsoft.AspNetCore.Identity;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace MR.DataAccessLayer.Entities
{
    public class Comment
    {
        public Guid CommentId { get; set; }

        public Guid UserId { get; set; }

        public int MovieId { get; set; }

        public string Body { get; set; }

    }
}
