using DotNetCoreLearning.Models;
using Microsoft.AspNetCore.Authorization;

namespace DotNetCoreLearning
{
    public class OwnTodoRequirement : IAuthorizationRequirement { }

    public class OwnTodoAuthorizationHandler : AuthorizationHandler<OwnTodoRequirement, ToDo>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, OwnTodoRequirement requirement, ToDo resource)
        {
            if (context.User.IsInRole("Admin"))
            {
                context.Succeed(requirement);
            }
            else if (context.User.IsInRole("User"))
            {
                if (resource.CreatedBy == context.User.Identity.Name)
                {
                    context.Succeed(requirement);
                }
            }

            return Task.CompletedTask;
        }
    }
}
