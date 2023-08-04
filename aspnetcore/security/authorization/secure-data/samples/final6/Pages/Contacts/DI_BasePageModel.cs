using System.Net;
using ContactManager.Data;
using Humanizer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ContactManager.Pages.Contacts
{
    // Create a base class that contains the services used in the contacts Razor Pages.
    // The base class puts the initialization code in one location:
    // 
    // The preceding code:
    //      Adds the IAuthorizationService service to access to the authorization handlers.
    //      Adds the Identity UserManager service.
    //      Add the ApplicationDbContext.
    public class DI_BasePageModel : PageModel
    {
        protected ApplicationDbContext Context { get; }
        protected IAuthorizationService AuthorizationService { get; }
        protected UserManager<IdentityUser> UserManager { get; }

        public DI_BasePageModel(
            ApplicationDbContext context,
            IAuthorizationService authorizationService,
            UserManager<IdentityUser> userManager) : base()
        {
            Context = context;
            UserManager = userManager;
            AuthorizationService = authorizationService;
        } 
    }
}
