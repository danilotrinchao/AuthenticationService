﻿using AuthenticationService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthenticationService.Domain.Repositories
{
    public interface IRoleRepository : IRepository<Role>
    {
        // Adicione métodos específicos do repositório de roles, se necessário
    }
}
