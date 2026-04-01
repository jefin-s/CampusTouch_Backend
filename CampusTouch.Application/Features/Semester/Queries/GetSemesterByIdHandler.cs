using CampusTouch.Application.Interfaces;
using CampusTouch.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CampusTouch.Application.Features.Semester.Queries
{
    public class GetSemesterByIdHandler:IRequestHandler<GetSemesterByIdQuery, Semesters?>
    {
        private readonly ISemsterRepository _semsterRepository;
        public GetSemesterByIdHandler(ISemsterRepository semsterRepository)
        {
            _semsterRepository = semsterRepository;
        }
        public async Task<Semesters?> Handle(GetSemesterByIdQuery request, CancellationToken cancellationToken)
        {
            return await _semsterRepository.GetByIdAsync(request.Id);
        }
    }
}
