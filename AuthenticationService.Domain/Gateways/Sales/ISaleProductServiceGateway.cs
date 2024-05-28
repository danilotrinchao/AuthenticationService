﻿using AuthenticationService.Core.Domain.Enums;
using AuthenticationService.Core.Domain.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthenticationService.Core.Domain.Gateways.Sales
{
    public interface ISaleProductServiceGateway
    {
        Task<Guid> AddProductAsync(ProductDTO productDto);
        Task<bool> UpdateProductAsync(Guid id, ProductDTO productDto);
        Task<bool> DeleteProductAsync(Guid id);
        Task<ProductDTO> GetProductAsync(Guid id);
        Task<IEnumerable<ProductDTO>> GetAllProductsAsync();
    }
}
