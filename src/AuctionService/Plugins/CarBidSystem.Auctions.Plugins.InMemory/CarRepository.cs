using CarBidSystem.Auctions.CoreBusiness.Entities;
using CarBidSystem.Auctions.CoreBusiness.Interfaces;

namespace CarBidSystem.Auctions.Plugins.InMemory
{
    public class CarRepository : ICarRepository
    {
        private readonly List<Car> cars =
        [
            new("Toyota", "Corolla", 2020, 15000m),
            new("Honda", "Civic", 2019, 16000m),
            new("Ford", "Mustang", 2021, 30000m),
            new("Chevrolet", "Camaro", 2022, 35000m),
            new("Tesla", "Model 3", 2023, 45000m),
            new("BMW", "3 Series", 2018, 25000m),
            new("Mercedes-Benz", "C-Class", 2020, 40000m),
            new("Audi", "A4", 2021, 38000m),
            new("Volkswagen", "Passat", 2019, 20000m),
            new("Hyundai", "Elantra", 2022, 18000m)
        ];
        public Task<List<Car>> GetAllAsync()
        {
            return Task.FromResult(cars);
        }

        public Task<Car?> GetByIdAsync(int id)
        {
            return Task.FromResult(cars.FirstOrDefault(x => x.Id == id));
        }
    }
}
