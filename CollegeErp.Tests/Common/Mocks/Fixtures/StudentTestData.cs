//using CampusTouch.Application.Features.Students.Commands;

//namespace CollegeERP.Tests.Common.Fixtures
//{
//    public static class StudentTestData
//    {
//        public static CreateStudentCommand ValidCommand()
//        {
//            return new CreateStudentCommand(
//                1,                              // CourseId
//                1,                              // DepartmentId
//                DateTime.UtcNow,                // AdmissionDate
//                "Jefin",                        // FirstName
//                "Test",                         // LastName
//                DateTime.UtcNow.AddYears(-20),  // DateOfBirth
//                "Male",                         // Gender
//                "9999999999",                   // PhoneNumber
//                "test@gmail.com",               // Email
//                "Address",                      // Address
//                "Father Name",                  // GuardianName
//                "8888888888",                   // GuardianPhone
//                "O+",                           // BloodGroup
//                null                            // ProfileImageUrl
//            );
//        }
//    }
//}
using CampusTouch.Application.Features.Students.Commands;
using CampusTouch.Domain.Entities;

namespace CollegeERP.Tests.Common.Fixtures
{
    public static class StudentTestData
    {
        // ✅ CREATE
        public static CreateStudentCommand ValidCommand()
        {
            return new CreateStudentCommand(
                1, 1, DateTime.UtcNow,
                "Jefin", "Test", DateTime.UtcNow.AddYears(-20),
                "Male", "9999999999", "test@gmail.com",
                "Address", "Father Name", "8888888888",
                "O+", null
            );
        }

        // ✅ UPDATE COMMAND
        public static UpdateStudentCommand UpdateCommand()
        {
            return new UpdateStudentCommand(
                1, "ADM001", 1, 1, DateTime.UtcNow,
                "Jefin", "Updated", DateTime.UtcNow.AddYears(-20),
                "Male", "9999999999", "updated@gmail.com",
                "Address", "Father", "8888888888",
                "O+", null
            );
        }

        // ✅ STUDENT ENTITY (for repository)
        public static Student GetStudent()
        {
            return new Student
            {
                Id = 1,
                AdmissionNumber = "OLD001",
                FirstName = "Old",
                LastName = "Name",
                Email = "old@gmail.com",
                IsDeleted = false
            };
        }
    }
}