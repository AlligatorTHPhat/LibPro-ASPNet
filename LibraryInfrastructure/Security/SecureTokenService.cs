using LibraryApplication.Interfaces;
using Microsoft.Extensions.Configuration;
using System.Text;

namespace LibraryInfrastructure.Security
{
    public class SecureTokenService : ISecureTokenService
    {
        private readonly IConfiguration _config;

        private readonly string _secureKey;
        public SecureTokenService(IConfiguration config)
        {
            _secureKey = config["SecureToken:Key"] ?? "DefaultFallbackKey123";
            _config = config;
        }
        public string GenerateEBookToken(Guid readerId, string filePath, DateTime dueDate)
        {
            var rawData = $"{readerId}|{filePath}|{dueDate:O}";
            var plainTextBytes = Encoding.UTF8.GetBytes(rawData);
            return Convert.ToBase64String(plainTextBytes);
        }

        public bool ValidateEBookToken(string token, Guid readerId, out string filePath)
        {
            filePath = string.Empty;
            try
            {
                var base64EncodedBytes = Convert.FromBase64String(token);
                var decodedString = Encoding.UTF8.GetString(base64EncodedBytes);
                var parts = decodedString.Split('|');

                if (parts.Length != 3) return false;

                var tokenReaderId = Guid.Parse(parts[0]);
                var tokenFilePath = parts[1];
                var expiryDate = DateTime.Parse(parts[2]);

                // Kiểm tra xem token có đúng chủ không và còn hạn không
                if (tokenReaderId == readerId && expiryDate > DateTime.Now)
                {
                    filePath = tokenFilePath;
                    return true;
                }
            }
            catch { return false; }
            return false;
        }
    }
}