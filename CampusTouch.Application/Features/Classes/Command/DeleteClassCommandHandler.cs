using CampusTouch.Application.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CampusTouch.Application.Features.Classes.Command
{
    public class DeleteClassCommandHandler:IRequestHandler<DeleteClassCommand,int>
    {
        private readonly IClassesRepository _repo;
        private readonly ICurrentUserService _currentUser;
        public DeleteClassCommandHandler(IClassesRepository repo, ICurrentUserService currentUser)
        {
            _repo = repo;
            _currentUser = currentUser;
        }
        public async Task<int> Handle(DeleteClassCommand request, CancellationToken cancellationToken)
        {
            if (!_currentUser.IsAdmin)
            {
                throw new UnauthorizedAccessException("Only admins can delete classes");
            }
            var existingClass = await _repo.GetByIdAsync(request.Id);
            if (existingClass == null)
            {
                throw new DirectoryNotFoundException("Class not found");
            }
            existingClass.IsActive = false;
            await _repo.DeleteAsync(request.Id);
            return request.Id;
        }
    }
}
