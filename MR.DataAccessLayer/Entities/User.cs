using System;
using System.ComponentModel.DataAnnotations;

namespace MR.DataAccessLayer.Entities
{
    public class User : BaseEntity
    {
        public Guid IdentityId { get; set; }

        public string FirstName { get; set; }

        public string MiddleName{ get; set; }

        public string LastName { get; set; }

        public string Country { get; set; }

        public string Email { get; set; }

    }
}
