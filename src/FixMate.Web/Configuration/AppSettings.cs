using System.Collections.Generic;

namespace FixMate.Web.Configuration
{
    public class AppSettings
    {
        public ConnectionStrings ConnectionStrings { get; set; }
        public JwtSettings Jwt { get; set; }
        public SerilogSettings Serilog { get; set; }
        public EmailSettings EmailSettings { get; set; }
        public CorsSettings Cors { get; set; }
    }

    public class ConnectionStrings
    {
        public string DefaultConnection { get; set; }
    }

    public class JwtSettings
    {
        public string Key { get; set; }
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public int ExpiryInDays { get; set; }
    }

    public class SerilogSettings
    {
        public MinimumLevel MinimumLevel { get; set; }
        public List<WriteTo> WriteTo { get; set; }
    }

    public class MinimumLevel
    {
        public string Default { get; set; }
        public Override Override { get; set; }
    }

    public class Override
    {
        public string Microsoft { get; set; }
        public string System { get; set; }
    }

    public class WriteTo
    {
        public string Name { get; set; }
        public WriteToArgs Args { get; set; }
    }

    public class WriteToArgs
    {
        public string Path { get; set; }
        public string RollingInterval { get; set; }
    }

    public class EmailSettings
    {
        public string SmtpServer { get; set; }
        public int Port { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string FromEmail { get; set; }
        public string FromName { get; set; }
    }

    public class CorsSettings
    {
        public List<string> AllowedOrigins { get; set; }
    }
} 