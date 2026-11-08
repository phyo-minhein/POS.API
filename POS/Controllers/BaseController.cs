using Microsoft.AspNetCore.Mvc;
using POS.API.ViewModels;
using System.Text.Json.Serialization;

namespace POS.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BaseController : ControllerBase
    {     
        [NonAction]
        protected IActionResult Execute<T>(Result<T> model)
        {
            if (model.IsValidationError)
                return BadRequest(model);

            if (model.IsSystemError)
                return StatusCode(500, model);

            return Ok(model);
        }
    }
}
