﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthenticationService.Application.Contracts
{
    public interface IAuthService
    {
        Task<string> LoginAsync(string email, string password);
        Task<string> RefreshTokenAsync();
    }

}
