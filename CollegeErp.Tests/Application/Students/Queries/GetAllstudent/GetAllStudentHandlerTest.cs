
using Xunit;
using Moq;
using FluentAssertions;
using CampusTouch.Application.Features.Students.Queries.GetAllStudents;
using CampusTouch.Application.Interfaces;
using CampusTouch.Domain.Entities;

namespace CollegeERP.Tests.Application.Students.Queries.GetAllStudents
{
    public class GetAllStudentsHandlerTests
    {
        private readonly Mock<IStudentRepository> _studentRepo;
        private readonly GetAllStudentsHandler _handler;

        public GetAllStudentsHandlerTests()
        {
            _studentRepo = new Mock<IStudentRepository>();
            _handler = new GetAllStudentsHandler(_studentRepo.Object);
        }

        // ✅ SUCCESS CASE
        [Fact]
        public async Task Should_Return_All_Students_With_Mapping()
        {
            // Arrange
            var students = new List<Student>
            {
                new Student
                {
                    Id = 1,
                    FirstName = "Jefin",
                    LastName = "Jose",
                    Email = "test@gmail.com",
                    AdmissionNumber = "ADM001",
                    IsActive = true
                },
                new Student
                {
                    Id = 2,
                    FirstName = "John",
                    LastName = "Doe",
                    Email = "john@gmail.com",
                    AdmissionNumber = "ADM002",
                    IsActive = true
                }
            };

            _studentRepo.Setup(x => x.GetAllStudents(1, 10, null))
                .ReturnsAsync(students);

            // 🔥 FIXED: Pass constructor parameters
            var query = new GetAllStudentsQuery(1, 10, null);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Count().Should().Be(2);

            // 🔥 VERIFY MAPPING
            result.First().Id.Should().Be(1);
            result.First().FullName.Should().Be("JefinJose");
            result.First().Email.Should().Be("test@gmail.com");

            // ✔ Verify repository call
            _studentRepo.Verify(x => x.GetAllStudents(1, 10, null), Times.Once);
        }

        // ❌ EMPTY CASE
        [Fact]
        public async Task Should_Return_Empty_List_When_No_Students()
        {
            _studentRepo.Setup(x => x.GetAllStudents(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>()))
                .ReturnsAsync(new List<Student>());

            // 🔥 FIXED: Pass parameters
            var query = new GetAllStudentsQuery(1, 10, null);

            var result = await _handler.Handle(query, CancellationToken.None);

            result.Should().BeEmpty();
        }
    }
}