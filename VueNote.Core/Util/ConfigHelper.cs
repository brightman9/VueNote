using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Extensions.Configuration;

namespace VueNote.Core.Util
{
    public static class ConfigHelper
    {
        public static string ConnectionString { get; private set; }
        public static IList<string> AllowOrigins { get; private set; }
        public static JwtSettings JwtSettings { get; private set; }


        public static void Init(IConfiguration configuration)
        {
            ConnectionString = configuration.GetValue<string>("ConnectionString");
            AllowOrigins = configuration.GetSection("AllowOrigins").GetChildren().Select(t => t.Value).ToList();
            JwtSettings = new JwtSettings
            {
                Issuer = configuration.GetValue<string>("JwtIssuer"),
                Audience = configuration.GetValue<string>("JwtAudience"),
                SigningKey = configuration.GetValue<string>("JwtSigningKey"),
            };
        }
    }

    public class JwtSettings
    {
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public string SigningKey { get; set; }
    }
}
