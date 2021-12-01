 using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MR.LogicLayer.Models
{
    public class CommentModel
    {
        public int CommentId { get; set; }

        public int UserId { get; set; }

        public int MovieId { get; set; }

        public string Body { get; set; }
    }
}
