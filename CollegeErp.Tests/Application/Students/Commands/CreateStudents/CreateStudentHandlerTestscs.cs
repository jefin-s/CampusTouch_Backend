using Xunit;
using Moq;
using FluentAssertions;
using CampusTouch.Application.Features.Students.Commands;
using CampusTouch.Application.Interfaces;
using CampusTouch.Domain.Entities;
using CampusTouch.Application.Common.Exceptions;
using Microsoft.Extensions.Logging;
using CollegeERP.Tests.Common.Mocks;
using CollegeERP.Tests.Common.Fixtures;

namespace CollegeERP.Tests.Application.Students.Commands.CreateStudent
{
    public class CreateStudentHandlerTests
    {
        private readonly Mock<IStudentRepository> _studentRepo;
        private readonly Mock<ICurrentUserService> _currentUser;
        private readonly Mock<IProgramRepository> _programRepo;
        private readonly Mock<IDepartementRepository> _deptRepo;
        private readonly Mock<IIdentityService> _identityService;
        private readonly Mock<IEmailService> _emailService;
        private readonly Mock<ILogger<CreateStudentHandler>> _logger;

        private readonly CreateStudentHandler _handler;

        public CreateStudentHandlerTests()
        {
            _studentRepo = StudentRepositoryMock.GetMock();
            _currentUser = new Mock<ICurrentUserService>();
            _programRepo = new Mock<IProgramRepository>();
            _deptRepo = new Mock<IDepartementRepository>();
            _identityService = new Mock<IIdentityService>();
            _emailService = new Mock<IEmailService>();
            _logger = new Mock<ILogger<CreateStudentHandler>>();

            _handler = new CreateStudentHandler(
                _studentRepo.Object,
                _currentUser.Object,
                _programRepo.Object,
                _deptRepo.Object,
                _identityService.Object,
                _emailService.Object,
                _logger.Object);
        }

        // ✅ SUCCESS CASE
        [Fact]
        public async Task Should_Create_Student_When_Valid_Data()
        {
            var command = StudentTestData.ValidCommand();

            _currentUser.Setup(x => x.IsAdmin).Returns(true);
            _currentUser.Setup(x => x.UserId).Returns("admin1");

            _programRepo.Setup(x => x.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(new AcademicProgram { Id = 1, IsDeleted = false });

            _deptRepo.Setup(x => x.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(new Departement { Id = 1, code = "CS", isDeleted = false });

            _identityService.Setup(x => x.RegisterAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                "Student"))
                .ReturnsAsync("user123");

            var result = await _handler.Handle(command, CancellationToken.None);

            result.Should().BeTrue();

            _studentRepo.Verify(x => x.CreateStudentAsync(It.IsAny<Student>()), Times.Once);
            _emailService.Verify(x => x.SendAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }

        // ❌ UNAUTHORIZED
        [Fact]
        public async Task Should_Throw_Unauthorized_When_Not_Admin()
        {
            var command = StudentTestData.ValidCommand();

            _currentUser.Setup(x => x.IsAdmin).Returns(false);

            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

            await act.Should().ThrowAsync<UnauthorizedException>();
        }

        // ❌ INVALID COURSE / DEPARTMENT
        [Fact]
        public async Task Should_Throw_NotFound_When_Invalid_Data()
        {
            var command = StudentTestData.ValidCommand();

            _currentUser.Setup(x => x.IsAdmin).Returns(true);

            _programRepo.Setup(x => x.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync((AcademicProgram)null);

            _deptRepo.Setup(x => x.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync((Departement)null);

            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

            await act.Should().ThrowAsync<NotFoundException>();
        }
    }
}