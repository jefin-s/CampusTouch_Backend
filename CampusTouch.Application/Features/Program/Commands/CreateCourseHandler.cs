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
        public CreateCourseHandler(IProgramRepository programRepository)
        {
            _programRepository = programRepository;
        }

        public async Task<int> Handle(CreateCourseCommand request, CancellationToken cancellationToken)
        {
            var course = new AcademicProgram
            {
                Name= request.Name,
                Level= request.Level,
                Duration= request.Duration,
                DepartmentId= request.DepartmentId,
                CreatedAt=DateTime.UtcNow,

            };
            return await _programRepository.CreateAsync(course);
        }
    }
}
