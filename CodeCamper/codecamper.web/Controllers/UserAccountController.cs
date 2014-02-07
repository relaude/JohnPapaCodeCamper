using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Security.Principal;
using System.Web.Security;
using System.Threading;
using System.Web;
using System.Web.Helpers;
using Microsoft.Web.WebPages.OAuth;
using DotNetOpenAuth.AspNet;
using codecamper.web.Filters;
using DataModel;
using codecamper.web.Properties;
using System.Net;
using WebMatrix.WebData;
using System.Net.Http;

namespace codecamper.web.Controllers
{
    [ModelValidation]
    public class UserAccountController : ApiController
    {
        ServicePovider.UserAccount _usr;

        public UserAccountController(ServicePovider.UserAccount _usr)
        {
            this._usr = _usr;
        }

        /// Sign in user
        /// </summary>
        /// <param name="credential">User credentials</param>
        /// <returns>Authenticathion object containing the result of this operation</returns>
        [HttpPost]
        [HttpOptions]
        [AllowAnonymous]
        public UserInfo Login(Credential credential)
        {
            // try to sign in
            if (WebSecurity.Login(credential.UserName, credential.Password, persistCookie: credential.RememberMe))
            {
                // Create a new Principal and return authenticated                
                IPrincipal principal = new GenericPrincipal(new GenericIdentity(credential.UserName), Roles.GetRolesForUser(credential.UserName));
                Thread.CurrentPrincipal = principal;
                HttpContext.Current.User = principal;
                return new UserInfo
                {
                    IsAuthenticated = true,
                    UserName = credential.UserName,
                    Roles = Roles.GetRolesForUser(credential.UserName)
                };
            }
            // if you get here => return 401 Unauthorized
            var errors = new Dictionary<string, IEnumerable<string>>();
            errors.Add("Authorization", new string[] { "The supplied credentials are not valid" });
            throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.Unauthorized, errors));
        }

        /// <summary>
        /// Sign out the authenticated user
        /// </summary>
        /// <returns>Authenticathion object containing the result of the sign out operation</returns>
        [HttpPost]
        [AntiForgeryToken]
        [HttpOptions]
        public UserInfo Logout()
        {
            // Try to sign out
            try
            {
                WebSecurity.Logout();
                Thread.CurrentPrincipal = null;
                HttpContext.Current.User = null;
            }
            catch (MembershipCreateUserException e)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, e.Message));
            }
            // return user not already authenticated
            return new UserInfo
            {
                IsAuthenticated = false,
                UserName = "",
                Roles = new string[] { }
            };
        }

        /// <summary>
        /// Register a new account using the Membership system
        /// </summary>
        /// <param name="model">Register model</param>
        /// <returns>Authenticathion object containing the result of the register operation</returns>
        [HttpPost]
        [AllowAnonymous]
        [AntiForgeryToken]
        public UserInfo Register(RegisterModel model)
        {
            try
            {
                WebSecurity.CreateUserAndAccount(model.UserName, model.Password);
                WebSecurity.Login(model.UserName, model.Password);
                Roles.AddUsersToRole(new string[] { model.UserName }, Settings.Default.DefaultRole);
                IPrincipal principal = new GenericPrincipal(new GenericIdentity(model.UserName), Roles.GetRolesForUser(model.UserName));
                Thread.CurrentPrincipal = principal;
                HttpContext.Current.User = principal;
                return new UserInfo()
                {
                    IsAuthenticated = true,
                    UserName = model.UserName,
                    Roles = new List<string> { Settings.Default.DefaultRole }
                };
            }
            catch (MembershipCreateUserException e)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, e.Message));
            }
        }

        [HttpPost]
        public void AddUser(AddEditUser model)
        {
            WebSecurity.CreateUserAndAccount(model.UserName, model.Password);
            System.Web.Security.Roles.AddUserToRoles(model.UserName, model.RoleName);
        }

        /// <summary>
        /// Get the actual authentication status
        /// </summary>
        /// <returns>User authentication object</returns>
        [HttpGet]
        [AllowAnonymous]
        public UserInfo UserInfo()
        {
            if (WebSecurity.IsAuthenticated)
            {
                return new UserInfo
                {
                    IsAuthenticated = true,
                    UserName = WebSecurity.CurrentUserName,
                    Roles = Roles.GetRolesForUser(WebSecurity.CurrentUserName)
                };
            }
            else
            {
                return new UserInfo
                {
                    IsAuthenticated = false
                };
            }
        }

        /// <summary>
        /// Check if the oAuth user has already a local account
        /// </summary>
        /// <returns>bool</returns>
        [HttpGet]
        public bool HasLocalAccount()
        {
            return OAuthWebSecurity.HasLocalAccount(WebSecurity.GetUserId(User.Identity.Name));
        }

        /// <summary>
        /// Change the password of the authenticated user
        /// </summary>
        /// <param name="model">The change password model</param>
        /// <returns>http response</returns>
        [HttpPost]
        [AntiForgeryToken]
        public HttpResponseMessage ChangePassword(ChangePasswordModel model)
        {
            // Cannot change passwords for test users
            // Remove following lines for real usage
            if (Roles.IsUserInRole("Administrator") || WebSecurity.CurrentUserName == "user")
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Cannot change test users passwords (admin & user) in this demo app. Remove lines in ChangePassword (AccountController) action for real usage"));
            }

            bool changePasswordSucceeded;
            try
            {
                changePasswordSucceeded = WebSecurity.ChangePassword(User.Identity.Name, model.OldPassword, model.NewPassword);
            }
            catch (Exception)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Unable to change the password"));
            }

            if (changePasswordSucceeded)
            {
                return new HttpResponseMessage(HttpStatusCode.OK);
            }
            else
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "The current password is incorrect or the new password is invalid."));
            }

            throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Password and confirmation should match"));
        }

        /// <summary>
        /// Creates a new local account for a user authenticated with any external provider
        /// </summary>
        /// <param name="model">The model</param>
        /// <returns>http response</returns>
        [HttpPost]
        [AntiForgeryToken]
        public HttpResponseMessage CreateLocalAccount(CreateLocalAccountModel model)
        {
            try
            {
                WebSecurity.CreateAccount(User.Identity.Name, model.NewPassword);
                return new HttpResponseMessage(HttpStatusCode.OK);
            }
            catch (Exception e)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, e.Message));
            }
            throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Password and confirmation should match"));
        }

        /// <summary>
        /// Get antiforgery tokens
        /// </summary>
        /// <returns>the anti forgery token</returns>
        [HttpGet]
        [AllowAnonymous]
        public string GetAntiForgeryTokens()
        {
            string cookieToken = "", formToken = "";
            AntiForgery.GetTokens(null, out cookieToken, out formToken);
            HttpContext.Current.Response.Cookies[AntiForgeryConfig.CookieName].Value = cookieToken;
            return formToken;
        }
        
    }
}
