using Moq;
using CampusTouch.Application.Interfaces;
using CampusTouch.Domain.Entities;

namespace CollegeERP.Tests.Common.Mocks
{
    public static class StudentRepositoryMock
    {
        public static Mock<IStudentRepository> GetMock()
        {
            var mock = new Mock<IStudentRepository>();

            mock.Setup(x => x.GetNextAdmissionSequence(It.IsAny<int>()))
                .ReturnsAsync(1);

            mock.Setup(x => x.AdmissionNumberExist(It.IsAny<string>()))
                .ReturnsAsync(false);

            mock.Setup(x => x.CreateStudentAsync(It.IsAny<Student>()))
                .ReturnsAsync(1);

            return mock;
        }
    }
}