using ContactManager.Models;
using Humanizer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;

namespace ContactManager.Authorization
{
    // Create a ContactIsOwnerAuthorizationHandler class in the Authorization folder. 
    // The ContactIsOwnerAuthorizationHandler verifies that the user acting on a resource owns the resource.
    // The app allows contact owners to edit/delete/create their own data.
    public class ContactIsOwnerAuthorizationHandler
                : AuthorizationHandler<OperationAuthorizationRequirement, Contact>
    {
        UserManager<IdentityUser> _userManager;

        public ContactIsOwnerAuthorizationHandler(UserManager<IdentityUser> 
            userManager)
        {
            _userManager = userManager;
        }

        // The ContactIsOwnerAuthorizationHandler calls context.Succeed if the current authenticated user is the contact owner. Authorization handlers generally:
        protected override Task
            HandleRequirementAsync(AuthorizationHandlerContext context,
                                   OperationAuthorizationRequirement requirement,
                                   Contact resource)
        {
            if (context.User == null || resource == null)
            {
                // Return Task.CompletedTask when requirements aren't met.
                return Task.CompletedTask;
            }

            // If not asking for CRUD permission, return.

            if (requirement.Name != Constants.CreateOperationName &&
                requirement.Name != Constants.ReadOperationName   &&
                requirement.Name != Constants.UpdateOperationName &&
                requirement.Name != Constants.DeleteOperationName )
            {
                // Return Task.CompletedTask when requirements aren't met.
                return Task.CompletedTask;
            }

            if (resource.OwnerID == _userManager.GetUserId(context.User))
            {
                // Call context.Succeed when the requirements are met.
                context.Succeed(requirement);
            }

            // Return Task.CompletedTask when requirements aren't met.
            return Task.CompletedTask;
        }
    }
}
