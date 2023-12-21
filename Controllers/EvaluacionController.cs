using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PoolManagerBackend.Interface;
using PoolManagerBackend.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PoolManagerBackend.Controllers
{
    [Route("Oracle/[controller]")]
    [ApiController]
    public class EvaluacionController : ControllerBase
    {
        private readonly IEvalucionInstructor _evaluacionService;

        public EvaluacionController(IEvalucionInstructor evaluacionService)
        {
            _evaluacionService = evaluacionService;
        }

        [HttpGet("GetAllEvaluaciones")]
        public async Task<IActionResult> GetAllEvaluaciones()
        {
            try
            {
                List<EvaluacionInstructor> evaluaciones = await _evaluacionService.GetAll();
                return Ok(evaluaciones);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error en la conexión: {ex.Message}");
            }
        }

        [HttpPost("CreateEvaluacion")]
        public async Task<IActionResult> CreateEvaluacion(EvaluacionInstructor evaluacionInstructor)
        {
            try
            {
                var status = await _evaluacionService.Create(evaluacionInstructor);
                return Ok(status);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error crítico: {ex.Message}");
            }
        }

        [HttpPut("EditEvaluacion")]
        public async Task<IActionResult> EditEvaluacion(EvaluacionInstructor evaluacionInstructor)
        {
            try
            {
                var editedEvaluacion = await _evaluacionService.Edit(evaluacionInstructor);
                if (editedEvaluacion != null)
                {
                    return Ok(editedEvaluacion);
                }
                else
                {
                    return NotFound($"Evaluación con ID {evaluacionInstructor.ID_Evaluacion} no encontrada.");
                }
            }
            catch (Exception ex)
            {
                return BadRequest($"Error crítico: {ex.Message}");
            }
        }

        [HttpDelete("DeleteEvaluacion/{id}")]
        public async Task<IActionResult> DeleteEvaluacion(int id)
        {
            var evaluacionEliminada = await _evaluacionService.Delete(id);
            if (evaluacionEliminada)
            {
                return Ok($"Evaluación con ID {id} eliminada exitosamente.");
            }
            else
            {
                return NotFound($"Evaluación con ID {id} no encontrada.");
            }
        }
        [HttpGet("GetEvaluacionInstructorById/{id}")]
        public async Task<IActionResult> GetEvaluacionInstructorById(int id)
        {
            try
            {
                var evaluacionInstructor = await _evaluacionService.GetEvaluacionInstructorById(id);

                if (evaluacionInstructor != null)
                {
                    return Ok(evaluacionInstructor);
                }
                else
                {
                    return NotFound($"No se encontró ninguna evaluación de instructor con el ID {id}");
                }
            }
            catch (Exception ex)
            {
                return BadRequest($"Error en la conexión: {ex.Message}");
            }
        }



    }
}
