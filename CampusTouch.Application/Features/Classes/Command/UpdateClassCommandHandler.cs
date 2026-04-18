//using CampusTouch.Application.Common.Exceptions;
//using CampusTouch.Application.Interfaces;
//using MediatR;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace CampusTouch.Application.Features.Classes.Command
//{
//    public  class UpdateClassCommandHandler:IRequestHandler<UpdateClassCommand,int>
//    {

//        private readonly IClassesRepository _repo;
//        private readonly ICurrentUserService _currentUser;

//        public UpdateClassCommandHandler(IClassesRepository repo, ICurrentUserService currentUser)
//        {
//            _repo = repo;
//            _currentUser = currentUser;
//        }


//        public async Task<int> Handle(UpdateClassCommand request, CancellationToken cancellationToken)
//        {
//            if (!_currentUser.IsAdmin)
//            {
//                throw new UnauthorizedAccessException("Only admins can update classes");
//            }
//            var existingClass = await _repo.GetByIdAsync(request.Id);
//            if (existingClass == null)
//            {
//                throw new NotFoundException("Class not found");
//            }
//            if (await _repo.ExistsAsync(request.Year, request.Semester, request.Id))
//            {
//                throw new BuisnessRuleException("Class already exists");
//            }
//            existingClass.Name = request.Name;
//            existingClass.DepartmentId = request.DepartmentId;
//            existingClass.CourseId = request.CourseId;
//            existingClass.Year = request.Year;
//            existingClass.Semester = request.Semester;
//            await _repo.UpdateAsync(request.Id, existingClass);
//            return request.Id;
//        }
//    }
//}

using CampusTouch.Application.Common.Exceptions;
using CampusTouch.Application.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CampusTouch.Application.Features.Classes.Command
{
    public class UpdateClassCommandHandler : IRequestHandler<UpdateClassCommand, int>
    {
        private readonly IClassesRepository _repo;
        private readonly ICurrentUserService _currentUser;
        private readonly ILogger<UpdateClassCommandHandler> _logger;

        public UpdateClassCommandHandler(
            IClassesRepository repo,
            ICurrentUserService currentUser,
            ILogger<UpdateClassCommandHandler> logger)
        {
            _repo = repo;
            _currentUser = currentUser;
            _logger = logger;
        }

        public async Task<int> Handle(UpdateClassCommand request, CancellationToken cancellationToken)
        {
            var userId = _currentUser.UserId;

            _logger.LogInformation("User {UserId} attempting to update class {ClassId}", userId, request.Id);

            // 🔐 Authorization
            if (!_currentUser.IsAdmin)
            {
                _logger.LogWarning("Unauthorized update attempt by User {UserId} for Class {ClassId}", userId, request.Id);
                throw new UnauthorizedAccessException("Only admins can update classes");
            }

            var existingClass = await _repo.GetByIdAsync(request.Id);

            if (existingClass == null)
            {
                _logger.LogWarning("Update failed: Class {ClassId} not found (User {UserId})", request.Id, userId);
                throw new NotFoundException("Class not found");
            }

            // 🔥 Duplicate check
            if (await _repo.ExistsAsync(request.CourseId, request.Year, request.Semester))
            {
                _logger.LogWarning(
                    "Duplicate update attempt by User {UserId} for Class {ClassId} (Course {CourseId}, Year {Year}, Semester {Semester})",
                    userId, request.Id, request.CourseId, request.Year, request.Semester);

                throw new BuisnessRuleException("Class already exists");
            }

            // 📝 Update fields
            existingClass.Name = request.Name;
            existingClass.DepartmentId = request.DepartmentId;
            existingClass.CourseId = request.CourseId;
            existingClass.Year = request.Year;
            existingClass.Semester = request.Semester;

            await _repo.UpdateAsync(request.Id, existingClass);

            // ✅ Audit log (MOST IMPORTANT)
            _logger.LogInformation("User {UserId} updated class {ClassId} successfully", userId, request.Id);

            return request.Id;
        }
    }
}