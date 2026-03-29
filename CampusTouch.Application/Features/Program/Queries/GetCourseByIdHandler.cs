using CampusTouch.Application.Interfaces;
using CampusTouch.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CampusTouch.Application.Features.Program.Queries
{
    public class GetCourseByIdHandler:IRequestHandler<GetCourseByIdQuery,AcademicProgram?>
    {
        private readonly IProgramRepository _programRepository;
        public GetCourseByIdHandler(IProgramRepository programRepository)
        {
            _programRepository = programRepository;
        }

        public async Task<AcademicProgram?> Handle(GetCourseByIdQuery request, CancellationToken cancellationToken)
        {
            return await _programRepository.GetByIdAsync(request.Id);
        }
    }
}
