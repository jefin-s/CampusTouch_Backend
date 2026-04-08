using CampusTouch.Application.Common.Exceptions;
using CampusTouch.Application.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace CampusTouch.Application.Features.Staffs.Commands
{
    public class DeletStaffCommandHandler : IRequestHandler<DeleteStafffCommand, bool>
    {
        private readonly IStaffRepository _staffRepository;
        private readonly ICurrentUserService _currentUserService;

        public DeletStaffCommandHandler(IStaffRepository staffRepository, ICurrentUserService currentUserService)
        {
            _staffRepository = staffRepository;
            _currentUserService = currentUserService;

        }

        public async Task<bool> Handle(DeleteStafffCommand request, CancellationToken cancellationToken)
        {
            if (!_currentUserService.IsAdmin)
                throw new UnauthorizedAccessException("Only admin can delete staff.");

            var staff = await _staffRepository.GetStaffById(request.Id);
            if (staff == null)
                throw new NotFoundException("Staff not found.");
             staff.IsActive=false;
            staff.DeletedAt = DateTime.UtcNow;
             staff.DeletedBy = _currentUserService.UserId;

            var count = await _staffRepository.DeactivateStaff(staff.Id);
            return count > 0;   
        }
    }
}
