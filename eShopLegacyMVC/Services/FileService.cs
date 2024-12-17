using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Principal;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using Microsoft.Win32.SafeHandles;
using System.Threading.Tasks;

namespace eShopLegacyMVC.Services
{
    public class FileService
    {
        private const int LOGON32_PROVIDER_DEFAULT = 0;
        private const int LOGON32_LOGON_NEWCREDENTIALS = 9;

        private readonly FileServiceConfiguration configuration;

        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern bool LogonUser(string lpszUsername, string lpszDomain, string lpszPassword, int dwLogonType, int dwLogonProvider, out SafeAccessTokenHandle phToken);

        public FileService(FileServiceConfiguration configuration)
        {
            this.configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        public static FileService Create() =>
            new FileService(new FileServiceConfiguration
            {
                BasePath = ConfigurationManager.AppSettings["Files:BasePath"],
                ServiceAccountUsername = ConfigurationManager.AppSettings["Files:ServiceAccountUsername"],
                ServiceAccountDomain = ConfigurationManager.AppSettings["Files:ServiceAccountDomain"],
                ServiceAccountPassword = ConfigurationManager.AppSettings["Files:ServiceAccountPassword"]
            });

        public IEnumerable<string> ListFiles()
        {
            var authToken = string.IsNullOrEmpty(configuration.ServiceAccountUsername)
                ? WindowsIdentity.GetCurrent().AccessToken
                : GetAuthToken(configuration.ServiceAccountUsername, configuration.ServiceAccountDomain, configuration.ServiceAccountPassword);

            return WindowsIdentity.RunImpersonated(authToken, () =>
            {
                return Directory.GetFiles(configuration.BasePath).Select(Path.GetFileName);
            });
        }

        public byte[] DownloadFile(string filename)
        {
            var authToken = string.IsNullOrEmpty(configuration.ServiceAccountUsername)
                ? WindowsIdentity.GetCurrent().AccessToken
                : GetAuthToken(configuration.ServiceAccountUsername, configuration.ServiceAccountDomain, configuration.ServiceAccountPassword);

            return WindowsIdentity.RunImpersonated(authToken, () =>
            {
                var path = Path.Combine(configuration.BasePath, filename);
                return File.ReadAllBytes(path);
            });
        }

public async Task UploadFileAsync(List<IFormFile> files)
        {
            var authToken = string.IsNullOrEmpty(configuration.ServiceAccountUsername)
                ? WindowsIdentity.GetCurrent().AccessToken
                : GetAuthToken(configuration.ServiceAccountUsername, configuration.ServiceAccountDomain, configuration.ServiceAccountPassword);

            await WindowsIdentity.RunImpersonatedAsync(authToken, async () =>
            {
                for (var i = 0; i < files.Count; i++)
                {
                    var file = files[i];
                    var filename = Path.GetFileName(file.FileName);
                    var path = Path.Combine(configuration.BasePath, filename);

using (var fs = File.Create(path))
                    {
                        await file.CopyToAsync(fs);
                    }
                }
            });
        }

        private SafeAccessTokenHandle GetAuthToken(string username, string domain, string password)
        {
            if (!LogonUser(username, domain, password, LOGON32_LOGON_NEWCREDENTIALS, LOGON32_PROVIDER_DEFAULT, out SafeAccessTokenHandle authToken))
            {
                throw new InvalidOperationException($"Unable to get auth token for service account {username} in domain {domain}");
            }

            return authToken;
        }
    }
}