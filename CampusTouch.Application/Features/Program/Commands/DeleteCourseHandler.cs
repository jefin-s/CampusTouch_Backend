using CampusTouch.Application.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CampusTouch.Application.Features.Program.Commands
{
    public class DeleteCourseHandler : IRequestHandler<DeleteCourseCommand, int>
    {
        private readonly IProgramRepository _programRepository;

        public DeleteCourseHandler(IProgramRepository programRepository)
        {
            _programRepository = programRepository;
        }

        public async Task<int> Handle(DeleteCourseCommand request, CancellationToken cancellationToken)
        {
            return await _programRepository.DeleteAsync(request.Id);
        }
    }
}
