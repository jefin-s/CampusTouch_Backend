using CampusTouch.Application.Interfaces;
using CampusTouch.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CampusTouch.Application.Features.Semester.Commands
{
    public class UpdateSemesterHandler:IRequestHandler<UpdateSemesterCommand,bool>
    {
        private readonly ISemsterRepository _repository;
        public UpdateSemesterHandler(ISemsterRepository repository)
        {
            _repository = repository;
        }
        public async Task<bool> Handle(UpdateSemesterCommand request, CancellationToken cancellationToken)
        {
            var semester = new Semesters
            {
                Id = request.Id,
                Name = request.Name,
                OrderNumber = request.OrderNumber,
                CourseId = request.CourseId
            };

            return await _repository.UpdateAsync(semester);
        }
    }
}
