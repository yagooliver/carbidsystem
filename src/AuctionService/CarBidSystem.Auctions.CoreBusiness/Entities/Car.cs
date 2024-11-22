using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CarBidSystem.Auctions.CoreBusiness.Entities
{
    public class Car(string make, string model, int year, decimal startingPrice)
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Make { get; private set; } = make;
        public string Model { get; private set; } = model;
        public int Year { get; private set; } = year;
        public decimal StartingPrice { get; private set; } = startingPrice;
        public CarState CarState { get; private set; } = CarState.Available;
        public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;

        public Auction? Auction { get; private set; }
    }
}
