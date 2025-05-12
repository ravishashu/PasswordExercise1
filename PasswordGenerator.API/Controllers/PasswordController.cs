using Microsoft.AspNetCore.Mvc;
using PasswordExcercise;
using PasswordGenerator.API.Filters;

namespace PasswordGenerator.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PasswordController : ControllerBase
    {
        private readonly IPasswordGenerator _generator;

        public PasswordController(IPasswordGenerator generator)
        {
            _generator = generator;
        }

        [HttpPost("generate")]
        [ValidatePasswordRequirements]
        public ActionResult<string> Generate([FromBody] PasswordRequirements req)
        {
            var password = _generator.GeneratePassword(req);
            return Ok(password);
        }
    }
}
