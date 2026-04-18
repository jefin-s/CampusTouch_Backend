//using CampusTouch.Application.Common.Exceptions;
//using CampusTouch.Application.Interfaces;
//using CampusTouch.Domain.Entities;
//using MediatR;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Runtime.CompilerServices;
//using System.Text;
//using System.Threading.Tasks;

//namespace CampusTouch.Application.Features.AssignSubject.Commands
//{
//    public  class AssignSubjectHandler:IRequestHandler<AssignSubjectCommand,bool>
//    {
//        private readonly IStaffRepository _staffRepository;
//        private readonly ISubjectRepository _subjectRepository;
//        private  readonly ICurrentUserService _currentUserService;
//        private readonly IStaffSubjectRepository _staffSubjectRepository;
//        public AssignSubjectHandler(ISubjectRepository subjectRepository,IStaffRepository staffRepository, IStaffSubjectRepository staffSubjectRepository,ICurrentUserService currentUserService)
//        {
//            _staffRepository = staffRepository;
//            _subjectRepository = subjectRepository;
//            _staffSubjectRepository = staffSubjectRepository;
//            _currentUserService = currentUserService;

//        }
//        public async Task <bool> Handle(AssignSubjectCommand command,CancellationToken  cancellationToken)

//        {
//            if (!_currentUserService.IsAdmin)
//                throw new UnauthorizedException("Only admin can assign the subject to staff");
//            var staff = await _staffRepository.GetStaffById(command.StaffId);
//            if(staff == null)
//            {
//                throw new NotFoundException("Staff not found");
//            }
//            var subject = await _subjectRepository.GetByIds(command.SubjectIds);

//            if (subject.Count != command.SubjectIds.Count)
//                throw new NotFoundException("One or more subjects not found");
//            await _staffSubjectRepository.RemoveAllByStaffId(command.StaffId);

//            // 🔥 4. Add new mappings
//            foreach (var subjectId in command.SubjectIds)
//            {
//                var staffSubject = new StaffSubject
//                {
//                    StaffId = command.StaffId,
//                    SubjectId = subjectId,
//                    CreatedAt = DateTime.UtcNow,
//                    CreatedBy=_currentUserService.UserId

//                };

//                await _staffSubjectRepository.AddAsync(staffSubject);
//            }

//            return true;
//        }

//    }
//}

using CampusTouch.Application.Common.Exceptions;
using CampusTouch.Application.Interfaces;
using CampusTouch.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CampusTouch.Application.Features.AssignSubject.Commands
{
    public class AssignSubjectHandler : IRequestHandler<AssignSubjectCommand, bool>
    {
        private readonly IStaffRepository _staffRepository;
        private readonly ISubjectRepository _subjectRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IStaffSubjectRepository _staffSubjectRepository;
        private readonly ILogger<AssignSubjectHandler> _logger;

        public AssignSubjectHandler(
            ISubjectRepository subjectRepository,
            IStaffRepository staffRepository,
            IStaffSubjectRepository staffSubjectRepository,
            ICurrentUserService currentUserService,
            ILogger<AssignSubjectHandler> logger)
        {
            _staffRepository = staffRepository;
            _subjectRepository = subjectRepository;
            _staffSubjectRepository = staffSubjectRepository;
            _currentUserService = currentUserService;
            _logger = logger;
        }

        public async Task<bool> Handle(AssignSubjectCommand command, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId;

            // ✅ Attempt log
            _logger.LogInformation(
                "User {UserId} attempting to assign {Count} subjects to Staff {StaffId}",
                userId, command.SubjectIds.Count, command.StaffId);

            // 🔐 Authorization
            if (!_currentUserService.IsAdmin)
            {
                _logger.LogWarning(
                    "Unauthorized subject assignment attempt by User {UserId} for Staff {StaffId}",
                    userId, command.StaffId);

                throw new UnauthorizedException("Only admin can assign subjects");
            }

            // 🔍 Validate staff
            var staff = await _staffRepository.GetStaffById(command.StaffId);

            if (staff == null)
            {
                _logger.LogWarning(
                    "Subject assignment failed: Staff {StaffId} not found (User {UserId})",
                    command.StaffId, userId);

                throw new NotFoundException("Staff not found");
            }

            // 🔍 Validate subjects
            var subjects = await _subjectRepository.GetByIds(command.SubjectIds);

            if (subjects.Count != command.SubjectIds.Count)
            {
                _logger.LogWarning(
                    "Subject assignment failed: One or more subjects not found for Staff {StaffId} (User {UserId})",
                    command.StaffId, userId);

                throw new NotFoundException("One or more subjects not found");
            }

            try
            {
                // 🧹 Remove old mappings
                await _staffSubjectRepository.RemoveAllByStaffId(command.StaffId);

                // ➕ Add new mappings
                foreach (var subjectId in command.SubjectIds)
                {
                    var staffSubject = new StaffSubject
                    {
                        StaffId = command.StaffId,
                        SubjectId = subjectId,
                        CreatedAt = DateTime.UtcNow,
                        CreatedBy = userId
                    };

                    await _staffSubjectRepository.AddAsync(staffSubject);
                }

                // ✅ Audit log (VERY IMPORTANT)
                _logger.LogInformation(
                    "User {UserId} assigned {Count} subjects to Staff {StaffId} successfully",
                    userId, command.SubjectIds.Count, command.StaffId);

                return true;
            }
            catch (Exception ex)
            {
                // ❌ Error log
                _logger.LogError(
                    ex,
                    "Error assigning subjects to Staff {StaffId} by User {UserId}",
                    command.StaffId, userId);

                throw;
            }
        }
    }
}