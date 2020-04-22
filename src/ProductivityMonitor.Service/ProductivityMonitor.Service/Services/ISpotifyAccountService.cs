using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProductivityMonitor.Service.Services
{
    public interface ISpotifyAccountService
    {
        public Task<bool> HasCredentialsAsync();

        Task<bool> TryAuthenticateAsync();

        Task<bool> PersistTokenAsync();

        
    }
}