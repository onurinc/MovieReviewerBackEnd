

namespace MR.DataAccessLayer.Entities
{
    public class Comment
    {
        public int CommentId { get; set; }

        public int UserId { get; set; }

        public int MovieId { get; set; }

        public string Body { get; set; }


    }
}
