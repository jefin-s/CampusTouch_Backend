using CampusTouch.Application.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace CampusTouch.Application.Features.Program.Commands
{
    public class UpdateCourseHandler:IRequestHandler<UpdateCourseCommand,int>
    {
        private readonly IProgramRepository _programRepository;

        public UpdateCourseHandler(IProgramRepository programRepository)
        {
            _programRepository = programRepository;
        }
            public async Task<int> Handle(UpdateCourseCommand request, CancellationToken cancellationToken)
        {
            var existingCourse = await _programRepository.GetByIdAsync(request.Id);
            if (existingCourse == null)
            {
                return 0;
            }
            existingCourse.Name = request.Name;
            existingCourse.Level = request.Level;
            existingCourse.Duration = request.Duration;
            existingCourse.DepartmentId = request.DepartmentId;
            existingCourse.UpdatedAt = DateTime.UtcNow;
            return await _programRepository.UpdateAsync(existingCourse);
        }
        }
    }

