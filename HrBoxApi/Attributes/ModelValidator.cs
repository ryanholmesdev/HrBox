using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace HrBoxApi.Filters
{
  public class ModelValidator : ActionFilterAttribute
  {
    public override void OnActionExecuting(ActionExecutingContext context)
    {
      if (!context.ModelState.IsValid)
      {
        // TODO: Check this.
        context.Result = new BadRequestObjectResult(context.ModelState); // returns 400 with validation error contents error
      }
    }
  }
}