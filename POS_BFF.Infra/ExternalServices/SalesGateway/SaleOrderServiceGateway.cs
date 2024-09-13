﻿using POS_BFF.Application.Contracts;
using POS_BFF.Core.Domain.Entities;
using POS_BFF.Core.Domain.Enums;
using POS_BFF.Core.Domain.Gateways.Cashier;
using POS_BFF.Core.Domain.Gateways.Sales;
using POS_BFF.Core.Domain.Repositories;
using POS_BFF.Core.Domain.Requests;
using POS_BFF.Domain.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System.Net.Http.Json;

namespace POS_BFF.Infra.ExternalServices.SalesGateway
{
    public class SalesOrderServiceGateway : ISaleOrderServiceGateway
    {
        private static IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ISaleProductServiceGateway _saleProductServiceGateway;
        private readonly IUserRepository _userRepository;
        private readonly ICashierOrderServiceGateway _cashierOrderServiceGateway;
        private readonly IConsumerServiceRepository _consumerServiceRepository;

        public SalesOrderServiceGateway(IHttpClientFactory httpClientFactory,
            IConfiguration configuration,
            IAuthService authService,
            IHttpContextAccessor httpContextAccessor,
            ISaleProductServiceGateway saleProductServiceGateway,
            IUserRepository userRepository,
            ICashierOrderServiceGateway cashierOrderServiceGateway,
            IConsumerServiceRepository consumerServiceRepository)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
            _saleProductServiceGateway = saleProductServiceGateway;
            _userRepository = userRepository;
            _cashierOrderServiceGateway = cashierOrderServiceGateway;
            _consumerServiceRepository = consumerServiceRepository;
        }

        private async Task<HttpClient> CreateHttpClientAsync()
        {
            var baseAddress = _configuration["SalesApi:baseAddress"];
            var httpClient = _httpClientFactory.CreateClient("SalesServiceClient");
            
            httpClient.BaseAddress = new Uri(baseAddress);
          
            return httpClient;
        }

        public async Task<Guid> CreateSaleAsync(SaleDTO saleDto)
        {
            var httpClient = await CreateHttpClientAsync();
            var response = await httpClient.PostAsJsonAsync("api/Sales", saleDto);
            response.EnsureSuccessStatusCode();
            var saleId = await response.Content.ReadFromJsonAsync<Guid>();
            return saleId;
        }

        public async Task<SaleDTO> GetSaleByIdAsync(Guid id)
        {
            var httpClient = await CreateHttpClientAsync();
            var response = await httpClient.GetAsync($"api/Sales/{id}");
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<SaleDTO>();
            }
            throw new HttpRequestException(response.ReasonPhrase);
        }
        public async Task<List<OrderItems>> GetOrderItemsByOrder(Guid orderid)
        {
            var httpClient = await CreateHttpClientAsync();
            var response = await httpClient.GetAsync($"api/Sales/{orderid}/items");
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<List<OrderItems>>();
            }
            throw new HttpRequestException(response.ReasonPhrase);
        }
        public async Task<IEnumerable<SaleDTO>> GetAllSalesAsync()
        {
            var httpClient = await CreateHttpClientAsync();
            var response = await httpClient.GetAsync("api/Sales");
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<IEnumerable<SaleDTO>>();
            }
            throw new HttpRequestException(response.ReasonPhrase);
        }

        public async Task<bool> CompleteSaleAsync(Guid id, SaleDTO saleDTO)
        {
            var httpClient = await CreateHttpClientAsync();
            var response = await httpClient.PutAsJsonAsync($"api/Sales/{id}/complete", saleDTO); 
            if (response.IsSuccessStatusCode)
            {
                var items = await GetOrderItemsByOrder(id);
                
                
                foreach (var item in items)
                {
                    
                    if(item.ProductType == EProductType.VirtualProduct)
                    {
                        var service = await _saleProductServiceGateway.GetServiceById(item.ProductId);
                        if(service.IsComputer == true)
                        {
                           item.Quantity += await _userRepository.GetAvailableTime(saleDTO.ClientId);
                           await _userRepository.UpdateUserClientAvailableTimeAsync(saleDTO.ClientId, item.Quantity);
                        }
                        else
                        {
                            var consumerService = new ConsumerService();
                            consumerService.userId = saleDTO.ClientId;
                            consumerService.orderId = saleDTO.Id;
                            consumerService.is_Active = true;
                            consumerService.totalTime = item.Quantity;
                            consumerService.serviceName = item.Name;
                            await _consumerServiceRepository.CreateConsumerService(consumerService);
                        }
                    }                                             
                }
            }
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> CancelSaleAsync(Guid id)
        {
            var httpClient = await CreateHttpClientAsync();
            var response = await httpClient.PutAsJsonAsync($"api/Sales/{id}/cancel", id);
            return response.IsSuccessStatusCode;
        }

        public async Task<Dictionary<EPaymentType, decimal>> GetDailyTotals()
        {
            var httpClient = await CreateHttpClientAsync();
            var response = await httpClient.GetAsync("api/Sales/daily-totals");
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<Dictionary<EPaymentType, decimal>>();
            }
            throw new HttpRequestException(response.ReasonPhrase);
        }
    }
}