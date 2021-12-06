using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MR.LogicLayer.Models.DTOs.Outgoing
{
    public class AuthResult
    {
        public string Token { get; set; }

        public string RefreshToken { get; set; }

        public bool Succes { get; set; }

        public List<string> Errors { get; set; }
    }
}
