using System.IO;
using System.Collections.Generic;
using System.Text.Json;

namespace Planora.BLL.Services
{
    public interface ICredentialService
    {
        (string Login, bool RememberMe) LoadCredentials();
        void SaveCredentials(string login, bool rememberMe);
        void ClearCredentials();
    }

    public class FileCredentialService : ICredentialService
    {
        private const string CredentialsPath = "credentials.json";

        public (string Login, bool RememberMe) LoadCredentials()
        {
            if (!File.Exists(CredentialsPath)) return (string.Empty, false);

            try
            {
                var json = File.ReadAllText(CredentialsPath);
                var data = JsonSerializer.Deserialize<Dictionary<string, string>>(json);
                if (data != null && data.ContainsKey("Login"))
                {
                    return (data["Login"], true);
                }
            }
            catch 
            { 
            }
            return (string.Empty, false);
        }

        public void SaveCredentials(string login, bool rememberMe)
        {
            try
            {
                if (rememberMe)
                {
                    var data = new Dictionary<string, string> { { "Login", login } };
                    var json = JsonSerializer.Serialize(data);
                    File.WriteAllText(CredentialsPath, json);
                }
                else
                {
                    ClearCredentials();
                }
            }
            catch {  }
        }

        public void ClearCredentials()
        {
            if (File.Exists(CredentialsPath)) File.Delete(CredentialsPath);
        }
    }
}