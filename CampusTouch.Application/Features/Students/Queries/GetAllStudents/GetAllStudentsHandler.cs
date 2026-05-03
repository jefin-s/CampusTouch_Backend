using CampusTouch.Application.Features.Students.DTOs;
using CampusTouch.Application.Interfaces;
using CampusTouch.Domain.Entities;
using MediatR;


namespace CampusTouch.Application.Features.Students.Queries.GetAllStudents
{
    public class GetAllStudentsHandler:IRequestHandler<GetAllStudentsQuery,IEnumerable<StudentResponseDTO>>
    {

        private readonly IStudentRepository _StudentRepository;
        private readonly ICacheService _CacheService;
        public GetAllStudentsHandler(IStudentRepository studentRepository, ICacheService cacheService)
        {
            _StudentRepository = studentRepository;
            _CacheService = cacheService;
        }

        public async Task <IEnumerable<StudentResponseDTO>> Handle(GetAllStudentsQuery request,CancellationToken token)
        {

            var cacheKey = $"students:{request.pageNumber}:{request.pageSize}:{request.Search}";

            // 1️⃣ Check cache
            var cached = await _CacheService.GetAsync<List<StudentResponseDTO>>(cacheKey);
            if (cached != null)
                return cached;

            var students= await _StudentRepository.GetAllStudents(request.pageNumber,request.pageSize,request.Search);

            var result = students.Select(s => new StudentResponseDTO
            {
               Id= s.Id,
               AdmissionNumber= s.AdmissionNumber,
               FullName=s.FirstName+""+s.LastName,
               Email=s.Email,
               IsActive=s.IsActive,
               ProfileImageUrl=s.ProfileImageUrl
            }).ToList();
            await _CacheService.SetAsync(cacheKey, result, 2);

            return result;
        }
    }
}
