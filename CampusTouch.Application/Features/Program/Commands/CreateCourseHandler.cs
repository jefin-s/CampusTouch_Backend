using CampusTouch.Application.Common.Exceptions;
using CampusTouch.Application.Interfaces;
using CampusTouch.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CampusTouch.Application.Features.Program.Commands
{
    public class CreateCourseHandler:IRequestHandler<CreateCourseCommand,int>
    {

        private readonly IProgramRepository _programRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IDepartementRepository _departementRepository;
       public CreateCourseHandler(IProgramRepository programRepository,ICurrentUserService currentUserService,IDepartementRepository departementRepository)
        {
            _programRepository = programRepository;
            _currentUserService = currentUserService;
            _departementRepository=departementRepository;
        }

        public async Task<int> Handle(CreateCourseCommand request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId;

            if (!_currentUserService.IsAdmin)
                throw new UnauthorizedException("Only admin can Create Programs");
            var DeptIsExistorNot = await _departementRepository.GetByIdAsync(request.DepartmentId);
            if (DeptIsExistorNot == null)
            {
                throw new NotFoundException("Departement is Not Found");
            }
            var course = new AcademicProgram
            {
                Name= request.Name,
                Level= request.Level,
                Duration= request.Duration,
                DepartmentId= request.DepartmentId,
                CreatedAt=DateTime.UtcNow,
                CreatedBy=userId

            };
            return await _programRepository.CreateAsync(course);
        }
    }
}
