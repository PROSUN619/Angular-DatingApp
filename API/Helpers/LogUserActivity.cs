using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Extensions;
using API.Interfaces;
using Microsoft.AspNetCore.Mvc.Filters;

namespace API.Helpers
{
    public class LogUserActivity : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
             //----->before execution of action method
             var resultContext = await next();    
             //----->After execution of action method

            if (!resultContext.HttpContext.User.Identity.IsAuthenticated) 
                return;

            var userId = resultContext.HttpContext.User.GetUserId();

            var unitOfWork = resultContext.HttpContext.RequestServices.GetRequiredService<IUnitOfWork>();
            var user = await unitOfWork.UserRepository.GetUserByIdWithoutPhotosAsync(Convert.ToInt32(userId));
            user.LastActive = DateTime.Now;
            await unitOfWork.CompleteAsync();
            //use this in the middleware to get the functionality worked

           // throw new NotImplementedException();
        }
    }
}