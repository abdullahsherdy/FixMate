using System.Collections.Generic;

namespace FixMate.Application.Configuration
{
    public class AppSettings
    {
        public JwtSettings Jwt { get; set; }
        public ConnectionStrings ConnectionStrings { get; set; }
        public CorsSettings Cors { get; set; }
    }

    public class JwtSettings
    {
        public string Key { get; set; }
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public int ExpiryInDays { get; set; }
    }

    public class ConnectionStrings
    {
        public string DefaultConnection { get; set; }
    }

    public class CorsSettings
    {
        public List<string> AllowedOrigins { get; set; }
    }
} 