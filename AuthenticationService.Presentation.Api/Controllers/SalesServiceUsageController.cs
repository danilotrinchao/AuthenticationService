﻿using AuthenticationService.Core.Domain.Gateways.Sales;
using Microsoft.AspNetCore.Mvc;

namespace AuthenticationService.Presentation.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SalesServiceUsageController : ControllerBase
    {
        private readonly ISalesServiceUsageGateway _salesServiceUsageGateway;
        private readonly ILogger<SalesServiceUsageController> _logger;

        public SalesServiceUsageController(ISalesServiceUsageGateway salesServiceUsageGateway, ILogger<SalesServiceUsageController> logger)
        {
            _salesServiceUsageGateway = salesServiceUsageGateway;
            _logger = logger;
        }

        [HttpPost("start/{orderItemId}")]
        public async Task<IActionResult> StartService(Guid orderItemId)
        {
            try
            {
                await _salesServiceUsageGateway.StartServiceAsync(orderItemId);
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error starting service for order item ID {orderItemId}");
                return StatusCode(500, "Failed to start service");
            }
        }

        [HttpPost("pause/{orderItemId}")]
        public async Task<IActionResult> PauseService(Guid orderItemId)
        {
            try
            {
                await _salesServiceUsageGateway.PauseServiceAsync(orderItemId);
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error pausing service for order item ID {orderItemId}");
                return StatusCode(500, "Failed to pause service");
            }
        }

        [HttpPost("stop/{orderItemId}")]
        public async Task<IActionResult> StopService(Guid orderItemId)
        {
            try
            {
                await _salesServiceUsageGateway.StopServiceAsync(orderItemId);
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error stopping service for order item ID {orderItemId}");
                return StatusCode(500, "Failed to stop service");
            }
        }

        [HttpGet("{orderItemId}")]
        public async Task<IActionResult> GetServiceUsage(Guid orderItemId)
        {
            try
            {
                var serviceUsage = await _salesServiceUsageGateway.GetServiceUsageByOrderItemIdAsync(orderItemId);
                return Ok(serviceUsage);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting service usage for order item ID {orderItemId}");
                return StatusCode(500, "Failed to retrieve service usage");
            }
        }
    }
}
