﻿using MR.DataAccessLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MR.DataAccessLayer.Interfaces
{
    public interface IUserRepository : IGenericRepository<User>
    {
        Task<bool> UpdateUserProfile(User user);

        Task<User> GetUserByIdentityId(Guid identityId);

    }
}
