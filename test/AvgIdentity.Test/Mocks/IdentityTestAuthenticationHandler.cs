namespace AvgIdentity.Test.Mocks
{
    using System.Security.Claims;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Http.Features.Authentication;

    internal class IdentityTestAuthenticationHandler : IAuthenticationHandler
    {
        public ClaimsPrincipal CurrentPrincipal { get; set; }

        public Task AuthenticateAsync(AuthenticateContext context)
        {
            return Task.CompletedTask;
        }

        public Task ChallengeAsync(ChallengeContext context)
        {
            context.Accept();
            return Task.CompletedTask;
        }

        public void GetDescriptions(DescribeSchemesContext context)
        {
        }

        public Task SignInAsync(SignInContext context)
        {
            this.CurrentPrincipal = context.Principal;
            context.Accept();
            return Task.CompletedTask;
        }

        public Task SignOutAsync(SignOutContext context)
        {
            this.CurrentPrincipal = null;
            return Task.CompletedTask;
        }
    }
}