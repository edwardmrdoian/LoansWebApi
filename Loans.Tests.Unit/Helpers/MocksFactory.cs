using AutoMapper;
using Loans.Contracts;
using Moq;

namespace Loans.Tests.Unit.Helpers
{
    public static class MocksFactory
    {
        public static (Mock<IRepositoryManager> repoMock, Mock<IMapper> mapperMock, Mock<ILoggerManager> loggerMock) CreateCoreMocks()
        {
            var repoMock = new Mock<IRepositoryManager>();
            var mapperMock = new Mock<IMapper>();
            var loggerMock = new Mock<ILoggerManager>();
            return (repoMock, mapperMock, loggerMock);
        }
    }
}
