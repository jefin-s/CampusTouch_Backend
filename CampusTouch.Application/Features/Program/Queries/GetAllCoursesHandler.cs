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
    public class GetAllCoursesHandler:IRequestHandler<GetAllCoursesQuery,IEnumerable<AcademicProgram>>
    {

        private readonly IProgramRepository _programRepository;
        public GetAllCoursesHandler(IProgramRepository programRepository)
        {
            _programRepository = programRepository;
        }
        public async Task<IEnumerable<AcademicProgram>> Handle(GetAllCoursesQuery request, CancellationToken cancellationToken)
        {
            return (await _programRepository.GetAllAsync()).ToList();
        }
    }
}
