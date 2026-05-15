

using CampusTouch.Application.Features.Applicants.DTO;
using CampusTouch.Application.Interfaces;
using Dapper;
using System.Data;

namespace CampusTouch.Infrastructure.Persistance.Repositories
{
    public  class ApplicantRepository:IApplicantRepository
    {
        private readonly IDbConnection _dbConnection;
        public ApplicantRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public async Task<List<ApplicantDto>>
          GetAllApplicants()
        {
            var result =
                await _dbConnection
                    .QueryAsync<ApplicantDto>(
                        "sp_Applicant_GetAll",
                        commandType:
                            CommandType.StoredProcedure);

            return result.ToList();
        }
    }
}
