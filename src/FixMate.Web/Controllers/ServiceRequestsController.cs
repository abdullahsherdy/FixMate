using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using FixMate.Application.Interfaces.Services;
using FixMate.Application.DTOs;

namespace FixMate.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ServiceRequestsController : ControllerBase
    {
        private readonly IServiceRequestService _serviceRequestService;

        public ServiceRequestsController(IServiceRequestService serviceRequestService)
        {
            _serviceRequestService = serviceRequestService;
        }

        [HttpPost]
        public async Task<ActionResult<ServiceRequestDto>> CreateServiceRequest(CreateServiceRequestDto requestDto)
        {
            var result = await _serviceRequestService.CreateServiceRequestAsync(requestDto);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ServiceRequestDto>> GetById(Guid id)
        {
            var request = await _serviceRequestService.GetServiceRequestByIdAsync(id);
            if (request == null)
                return NotFound();

            return Ok(request);
        }

        [HttpGet("user/{userId}")]
        public async Task<ActionResult<IEnumerable<ServiceRequestDto>>> GetByUserId(Guid userId)
        {
            var requests = await _serviceRequestService.GetServiceRequestsByUserIdAsync(userId);
            return Ok(requests);
        }

        [HttpGet("mechanic/{mechanicId}")]
        public async Task<ActionResult<IEnumerable<ServiceRequestDto>>> GetByMechanicId(Guid mechanicId)
        {
            var requests = await _serviceRequestService.GetServiceRequestsByMechanicIdAsync(mechanicId);
            return Ok(requests);
        }

        [HttpPut("{id}/status")]
        [Authorize(Roles = "Admin,ServiceProvider")]
        public async Task<ActionResult<ServiceRequestDto>> UpdateStatus(Guid id, UpdateServiceStatusDto statusDto)
        {
            try
            {
                var request = await _serviceRequestService.UpdateServiceStatusAsync(id, statusDto);
                return Ok(request);
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpPut("{id}/assign")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ServiceRequestDto>> AssignServiceProvider(Guid id, AssignServiceProviderDto assignDto)
        {
            try
            {
                var request = await _serviceRequestService.AssignServiceProviderAsync(id, assignDto);
                return Ok(request);
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ServiceRequestDto>> Update(Guid id, UpdateServiceRequestDto requestDto)
        {
            try
            {
                var request = await _serviceRequestService.UpdateServiceRequestAsync(id, requestDto);
                return Ok(request);
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
            var result = await _serviceRequestService.DeleteServiceRequestAsync(id);
            if (!result)
                return NotFound();

            return NoContent();
        }
    }
} 