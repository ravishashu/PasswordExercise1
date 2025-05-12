using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using PasswordExcercise;

namespace PasswordGenerator.API.Filters
{
    public class ValidatePasswordRequirementsAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (context.ActionArguments.TryGetValue("req", out var value) &&
                value is PasswordRequirements req)
            {
                var errors = new List<string>();

                if (req.MinLength <= 0)
                    errors.Add("MinLength must be greater than 0.");

                if (req.MaxLength < req.MinLength)
                    errors.Add("MaxLength must be greater than or equal to MinLength.");

                if (req.MinUpperAlphaChars < 0 || req.MinLowerAlphaChars < 0 ||
                    req.MinNumericChars < 0 || req.MinSpecialChars < 0)
                    errors.Add("Minimum character requirements cannot be negative.");

                int totalRequired = req.MinUpperAlphaChars + req.MinLowerAlphaChars +
                                    req.MinNumericChars + req.MinSpecialChars;

                if (totalRequired > req.MaxLength)
                    errors.Add("Total minimum character requirements exceed MaxLength.");

                if (errors.Any())
                {
                    context.Result = new BadRequestObjectResult(new { Errors = errors });
                }
            }
            else
            {
                context.Result = new BadRequestObjectResult("Invalid or missing request body.");
            }
        }
    }
}
