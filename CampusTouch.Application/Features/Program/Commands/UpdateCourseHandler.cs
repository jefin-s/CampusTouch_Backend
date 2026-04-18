//using CampusTouch.Application.Common.Exceptions;
//using CampusTouch.Application.Interfaces;
//using MediatR;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Linq.Expressions;
//using System.Security.Cryptography.X509Certificates;
//using System.Text;
//using System.Threading.Tasks;

//namespace CampusTouch.Application.Features.Program.Commands
//{
//    public class UpdateCourseHandler:IRequestHandler<UpdateCourseCommand,int>
//    {
//        private readonly IProgramRepository _programRepository;
//        private readonly ICurrentUserService _currentUserService;
//        private readonly IDepartementRepository _departementRepository;

//        public UpdateCourseHandler(IProgramRepository programRepository,ICurrentUserService currentUserService,IDepartementRepository departementRepository)
//        {
//            _programRepository = programRepository;
//            _currentUserService=currentUserService;
//            _departementRepository=departementRepository;
//        }
//            public async Task<int> Handle(UpdateCourseCommand request, CancellationToken cancellationToken)
//        {

//            if (!_currentUserService.IsAdmin)
//            {
//                throw new UnauthorizedException("Only Admin can Updaet the course");
//            }
//            var userId = _currentUserService.UserId;
//            var existingCourse = await _programRepository.GetByIdAsync(request.Id);
//            if (existingCourse == null||existingCourse.IsDeleted)
//            {
//                throw new NotFoundException("Program is not Exist");
//            }
//            var existing = await _departementRepository.GetByIdAsync(request.DepartmentId);

//            if(existing==null)
//            {
//                throw new NotFoundException("Departement is not exist");
//            }
//            var name = request.Name.Trim();
//            existingCourse.Name = name;
//            existingCourse.Level = request.Level;
//            existingCourse.Duration = request.Duration;
//            existingCourse.DepartmentId = request.DepartmentId;
//            existingCourse.UpdatedAt = DateTime.UtcNow;
//            existingCourse.UpdatedBy = userId;
//            return await _programRepository.UpdateAsync(existingCourse);
//        } 
//        }
//    }


using CampusTouch.Application.Common.Exceptions;
using CampusTouch.Application.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CampusTouch.Application.Features.Program.Commands
{
    public class UpdateCourseHandler : IRequestHandler<UpdateCourseCommand, int>
    {
        private readonly IProgramRepository _programRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IDepartementRepository _departementRepository;
        private readonly ILogger<UpdateCourseHandler> _logger;

        public UpdateCourseHandler(
            IProgramRepository programRepository,
            ICurrentUserService currentUserService,
            IDepartementRepository departementRepository,
            ILogger<UpdateCourseHandler> logger)
        {
            _programRepository = programRepository;
            _currentUserService = currentUserService;
            _departementRepository = departementRepository;
            _logger = logger;
        }

        public async Task<int> Handle(UpdateCourseCommand request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId;

            // ✅ Attempt log
            _logger.LogInformation("User {UserId} attempting to update program {ProgramId}", userId, request.Id);

            // 🔐 Authorization
            if (!_currentUserService.IsAdmin)
            {
                _logger.LogWarning("Unauthorized update attempt by User {UserId} for Program {ProgramId}", userId, request.Id);
                throw new UnauthorizedException("Only admin can update programs");
            }

            var existingCourse = await _programRepository.GetByIdAsync(request.Id);

            if (existingCourse == null || existingCourse.IsDeleted)
            {
                _logger.LogWarning("Update failed: Program {ProgramId} not found (User {UserId})", request.Id, userId);
                throw new NotFoundException("Program not found");
            }

            // 🔍 Department check
            var department = await _departementRepository.GetByIdAsync(request.DepartmentId);

            if (department == null)
            {
                _logger.LogWarning(
                    "Update failed: Department {DepartmentId} not found for Program {ProgramId} (User {UserId})",
                    request.DepartmentId, request.Id, userId);

                throw new NotFoundException("Department not found");
            }

            var name = request.Name.Trim();

            // (Optional) capture old values for audit
            var oldName = existingCourse.Name;
            var oldDepartmentId = existingCourse.DepartmentId;

            // 📝 Update fields
            existingCourse.Name = name;
            existingCourse.Level = request.Level;
            existingCourse.Duration = request.Duration;
            existingCourse.DepartmentId = request.DepartmentId;
            existingCourse.UpdatedAt = DateTime.UtcNow;
            existingCourse.UpdatedBy = userId;

            await _programRepository.UpdateAsync(existingCourse);

            // ✅ Audit log (VERY IMPORTANT)
            _logger.LogInformation(
                "User {UserId} updated program {ProgramId} successfully. Name: {OldName} → {NewName}, Department: {OldDept} → {NewDept}",
                userId, request.Id, oldName, name, oldDepartmentId, request.DepartmentId);

            return request.Id;
        }
    }
}