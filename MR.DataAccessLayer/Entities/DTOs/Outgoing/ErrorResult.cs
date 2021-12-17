using System.Collections.Generic;

namespace MR.DataAccessLayer.Entities.DTOs.Outgoing
{
    public class ErrorResult
    {

        public bool Succes { get; set; }

        public List<string> Errors { get; set; }
    }
}