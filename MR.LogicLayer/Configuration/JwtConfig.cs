using System;

namespace MR.LogicLayer.Configuration
{
    public class JwtConfig
    {
        public string Secret { get; set; }

        public TimeSpan ExpiryTimeFrame { get; set; }
    }
}
