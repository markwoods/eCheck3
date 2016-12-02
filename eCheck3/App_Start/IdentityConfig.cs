using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using eCheck3.Models;
using SendGrid;
using SendGrid.Helpers.Mail;
using System.Configuration;
using Twilio;
using System.Diagnostics;

namespace eCheck3
{
    public class EmailService : IIdentityMessageService        
    {
        public async Task SendAsync(IdentityMessage message)
        {
            await configSendGridasync(message);
        }

        private async Task configSendGridasync(IdentityMessage message)
        {
            string apiKey = ConfigurationManager.AppSettings["SendGridAPI"];  
            
            dynamic sg = new SendGridAPIClient(apiKey);

            Email from = new Email("NoReply@CriticalCommunication.ca");
            Content content = new Content("text/html", message.Body);
            Email to = new Email(message.Destination);
            Mail mail = new Mail(from, message.Subject, to, content);
            switch (message.Subject)
            {
                case "Please confirm your email address - Resend":
                    mail.TemplateId = "e00c5c5a-4eda-42ad-b1ca-42f6a042d7c5";
                    mail.Personalization[0].AddSubstitution("-ConfirmURL-", message.Body);
                    break;
                case "Please confirm your email address":
                    mail.TemplateId = "e00c5c5a-4eda-42ad-b1ca-42f6a042d7c5";
                    mail.Personalization[0].AddSubstitution("-ConfirmURL-", message.Body);
                    break;
                case "Reset Password":
                    mail.TemplateId = "797f8782-66dc-4f66-bdaf-b4fe2512cc20";
                    mail.Personalization[0].AddSubstitution("-ResetURL-", message.Body);
                    break;
                case "Forgot Username":
                    mail.TemplateId = "55e6223e-bac2-4b08-b99c-cb7839dbc54d";
                    break;

            }

            dynamic response = await sg.client.mail.send.post(requestBody: mail.Get());
        }    
    }

    public class SmsService : IIdentityMessageService
    {

        public Task SendAsync(IdentityMessage message)
        {
            // Twilio Begin
            var Twilio = new TwilioRestClient("AC5415201f030cf1bd30b459351c77aaa6", "d6892658770506aa45f69b7f43ad2f48");

            var result = Twilio.SendMessage("15876002911", message.Destination, message.Body);

            // Status is one of Queued, Sending, Sent, Failed or null if the number is not valid
             Trace.TraceInformation(result.Status);

            // Twilio doesn't currently have an async API, so return success.
             return Task.FromResult(0);
            }
    }

    // Configure the application user manager used in this application. UserManager is defined in ASP.NET Identity and is used by the application.
    public class ApplicationUserManager : UserManager<ApplicationUser>
    {
        public ApplicationUserManager(IUserStore<ApplicationUser> store)
            : base(store)
        {
        }

        public static ApplicationUserManager Create(IdentityFactoryOptions<ApplicationUserManager> options, IOwinContext context) 
        {
            var manager = new ApplicationUserManager(new UserStore<ApplicationUser>(context.Get<ApplicationDbContext>()));
            // Configure validation logic for usernames
            manager.UserValidator = new UserValidator<ApplicationUser>(manager)
            {
                AllowOnlyAlphanumericUserNames = false,
                RequireUniqueEmail = true
            };

            // Configure validation logic for passwords
            manager.PasswordValidator = new PasswordValidator
            {
                RequiredLength = 6,
                RequireNonLetterOrDigit = false,
                RequireDigit = false,
                RequireLowercase = false,
                RequireUppercase = false,
            };

            // Configure user lockout defaults
            manager.UserLockoutEnabledByDefault = true;
            manager.DefaultAccountLockoutTimeSpan = TimeSpan.FromMinutes(5);
            manager.MaxFailedAccessAttemptsBeforeLockout = 5;

            // Register two factor authentication providers. 
            manager.RegisterTwoFactorProvider("Phone Code", new PhoneNumberTokenProvider<ApplicationUser>
            {
                MessageFormat = "Your EMSdata.ca security code is {0}"
            });
            manager.EmailService = new EmailService();
            manager.SmsService = new SmsService();
            var dataProtectionProvider = options.DataProtectionProvider;
            if (dataProtectionProvider != null)
            {
                manager.UserTokenProvider = 
                    new DataProtectorTokenProvider<ApplicationUser>(dataProtectionProvider.Create("ASP.NET Identity"));
            }
            return manager;
        }
        

        public async Task<ApplicationUser> FindByPhoneNumberAsync(string phoneNumber) {

            if (phoneNumber == null)
            {
                throw new ArgumentNullException("phoneNumber");
            }

            ApplicationUser currentUser = await Users.FirstOrDefaultAsync(x => x.PhoneNumber == phoneNumber);
            return(currentUser);

        }

    }

    // Configure the application sign-in manager which is used in this application.
    public class ApplicationSignInManager : SignInManager<ApplicationUser, string>
    {
        public ApplicationSignInManager(ApplicationUserManager userManager, IAuthenticationManager authenticationManager)
            : base(userManager, authenticationManager)
        {
        }

        public override Task<ClaimsIdentity> CreateUserIdentityAsync(ApplicationUser user)
        {
            return user.GenerateUserIdentityAsync((ApplicationUserManager)UserManager);
        }

        public static ApplicationSignInManager Create(IdentityFactoryOptions<ApplicationSignInManager> options, IOwinContext context)
        {
            return new ApplicationSignInManager(context.GetUserManager<ApplicationUserManager>(), context.Authentication);
        }
    }
}
