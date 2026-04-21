using Xunit;
using Moq;
using FluentAssertions;
using CampusTouch.Application.Features.Students.Queries.GetStudentsById;
using CampusTouch.Application.Interfaces;
using CampusTouch.Domain.Entities;

namespace CollegeERP.Tests.Application.Students.Queries.GetStudentById
{
    public class GetStudentByIdHandlerTests
    {
        private readonly Mock<IStudentRepository> _studentRepo;
        private readonly GetStudentByIdHandler _handler;

        public GetStudentByIdHandlerTests()
        {
            _studentRepo = new Mock<IStudentRepository>();
            _handler = new GetStudentByIdHandler(_studentRepo.Object);
        }

        // ✅ SUCCESS CASE
        [Fact]
        public async Task Should_Return_Student_When_Id_Exists()
        {
            // Arrange
            var studentId = 1;

            var student = new Student
            {
                Id = 1,
                FirstName = "Jefin",
                LastName = "Jose",
                Email = "test@gmail.com",
                AdmissionNumber = "ADM001",
                IsActive = true
            };

            _studentRepo.Setup(x => x.GetStudentsById(studentId))
                .ReturnsAsync(student);

            var query = new GetStudentByIdQuery(studentId);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(studentId);
            result.Email.Should().Be("test@gmail.com");

            // 🔥 VERIFY MAPPING
            result.FullName.Should().Be("JefinJose");

            // ✔ Verify repo call
            _studentRepo.Verify(x => x.GetStudentsById(studentId), Times.Once);
        }

        // ❌ NOT FOUND CASE
        [Fact]
        public async Task Should_Return_Null_When_Student_Not_Exist()
        {
            var studentId = 1;

            _studentRepo.Setup(x => x.GetStudentsById(studentId))
                .ReturnsAsync((Student?)null);

            var query = new GetStudentByIdQuery(studentId);

            var result = await _handler.Handle(query, CancellationToken.None);

            result.Should().BeNull();
        }
    }
}