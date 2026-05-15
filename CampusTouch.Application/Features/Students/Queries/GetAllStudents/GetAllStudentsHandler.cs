
using CampusTouch.Application.Features.Students.DTOs;
using CampusTouch.Application.Interfaces;
using MediatR;

namespace CampusTouch.Application.Features.Students.Queries.GetAllStudents
{
    public class GetAllStudentsHandler
        : IRequestHandler<
            GetAllStudentsQuery,
            PaginatedResult<StudentResponseDTO>>
    {
        private readonly IStudentRepository _StudentRepository;
        private readonly ICacheService _CacheService;

        public GetAllStudentsHandler(
            IStudentRepository studentRepository,
            ICacheService cacheService)
        {
            _StudentRepository = studentRepository;
            _CacheService = cacheService;
        }

        public async Task<PaginatedResult<StudentResponseDTO>> Handle(
            GetAllStudentsQuery request,
            CancellationToken token)
        {
            var cacheKey =
                $"students:{request.pageNumber}:{request.pageSize}:{request.Search}";

            // 1️⃣ Check Cache
            var cached =
                await _CacheService.GetAsync<PaginatedResult<StudentResponseDTO>>(cacheKey);

            if (cached != null)
                return cached;

            // 2️⃣ Fetch Repository Data
            var (students, totalCount) =
                await _StudentRepository.GetAllStudents(
                    request.pageNumber,
                    request.pageSize,
                    request.Search
                );

            // 3️⃣ Map DTO
            var mappedStudents = students.Select(s => new StudentResponseDTO
            {
                Id = s.Id,
                AdmissionNumber = s.AdmissionNumber,
                FullName = s.FirstName + " " + s.LastName,
                Email = s.Email,
                IsActive = s.IsActive,
                ProfileImageUrl = s.ProfileImageUrl
            }).ToList();

            // 4️⃣ Create Paginated Result
            var result = new PaginatedResult<StudentResponseDTO>
            {
                Items = mappedStudents,
                TotalCount = totalCount,
                PageNumber = request.pageNumber,
                PageSize = request.pageSize
            };

            // 5️⃣ Save Cache
            await _CacheService.SetAsync(cacheKey, result, 2);

            return result;
        }
    }
}
//using CampusTouch.Application.Features.Students.DTOs;
//using CampusTouch.Application.Interfaces;
//using MediatR;

//namespace CampusTouch.Application.Features.Students.Queries.GetAllStudents
//{
//    public class GetAllStudentsHandler
//        : IRequestHandler<
//            GetAllStudentsQuery,
//            PaginatedResult<StudentResponseDTO>>
//    {
//        private readonly IStudentRepository _StudentRepository;

//        public GetAllStudentsHandler(
//            IStudentRepository studentRepository)
//        {
//            _StudentRepository = studentRepository;
//        }

//        public async Task<PaginatedResult<StudentResponseDTO>> Handle(
//            GetAllStudentsQuery request,
//            CancellationToken token)
//        {
//            // Fetch paginated data from repository
//            var (students, totalCount) =
//                await _StudentRepository.GetAllStudents(
//                    request.pageNumber,
//                    request.pageSize,
//                    request.Search
//                );

//            // Map entity to DTO
//            var mappedStudents = students.Select(s => new StudentResponseDTO
//            {
//                Id = s.Id,
//                AdmissionNumber = s.AdmissionNumber,
//                FullName = s.FirstName + " " + s.LastName,
//                Email = s.Email,
//                IsActive = s.IsActive,
//                ProfileImageUrl = s.ProfileImageUrl
//            }).ToList();

//            // Return paginated result
//            return new PaginatedResult<StudentResponseDTO>
//            {
//                Items = mappedStudents,
//                TotalCount = totalCount,
//                PageNumber = request.pageNumber,
//                PageSize = request.pageSize
//            };
//        }
//    }
//}