using Xunit;
using Moq;
using FluentAssertions;
using CampusTouch.Application.Features.Students.Commands;
using CampusTouch.Application.Interfaces;
using CampusTouch.Domain.Entities;
using CampusTouch.Application.Common.Exceptions;
using Microsoft.Extensions.Logging;
using CollegeERP.Tests.Common.Fixtures;

namespace CollegeErp.Tests.Application.Students.Commands.DeleteStudent
{
    public class DeleteStudentHandlerTests
    {
        private readonly Mock<IStudentRepository> _studentRepo;
        private readonly Mock<ICurrentUserService> _currentUser;
        private readonly Mock<ILogger<DeleteStudentCommandHandler>> _logger;

        private readonly DeleteStudentCommandHandler _handler;

        public DeleteStudentHandlerTests()
        {
            _studentRepo = new Mock<IStudentRepository>();
            _currentUser = new Mock<ICurrentUserService>();
            _logger = new Mock<ILogger<DeleteStudentCommandHandler>>();

            _handler = new DeleteStudentCommandHandler(
                _studentRepo.Object,
                _currentUser.Object,
                _logger.Object);
        }

        // ✅ SUCCESS
        [Fact]
        public async Task Should_Delete_Student_When_Valid()
        {
            var command = new DeleteStudentCommand(1);

            _currentUser.Setup(x => x.IsAdmin).Returns(true);
            _currentUser.Setup(x => x.UserId).Returns("admin1");

            var student = StudentTestData.GetStudent();

            _studentRepo.Setup(x => x.GetStudentsById(1))
                .ReturnsAsync(student);

            _studentRepo.Setup(x => x.DeleteStudent(1, "admin1"))
                .ReturnsAsync(true);

            var result = await _handler.Handle(command, CancellationToken.None);

            result.Should().BeTrue();

            // 🔥 STATE CHECK (IMPORTANT)
            student.IsDeleted.Should().BeTrue();
            student.IsActive.Should().BeFalse();
            student.DeletedBy.Should().Be("admin1");
            student.DeletedAt.Should().NotBeNull();

            // ✔ METHOD VERIFY
            _studentRepo.Verify(x => x.DeleteStudent(1, "admin1"), Times.Once);
        }

        // ❌ UNAUTHORIZED
        [Fact]
        public async Task Should_Throw_Unauthorized_When_Not_Admin()
        {
            var command = new DeleteStudentCommand(1);

            _currentUser.Setup(x => x.IsAdmin).Returns(false);

            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

            await act.Should().ThrowAsync<UnauthorizedAccessException>();
        }

        // ❌ NOT FOUND
        [Fact]
        public async Task Should_Throw_NotFound_When_Student_Not_Exist()
        {
            var command = new DeleteStudentCommand(1);

            _currentUser.Setup(x => x.IsAdmin).Returns(true);

            _studentRepo.Setup(x => x.GetStudentsById(It.IsAny<int>()))
                .ReturnsAsync((Student?)null);

            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

            await act.Should().ThrowAsync<NotFoundException>();
        }

        // ❌ ALREADY DELETED
        [Fact]
        public async Task Should_Throw_NotFound_When_Student_Already_Deleted()
        {
            var command = new DeleteStudentCommand(1);

            _currentUser.Setup(x => x.IsAdmin).Returns(true);

            var student = StudentTestData.GetStudent();
            student.IsDeleted = true;

            _studentRepo.Setup(x => x.GetStudentsById(1))
                .ReturnsAsync(student);

            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

            await act.Should().ThrowAsync<NotFoundException>();
        }

        // ❌ DELETE FAILED
        [Fact]
        public async Task Should_Return_False_When_Delete_Fails()
        {
            var command = new DeleteStudentCommand(1);

            _currentUser.Setup(x => x.IsAdmin).Returns(true);
            _currentUser.Setup(x => x.UserId).Returns("admin1");

            var student = StudentTestData.GetStudent();

            _studentRepo.Setup(x => x.GetStudentsById(1))
                .ReturnsAsync(student);

            _studentRepo.Setup(x => x.DeleteStudent(1, "admin1"))
                .ReturnsAsync(false);

            var result = await _handler.Handle(command, CancellationToken.None);

            result.Should().BeFalse();
        }
    }
}