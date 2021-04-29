using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoScalingService.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace AutoScalingService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TasksController : ControllerBase
    {

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] ComputeRequest request)
        {
            if (request == null)
            {
                return BadRequest();
            }
            try
            {
                TasksHandler.Instance.HandleInstances(request);
                return Ok(request);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Error");
            }
            
        }


    }
}
