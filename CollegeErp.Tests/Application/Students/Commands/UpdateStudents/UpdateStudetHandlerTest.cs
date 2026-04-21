using Xunit;
using Moq;
using FluentAssertions;
using CampusTouch.Application.Features.Students.Commands;
using CampusTouch.Application.Interfaces;
using CampusTouch.Domain.Entities;
using CampusTouch.Application.Common.Exceptions;
using Microsoft.Extensions.Logging;
using CollegeERP.Tests.Common.Fixtures;

namespace CollegeERP.Tests.Application.Students.Commands.UpdateStudent
{
    public class UpdateStudentHandlerTests
    {
        private readonly Mock<IStudentRepository> _studentRepo;
        private readonly Mock<ICurrentUserService> _currentUser;
        private readonly Mock<ILogger<UpdateStudentHandler>> _logger;

        private readonly UpdateStudentHandler _handler;

        public UpdateStudentHandlerTests()
        {
            _studentRepo = new Mock<IStudentRepository>();
            _currentUser = new Mock<ICurrentUserService>();
            _logger = new Mock<ILogger<UpdateStudentHandler>>();

            _handler = new UpdateStudentHandler(
                _studentRepo.Object,
                _currentUser.Object,
                _logger.Object);
        }

        // ✅ SUCCESS
        [Fact]
        public async Task Should_Update_Student_When_Valid_Data()
        {
            var command = StudentTestData.UpdateCommand();

            _currentUser.Setup(x => x.IsAdmin).Returns(true);
            _currentUser.Setup(x => x.UserId).Returns("admin1");

            var student = StudentTestData.GetStudent();

            _studentRepo.Setup(x => x.GetStudentsById(command.Id))
                .ReturnsAsync(student);

            _studentRepo.Setup(x => x.AdmissionNumberExist(It.IsAny<string>()))
                .ReturnsAsync(false);

            var result = await _handler.Handle(command, CancellationToken.None);

            result.Should().BeTrue();

            _studentRepo.Verify(x => x.UpdateStudent(It.IsAny<Student>()), Times.Once);

            // 🔥 VERIFY STATE CHANGE
            student.FirstName.Should().Be(command.FirstName);
            student.Email.Should().Be(command.Email);
            student.AdmissionNumber.Should().Be(command.AdmissionNumber);
            student.UpdatedBy.Should().Be("admin1");
        }

        // ❌ UNAUTHORIZED
        [Fact]
        public async Task Should_Throw_Unauthorized_When_Not_Admin()
        {
            var command = StudentTestData.UpdateCommand();

            _currentUser.Setup(x => x.IsAdmin).Returns(false);

            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

            await act.Should().ThrowAsync<UnauthorizedAccessException>();
        }

        // ❌ NOT FOUND
        [Fact]
        public async Task Should_Throw_NotFound_When_Student_Not_Exist()
        {
            var command = StudentTestData.UpdateCommand();

            _currentUser.Setup(x => x.IsAdmin).Returns(true);

            _studentRepo.Setup(x => x.GetStudentsById(It.IsAny<int>()))
                .ReturnsAsync((Student)null);

            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

            await act.Should().ThrowAsync<NotFoundException>();
        }

        // ❌ DUPLICATE
        [Fact]
        public async Task Should_Throw_When_AdmissionNumber_Already_Exists()
        {
            var command = StudentTestData.UpdateCommand();

            _currentUser.Setup(x => x.IsAdmin).Returns(true);

            var student = new Student { Id = 2, IsDeleted = false };

            _studentRepo.Setup(x => x.GetStudentsById(It.IsAny<int>()))
                .ReturnsAsync(student);

            _studentRepo.Setup(x => x.AdmissionNumberExist(It.IsAny<string>()))
                .ReturnsAsync(true);

            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

            await act.Should().ThrowAsync<BuisnessRuleException>();
        }
    }
}