using CampusTouch.Application.Common.Exceptions;
using CampusTouch.Application.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace CampusTouch.Application.Features.Semester.Commands
{
    public  class DeleteSemesterHandler:IRequestHandler<DeleteSemesterCommand, bool>
    {
        private readonly ISemsterRepository _repository;
        private readonly ICurrentUserService _currentUserService;

        public DeleteSemesterHandler(ISemsterRepository repository,ICurrentUserService currentUserService)
        {
            _repository = repository;
            _currentUserService = currentUserService;
        }

        public async Task<bool> Handle(DeleteSemesterCommand request, CancellationToken cancellationToken)
        {

            var userid = _currentUserService.UserId;
            if (!_currentUserService.IsAdmin)
            {
                throw new UnauthorizedException("Only Admin can Delete the  Semster");
            }

            var semster = await _repository.GetByIdAsync(request.Id);
            if (semster == null||semster.IsDeleted)
            {
                throw new NotFoundException("Semester is Not found");
            }
            return await _repository.DeleteAsync(request.Id,userid);
        }

    }
}
