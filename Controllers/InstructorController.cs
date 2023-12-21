using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PoolManagerBackend.Interface;
using PoolManagerBackend.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PoolManagerBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InstructorController : ControllerBase
    {
        private readonly IInstructor _instructorService;

        public InstructorController(IInstructor instructorService)
        {
            _instructorService = instructorService;
        }

        [HttpGet("GetInstructors")]
        public IActionResult Get()
        {
            try
            {
                List<Instructor> instructors = _instructorService.GetAll().Result;

                return Ok(instructors);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error en la conexión: {ex.Message}");
            }
        }

        [HttpPost("CreateInstructor")]
        public async Task<IActionResult> CreateInstructor(Instructor instructor)
        {
            try
            {
                var status = await _instructorService.Create(instructor);
                return Ok(status);
            }
            catch (Exception ex)
            {
                return Ok($"Error crítico: {ex}");
            }
        }

        [HttpPut("EditInstructor")]
        public async Task<IActionResult> EditInstructor(Instructor instructor)
        {
            try
            {
                var status = await _instructorService.Edit(instructor);
                return Ok(status);
            }
            catch (Exception ex)
            {
                return Ok($"Error crítico: {ex}");
            }
        }

        [HttpDelete("DeleteInstructor")]
        public async Task<IActionResult> DeleteInstructor([FromBody] Delete request)
        {
            var instructorDeleted = await _instructorService.Delete(request.id);
            return Ok(instructorDeleted);
        }
    }
}
