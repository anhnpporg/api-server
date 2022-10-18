using System.Collections.Generic;
using System.Threading.Tasks;
using UtNhanDrug_BE.Models.ConsignmentModel;

namespace UtNhanDrug_BE.Services.ConsignmentService
{
    public interface IConsignmentSvc
    {
        Task<bool> CreateConsignment(int userId, CreateConsignmentModel model);
        Task<bool> UpdateConsignment(int id, int userId, UpdateConsignmentModel model);
        Task<bool> DeleteConsignment(int id, int userId);
        Task<ViewConsignmentModel> GetConsignmentById(int id);
        Task<List<ViewConsignmentModel>> GetAllConsignment();
        Task<bool> CheckConsignment(int id);
    }
}
