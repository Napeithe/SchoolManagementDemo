using Microsoft.AspNetCore.Http;
using Moq;

namespace SchoolManagementTest.Moq
{
    public static class HttpContextAccessorMoq
    {
        public static Mock<IHttpContextAccessor> Get()
        {
            Mock<IHttpContextAccessor> contextAccessorMock = new Mock<IHttpContextAccessor>();

            contextAccessorMock.Setup(x => x.HttpContext.Request.Scheme).Returns(() => "http");
            contextAccessorMock.Setup(x => x.HttpContext.Request.Host).Returns(() => new HostString("localhost", 9000));
            return contextAccessorMock;
        }
    }
}
