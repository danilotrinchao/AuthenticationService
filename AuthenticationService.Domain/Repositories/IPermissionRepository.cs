﻿using AuthenticationService.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthenticationService.Domain.Repositories
{
    public interface IPermissionRepository : IRepository<Permission>
    {
        // Adicione métodos específicos do repositório de permissões, se necessário
    }
}
