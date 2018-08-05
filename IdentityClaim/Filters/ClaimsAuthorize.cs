using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;

namespace IdentityClaim
{
    public class ClaimsAuthorizeAttribute : AuthorizeAttribute
    {
        private string ClaimType;
        private string ClaimValue;
        public ClaimsAuthorizeAttribute(string claimType,
           string claimValue)
        {
            this.ClaimType = claimType;
            this.ClaimValue = claimValue;
        }
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            var user = filterContext.HttpContext.User as ClaimsPrincipal;
            if (user != null && user.HasClaim(ClaimType, ClaimValue))
            {
                base.OnAuthorization(filterContext);
            }
            else
            {
                base.HandleUnauthorizedRequest(filterContext);
            }

        }
    }
}