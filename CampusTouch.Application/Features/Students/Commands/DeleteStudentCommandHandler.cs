//using CampusTouch.Application.Common.Exceptions;
//using CampusTouch.Application.Interfaces;
//using MediatR;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace CampusTouch.Application.Features.Students.Commands
//{
//    public class DeleteStudentCommandHandler:IRequestHandler<DeleteStudentCommand,bool>
//    {
//        private readonly IStudentRepository _studentRepository;
//        private readonly ICurrentUserService _currentUserService;
//        public DeleteStudentCommandHandler(IStudentRepository studentRepository,ICurrentUserService currentUserService)
//        {
//           _studentRepository = studentRepository;
//            _currentUserService = currentUserService;   
//        }

//        public async Task<bool> Handle(DeleteStudentCommand request, CancellationToken cancellationToken)
//        {
//            var userid = _currentUserService.UserId;
//            if (!_currentUserService.IsAdmin)
//            {
//                throw new UnauthorizedAccessException("Only admin can delete student");
//            }
//            var studentexisting = await _studentRepository.GetStudentsById(request.id);
//            if (studentexisting == null || studentexisting.IsDeleted)
//            {
//                throw new NotFoundException("Student is not exist");
//            }
//            studentexisting.IsDeleted = true;
//            studentexisting.DeletedAt = DateTime.UtcNow;
//            studentexisting.DeletedBy = userid;
//            studentexisting.IsActive = false;
//            return  await _studentRepository.DeleteStudent(request.id,userid);
//        }
//    }
//}


using CampusTouch.Application.Common.Exceptions;
using CampusTouch.Application.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CampusTouch.Application.Features.Students.Commands
{
    public class DeleteStudentCommandHandler : IRequestHandler<DeleteStudentCommand, bool>
    {
        private readonly IStudentRepository _studentRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly ILogger<DeleteStudentCommandHandler> _logger;

        public DeleteStudentCommandHandler(
            IStudentRepository studentRepository,
            ICurrentUserService currentUserService,
            ILogger<DeleteStudentCommandHandler> logger)
        {
            _studentRepository = studentRepository;
            _currentUserService = currentUserService;
            _logger = logger;
        }

        public async Task<bool> Handle(DeleteStudentCommand request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId;

            // ✅ Attempt log
            _logger.LogInformation(
                "User {UserId} attempting to delete student {StudentId}",
                userId, request.id);

            // 🔐 Authorization
            if (!_currentUserService.IsAdmin)
            {
                _logger.LogWarning(
                    "Unauthorized delete attempt by User {UserId} for Student {StudentId}",
                    userId, request.id);

                throw new UnauthorizedAccessException("Only admin can delete student");
            }

            var student = await _studentRepository.GetStudentsById(request.id);

            if (student == null || student.IsDeleted)
            {
                _logger.LogWarning(
                    "Delete failed: Student {StudentId} not found (User {UserId})",
                    request.id, userId);

                throw new NotFoundException("Student not found");
            }

            // 🗑 Soft delete
            student.IsDeleted = true;
            student.DeletedAt = DateTime.UtcNow;
            student.DeletedBy = userId;
            student.IsActive = false;

            var result = await _studentRepository.DeleteStudent(request.id, userId);

            if (result)
            {
                // ✅ Audit log (VERY IMPORTANT)
                _logger.LogInformation(
                    "User {UserId} deleted student {StudentId} ({Email}) successfully",
                    userId, student.Id, student.Email);
            }
            else
            {
                // ❌ Unexpected failure
                _logger.LogError(
                    "Failed to delete student {StudentId} by User {UserId}",
                    request.id, userId);
            }

            return result;
        }
    }
}