using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Model.Domain;
using Moq;

namespace SchoolManagementTest.Moq
{
    public class SignInManagerMoq : SignInManager<User>
    {
        public SignInManagerMoq()
            : base(UserManagerMoq.Get().Object,
                new HttpContextAccessor(),
                new Mock<IUserClaimsPrincipalFactory<User>>().Object,
                new Mock<IOptions<IdentityOptions>>().Object,
                new Mock<ILogger<SignInManager<User>>>().Object,
                new Mock<AuthenticationSchemeProvider>().Object)
        {

        }


        public static Mock<SignInManager<User>> Get()
        {
            return new Mock<SignInManager<User>>(
                UserManagerMoq.Get().Object, new HttpContextAccessor(), new Mock<IUserClaimsPrincipalFactory<User>>().Object, null, null, null);
        }
    }
}
