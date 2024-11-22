using CarBidSystem.Auctions.CoreBusiness.Entities;

namespace CarBidSystem.Auctions.CoreBusiness.Interfaces
{
    public interface ICarRepository
    {
        Task<List<Car>> GetAllAsync();
        Task<Car?> GetByIdAsync(int id);
    }
}
