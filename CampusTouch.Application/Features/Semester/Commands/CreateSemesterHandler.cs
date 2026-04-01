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
    public class CreateSemesterHandler:IRequestHandler<CreateSemesterCommand,int>
    {

        private readonly ISemsterRepository _semsterRepository;
        public CreateSemesterHandler(ISemsterRepository semsterRepository)
        {
            _semsterRepository = semsterRepository;
        }
        public async Task<int> Handle(CreateSemesterCommand request, CancellationToken cancellationToken)
        {
            var semester = new Semesters
            {
                Name = request.Name,
                OrderNumber = request.orderNumber,
                CourseId = request.CourseId
            };

            return await _semsterRepository.CreateAsync(semester);
        }
    }
}
