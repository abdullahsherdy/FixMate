using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using FixMate.Application.Interfaces.Services;
using FixMate.Application.DTOs;
using FixMate.Domain.Enums;

namespace FixMate.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ServiceProvidersController : ControllerBase
    {
        private readonly IServiceProviderService _serviceProviderService;

        public ServiceProvidersController(IServiceProviderService serviceProviderService)
        {
            _serviceProviderService = serviceProviderService;
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ServiceProviderDto>> CreateServiceProvider(CreateServiceProviderDto providerDto)
        {
            try
            {
                var result = await _serviceProviderService.CreateServiceProviderAsync(providerDto);
                return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ServiceProviderDto>> GetById(Guid id)
        {
            var provider = await _serviceProviderService.GetServiceProviderByIdAsync(id);
            if (provider == null)
                return NotFound();

            return Ok(provider);
        }

        [HttpGet("email/{email}")]
        public async Task<ActionResult<ServiceProviderDto>> GetByEmail(string email)
        {
            var provider = await _serviceProviderService.GetServiceProviderByEmailAsync(email);
            if (provider == null)
                return NotFound();

            return Ok(provider);
        }

        [HttpGet("specialization/{specialization}")]
        public async Task<ActionResult<IEnumerable<ServiceProviderDto>>> GetBySpecialization(Specialization specialization)
        {
            var providers = await _serviceProviderService.GetServiceProvidersBySpecializationAsync(specialization);
            return Ok(providers);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ServiceProviderDto>>> GetAll()
        {
            var providers = await _serviceProviderService.GetAllServiceProvidersAsync();
            return Ok(providers);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ServiceProviderDto>> Update(Guid id, UpdateServiceProviderDto providerDto)
        {
            try
            {
                var result = await _serviceProviderService.UpdateServiceProviderAsync(id, providerDto);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _serviceProviderService.DeleteServiceProviderAsync(id);
            if (!result)
                return NotFound();

            return NoContent();
        }

        [HttpGet("{providerId}/assigned-requests")]
        public async Task<ActionResult<IEnumerable<ServiceRequestDto>>> GetAssignedRequests(Guid providerId)
        {
            var requests = await _serviceProviderService.GetAssignedServiceRequestsAsync(providerId);
            return Ok(requests);
        }

        [HttpPut("{id}/availability")]
        public async Task<IActionResult> UpdateAvailability(Guid id, UpdateAvailabilityDto availabilityDto)
        {
            var result = await _serviceProviderService.UpdateAvailabilityAsync(id, availabilityDto);
            if (!result)
                return NotFound();

            return NoContent();
        }
    }
} 