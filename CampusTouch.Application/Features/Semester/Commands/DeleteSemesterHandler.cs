using CampusTouch.Application.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CampusTouch.Application.Features.Semester.Commands
{
    public  class DeleteSemesterHandler:IRequestHandler<DeleteSemesterCommand, bool>
    {
        private readonly ISemsterRepository _repository;

        public DeleteSemesterHandler(ISemsterRepository repository)
        {
            _repository = repository;
        }

        public async Task<bool> Handle(DeleteSemesterCommand request, CancellationToken cancellationToken)
        {
            return await _repository.DeleteAsync(request.Id);
        }

    }
}
