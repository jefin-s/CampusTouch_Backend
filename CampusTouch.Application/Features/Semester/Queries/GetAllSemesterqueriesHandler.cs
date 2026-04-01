using CampusTouch.Application.Interfaces;
using CampusTouch.Domain.Entities;
using MediatR;

namespace CampusTouch.Application.Features.Semester.Queries
{
    public class GetAllSemesterqueriesHandler:IRequestHandler<GetAllSemesterQuery,IEnumerable<Semesters>>
    {
        private readonly ISemsterRepository _semsterRepository;
        public GetAllSemesterqueriesHandler(ISemsterRepository semsterRepository)
        {
            _semsterRepository = semsterRepository;
        }
        public async Task<IEnumerable<Semesters>> Handle(GetAllSemesterQuery request, CancellationToken cancellationToken)
        {
            return await _semsterRepository.GetAllAsync();
        }
    }
}
