﻿using CarBidSystem.Bids.CoreBusiness.Entities;
using CarBidSystem.Bids.CoreBusiness.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CarBidSystem.Bids.Plugins.EFCoreSqlServer.Repositories
{
    public class BidRepository(IDbContextFactory<BidDbContext> contextFactory) : IBidRepository
    {
        private readonly IDbContextFactory<BidDbContext> contextFactory = contextFactory;

        public async Task AddAsync(Bid bid)
        {
            using var db = contextFactory.CreateDbContext();
            db.Bids?.Add(bid);
            await db.SaveChangesAsync();
        }

        public async Task<List<Bid>?> GetAllBidsAsync()
        {
            using var db = contextFactory.CreateDbContext();
            return await (db.Bids?.ToListAsync() ?? Task.FromResult(new List<Bid>()));
        }

        public async Task<List<Bid>?> GetAllBidsByAuctionIdAsync(int auctionId)
        {
            using var db = contextFactory.CreateDbContext();
            return await (db.Bids?.Where(x => x.AuctionId == auctionId).ToListAsync() ?? Task.FromResult(new List<Bid>()));
        }

        public async Task<Bid?> GetByIdAsync(int id)
        {
            using var db = contextFactory.CreateDbContext();
            return await (db.Bids?.FirstOrDefaultAsync(x => x.Id == id) ?? Task.FromResult<Bid?>(null));
        }

        public async Task<(List<Bid>, int)> GetPaginatedBidsByAuctionIdAsync(int auctionId, int pageNumber, int pageSize, CancellationToken cancellationToken)
        {
            using var db = contextFactory.CreateDbContext();

            var totalRecords = await db.Bids.Where(x => x.AuctionId == auctionId).CountAsync(cancellationToken);

            var bids = await db.Bids
                .Where(x => x.AuctionId == auctionId)
                .OrderBy(x => x.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            return (bids, totalRecords);
        }
    }
}
