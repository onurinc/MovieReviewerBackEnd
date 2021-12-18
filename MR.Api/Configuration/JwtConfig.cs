using System;

namespace MR.Api.Configuration
{
    public class JwtConfig
    {
        public string Secret { get; set; }

        public TimeSpan ExpiryTimeFrame { get; set; }
    }
}
