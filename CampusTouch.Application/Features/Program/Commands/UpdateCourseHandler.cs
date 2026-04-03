using CampusTouch.Application.Common.Exceptions;
using CampusTouch.Application.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace CampusTouch.Application.Features.Program.Commands
{
    public class UpdateCourseHandler:IRequestHandler<UpdateCourseCommand,int>
    {
        private readonly IProgramRepository _programRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IDepartementRepository _departementRepository;

        public UpdateCourseHandler(IProgramRepository programRepository,ICurrentUserService currentUserService,IDepartementRepository departementRepository)
        {
            _programRepository = programRepository;
            _currentUserService=currentUserService;
            _departementRepository=departementRepository;
        }
            public async Task<int> Handle(UpdateCourseCommand request, CancellationToken cancellationToken)
        {

            if (!_currentUserService.IsAdmin)
            {
                throw new UnauthorizedException("Only Admin can Updaet the course");
            }
            var userId = _currentUserService.UserId;
            var existingCourse = await _programRepository.GetByIdAsync(request.Id);
            if (existingCourse == null||existingCourse.IsDeleted)
            {
                throw new NotFoundException("Program is not Exist");
            }
            var existing = await _departementRepository.GetByIdAsync(request.DepartmentId);

            if(existing==null)
            {
                throw new NotFoundException("Departement is not exist");
            }
            var name = request.Name.Trim();
            existingCourse.Name = name;
            existingCourse.Level = request.Level;
            existingCourse.Duration = request.Duration;
            existingCourse.DepartmentId = request.DepartmentId;
            existingCourse.UpdatedAt = DateTime.UtcNow;
            existingCourse.UpdatedBy = userId;
            return await _programRepository.UpdateAsync(existingCourse);
        } 
        }
    }

