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
    public class VehiclesController : ControllerBase
    {
        private readonly IVehicleService _vehicleService;

        public VehiclesController(IVehicleService vehicleService)
        {
            _vehicleService = vehicleService;
        }

        [HttpPost]
        public async Task<ActionResult<VehicleDto>> CreateVehicle(CreateVehicleDto vehicleDto)
        {
            try
            {
                var result = await _vehicleService.CreateVehicleAsync(vehicleDto);
                return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<VehicleDto>> GetById(Guid id)
        {
            var vehicle = await _vehicleService.GetVehicleByIdAsync(id);
            if (vehicle == null)
                return NotFound();

            return Ok(vehicle);
        }

        [HttpGet("user/{userId}")]
        public async Task<ActionResult<IEnumerable<VehicleDto>>> GetByUserId(Guid userId)
        {
            var vehicles = await _vehicleService.GetVehiclesByUserIdAsync(userId);
            return Ok(vehicles);
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<VehicleDto>>> GetAll()
        {
            var vehicles = await _vehicleService.GetAllVehiclesAsync();
            return Ok(vehicles);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<VehicleDto>> Update(Guid id, UpdateVehicleDto vehicleDto)
        {
            try
            {
                var result = await _vehicleService.UpdateVehicleAsync(id, vehicleDto);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _vehicleService.DeleteVehicleAsync(id);
            if (!result)
                return NotFound();

            return NoContent();
        }

        [HttpGet("{vehicleId}/service-history")]
        public async Task<ActionResult<IEnumerable<ServiceRequestDto>>> GetServiceHistory(Guid vehicleId)
        {
            var history = await _vehicleService.GetVehicleServiceHistoryAsync(vehicleId);
            return Ok(history);
        }
    }
} 