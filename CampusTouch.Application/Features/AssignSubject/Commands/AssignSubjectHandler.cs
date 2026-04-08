using CampusTouch.Application.Common.Exceptions;
using CampusTouch.Application.Interfaces;
using CampusTouch.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace CampusTouch.Application.Features.AssignSubject.Commands
{
    public  class AssignSubjectHandler:IRequestHandler<AssignSubjectCommand,bool>
    {
        private readonly IStaffRepository _staffRepository;
        private readonly ISubjectRepository _subjectRepository;
        private  readonly ICurrentUserService _currentUserService;
        private readonly IStaffSubjectRepository _staffSubjectRepository;
        public AssignSubjectHandler(ISubjectRepository subjectRepository,IStaffRepository staffRepository, IStaffSubjectRepository staffSubjectRepository,ICurrentUserService currentUserService)
        {
            _staffRepository = staffRepository;
            _subjectRepository = subjectRepository;
            _staffSubjectRepository = staffSubjectRepository;
            _currentUserService = currentUserService;
            
        }
        public async Task <bool> Handle(AssignSubjectCommand command,CancellationToken  cancellationToken)
            
        {
            if (!_currentUserService.IsAdmin)
                throw new UnauthorizedException("Only admin can assign the subject to staff");
            var staff = await _staffRepository.GetStaffById(command.StaffId);
            if(staff == null)
            {
                throw new NotFoundException("Staff not found");
            }
            var subject = await _subjectRepository.GetByIds(command.SubjectIds);

            if (subject.Count != command.SubjectIds.Count)
                throw new NotFoundException("One or more subjects not found");
            await _staffSubjectRepository.RemoveAllByStaffId(command.StaffId);

            // 🔥 4. Add new mappings
            foreach (var subjectId in command.SubjectIds)
            {
                var staffSubject = new StaffSubject
                {
                    StaffId = command.StaffId,
                    SubjectId = subjectId,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy=_currentUserService.UserId

                };

                await _staffSubjectRepository.AddAsync(staffSubject);
            }

            return true;
        }

    }
}
