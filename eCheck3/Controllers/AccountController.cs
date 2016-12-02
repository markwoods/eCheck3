using System;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using eCheck3.Models;

namespace eCheck3.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        private EMSDataCompanyEntities dbCompany = new EMSDataCompanyEntities();

        public AccountController()
        {
        }

        public AccountController(ApplicationUserManager userManager, ApplicationSignInManager signInManager )
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set 
            { 
                _signInManager = value; 
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        //
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            if ((returnUrl != null) && returnUrl.StartsWith("/Mobile/",
                                       StringComparison.OrdinalIgnoreCase))
            {
                return RedirectToAction("Login", "Account",
                                        new { Area = "Mobile", ReturnUrl = returnUrl });
            }
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Require the user to have a confirmed email before they can log on.
            var user = await UserManager.FindByNameAsync(model.UserName);
            if (user != null)
            {
                if (!await UserManager.IsEmailConfirmedAsync(user.Id))
                {
                    string callbackUrl = await SendEmailConfirmationTokenAsync(user.Id, "Please confirm your email address - Resend");
                    ModelState.AddModelError("", "You must have a confirmed email to log on.");
                    ModelState.AddModelError("", "The confirmation token has been resent to your email account.");
                    return View(model);
                }
            }
            // This doesn't count login failures towards account lockout
            // To enable password failures to trigger account lockout, change to shouldLockout: true
            var result = await SignInManager.PasswordSignInAsync(model.UserName, model.Password, model.RememberMe, shouldLockout: false);
            switch (result)
            {
                case SignInStatus.Success:
                    //user = await UserManager.FindByEmailAsync(model.Email);
                    // fullname claim is added in account register, and also if any name changes done in manage
                    
                    return RedirectToLocal(returnUrl);                     

                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = model.RememberMe });
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Invalid login attempt.");
                    return View(model);
            }
        }

        //
        // GET: /Account/VerifyCode
        [AllowAnonymous]
        public async Task<ActionResult> VerifyCode(string provider, string returnUrl, bool rememberMe)
        {
            // Require that the user has already logged in via username/password or external login
            if (!await SignInManager.HasBeenVerifiedAsync())
            {
                return View("Error");
            }
            return View(new VerifyCodeViewModel { Provider = provider, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        //
        // POST: /Account/VerifyCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> VerifyCode(VerifyCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // The following code protects for brute force attacks against the two factor codes. 
            // If a user enters incorrect codes for a specified amount of time then the user account 
            // will be locked out for a specified amount of time. 
            // You can configure the account lockout settings in IdentityConfig
            var result = await SignInManager.TwoFactorSignInAsync(model.Provider, model.Code, isPersistent:  model.RememberMe, rememberBrowser: model.RememberBrowser);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(model.ReturnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Invalid code.");
                    return View(model);
            }
        }

        //
        // GET: /Account/Register
        [AllowAnonymous]
        public ActionResult Register()
        {

            // temp delete user aaa@criticalcommunication.ca
            ApplicationUserManager usermanager = System.Web.HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>();
            ApplicationUser user = usermanager.FindByEmail("aaa@criticalcommunication.ca");
            if (user != null)
            {
                usermanager.Delete(user);
            }
            
            
            
            
            return View();
        }

        //
        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                user.LastName = model.LastName;
                user.FirstName = model.FirstName;

                // Get email suffix
                string strEmailSuffix = model.Email.Substring(model.Email.IndexOf("@")+1);

                // Try to find company ID from Company EMail                
                var intCompanyID = (from s in dbCompany.tbCompany_CompanyEmail
                                    where s.EmailSuffix == strEmailSuffix
                                    select s.CompanyID).FirstOrDefault();

                // If no company ID found, reject self-registration
                if (intCompanyID == 0) {
                    ModelState.AddModelError("Email", "Unapproved email for self-registration");
                    return View(model);                
                }

                // Assign company ID to user
                user.CompanyID = intCompanyID;
                // Create User
                var result = await UserManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    var t = await UserManager.AddClaimAsync(user.Id, new Claim("FullName", user.FirstName + " " + user.LastName));

                    // email confirmation
                    string callbackUrl = await SendEmailConfirmationTokenAsync(user.Id, "Please confirm your email address");

                    // Redirect to email sent page
                    return View("SendEmail");
                }
                AddErrors(result);
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/ConfirmEmail
        [AllowAnonymous]
        public async Task<ActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return View("Error");
            }
            var result = await UserManager.ConfirmEmailAsync(userId, code);
            return View(result.Succeeded ? "ConfirmEmail" : "Error");
        }

        //
        // GET: /Account/ForgotUsername
        [AllowAnonymous]
        public ActionResult ForgotUsername()
        {
            ForgotUsernameViewModel model = new ForgotUsernameViewModel();
            model.ValidationType = "email";
            return View(model);
        }

        //
        // POST: /Account/ForgotUsername
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ForgotUsername(ForgotUsernameViewModel model)
        {
            if (ModelState.IsValid)
            {
                // if send by phone, ensure phone is valid type, if by email, validate email type
                switch (model.ValidationType)
                {
                    case "email":
                        // Look for username by email
                        if (model.Email == null)
                        {
                            // ensure an address was sent.  (if not null, the model will validate format)
                            ModelState.AddModelError("Email", "No valid email address provided");
                            return View(model);
                        }
                        var user = await UserManager.FindByEmailAsync(model.Email);
                        if (user == null || !(await UserManager.IsEmailConfirmedAsync(user.Id)))
                        {
                            // Don't reveal that the user does not exist or is not confirmed
                            return View("ForgotUsernameConfirmation");
                        }
                        // Send email with username to user using appropriate sendgrid template
                        await UserManager.SendEmailAsync(user.Id, "Forgot Username", user.UserName);
                        return RedirectToAction("ForgotUsernameConfirmation", "Account");

                    case "phone":
                        if (model.Phone == null)
                        {
                            ModelState.AddModelError("Phone", "No valid phone number provided");
                            return View(model);
                        }
                        // Get Username from phone number
                        ApplicationUser phoneUser = await UserManager.FindByPhoneNumberAsync(model.Phone);

                        if (phoneUser == null)
                        {
                            ModelState.AddModelError("Phone", "No valid phone number provided");
                            return View(model);
                        }
                    
                        // Generate random 6-digit reset code
                        string strResetCode = await UserManager.GenerateTwoFactorTokenAsync(phoneUser.Id, "Phone Code");
                        // Send text to user
                        await UserManager.SendSmsAsync(phoneUser.Id, "Your EMSdata.ca security code is " + strResetCode);

                        // redirect to validation page
                        SMSVerificationViewModel vm = new SMSVerificationViewModel();
                        vm.userId = phoneUser.Id;
                        return View("VerifyCodeForUsername", vm);

                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/ForgotUsernameConfirmation
        [AllowAnonymous]
        public ActionResult ForgotUsernameConfirmation()
        {
            return View();
        }

        // POST: /Account/VerifyCodeForUsername
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> VerifyCodeForUsername(SMSVerificationViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (!await UserManager.VerifyTwoFactorTokenAsync(model.userId, "Phone Code", model.code))
                {
                    ModelState.AddModelError("code", "Invalid Code");
                    return View(model);
                }
                model.userName = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>().FindById(model.userId).UserName;
                return View(model);
            }
            return View(model);
        }

        //
        // GET: /Account/ForgotPassword
        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        //
        // POST: /Account/ForgotPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindByNameAsync(model.Email);
                if (user == null || !(await UserManager.IsEmailConfirmedAsync(user.Id)))
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    return View("ForgotPasswordConfirmation");
                }
                // Send an email with this link
                string code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
                var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                await UserManager.SendEmailAsync(user.Id, "Reset Password", callbackUrl);
                return RedirectToAction("ForgotPasswordConfirmation", "Account");
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/ForgotPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        //
        // GET: /Account/ResetPassword
        [AllowAnonymous]
        public async Task<ActionResult> ResetPassword(string code, string userID)
        {
            if (code == null)
            {
                return View("Error");
            }
            // Get User and see if 2 factor is set up
            var user = UserManager.FindById(userID);
            // If two factor is enabled, send there for verification
            if (user.TwoFactorEnabled){
                ResetPasswordWithTwoFactorViewModel vm = new ResetPasswordWithTwoFactorViewModel();
                vm.code = code;
                vm.userID = userID;

                vm.phone = user.PhoneNumber.Substring(user.PhoneNumber.Length-4);

                // Generate random 6-digit reset code
                string strResetCode = await UserManager.GenerateTwoFactorTokenAsync(userID, "Phone Code");
                // Send text to user
                await UserManager.SendSmsAsync(userID, "Your EMSdata.ca security code is " + strResetCode);

                return View("ResetPasswordWithTwoFactor", vm);
            }

            return View();
        }

        //
        // POST: /Account/ResetPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await UserManager.FindByNameAsync(model.Email);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            var result = await UserManager.ResetPasswordAsync(user.Id, model.Code, model.Password);
            if (result.Succeeded)
            {
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            AddErrors(result);
            return View();
        }

        //
        // GET: /Account/ResetPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        //
        //POST: /Account/ResetPasswordWithTwoFactor
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetPasswordWithTwoFactor(ResetPasswordWithTwoFactorViewModel model)
        {

            if (ModelState.IsValid)
            {
                if (!await UserManager.VerifyTwoFactorTokenAsync(model.userID, "Phone Code", model.SMSCode))
                {
                    ModelState.AddModelError("SMScode", "Invalid Code");
                    return View(model);
                }
                // Code Good, send to page for password reset
                ResetPasswordViewModel vm = new ResetPasswordViewModel();
                vm.Code = model.code;
                var user = await UserManager.FindByIdAsync(model.userID);

                vm.Email = user.Email;

                return View("ResetPassword", vm);
                //return View("ResetPasswordPostTwoFactor", model);
            }
            return View(model);

        } 
        
        //
        // GET: /Account/SendCode
        [AllowAnonymous]
        public async Task<ActionResult> SendCode(string returnUrl, bool rememberMe)
        {
            var userId = await SignInManager.GetVerifiedUserIdAsync();
            if (userId == null)
            {
                return View("Error");
            }
            var userFactors = await UserManager.GetValidTwoFactorProvidersAsync(userId);
            var factorOptions = userFactors.Select(purpose => new SelectListItem { Text = purpose, Value = purpose }).ToList();
            return View(new SendCodeViewModel { Providers = factorOptions, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        //
        // POST: /Account/SendCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SendCode(SendCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            // Generate the token and send it
            if (!await SignInManager.SendTwoFactorCodeAsync(model.SelectedProvider))
            {
                return View("Error");
            }
            return RedirectToAction("VerifyCode", new { Provider = model.SelectedProvider, ReturnUrl = model.ReturnUrl, RememberMe = model.RememberMe });
        }

        //
        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return RedirectToAction("Index", "Home");
        }

        private async Task<string> SendEmailConfirmationTokenAsync(string userID, string subject)
        {
            string code = await UserManager.GenerateEmailConfirmationTokenAsync(userID);
            var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = userID, code = code }, protocol: Request.Url.Scheme);
            await UserManager.SendEmailAsync(userID, subject, callbackUrl);
            return callbackUrl;
        }
        
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_userManager != null)
                {
                    _userManager.Dispose();
                    _userManager = null;
                }

                if (_signInManager != null)
                {
                    _signInManager.Dispose();
                    _signInManager = null;
                }
                dbCompany.Dispose();
            }

            base.Dispose(disposing);
        }

        #region Helpers
        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }

        internal class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }
        #endregion
    }
}