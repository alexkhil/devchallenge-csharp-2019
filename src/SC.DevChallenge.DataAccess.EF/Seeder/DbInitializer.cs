using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CsvHelper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SC.DevChallenge.DataAccess.Abstractions.Entities;
using SC.DevChallenge.DataAccess.EF.Seeder.Abstractions;
using Z.EntityFramework.Extensions;

namespace SC.DevChallenge.DataAccess.EF.Seeder
{
    public class DbInitializer : IDbInitializer
    {
        private readonly ILogger<DbInitializer> logger;
        private readonly AppDbContext dbContext;

        public DbInitializer(ILogger<DbInitializer> logger, AppDbContext dbContext)
        {
            this.logger = logger;
            this.dbContext = dbContext;
        }

        public Task InitializeAsync(string filePath)
        {
            dbContext.Database.EnsureDeleted();
            logger.LogInformation("Creating Db...");
            dbContext.Database.EnsureCreated();

            if (!File.Exists(filePath))
            {
                throw new ArgumentException("Input data file doesn't exists", nameof(filePath));
            }

            return InitializeInternalAsync(filePath);
        }

        public async Task InitializeInternalAsync(string filePath)
        {
            using (var reader = new StreamReader(filePath))
            {
                using (var csv = new CsvReader(reader))
                {
                    var rows = csv.GetRecords<DataRow>().ToList();

                    logger.LogInformation("Seeding from {file}", filePath);

                    var portfolios = rows.Select(r => r.Portfolio).Distinct().Select(p => new Portfolio { Name = p }).ToList();
                    await dbContext.BulkInsertAsync(portfolios);
                    logger.LogInformation("{Count} portfolios created", portfolios.Count);

                    var owners = rows.Select(r => r.Owner).Distinct().Select(p => new Owner { Name = p }).ToList();
                    await dbContext.BulkInsertAsync(owners);
                    logger.LogInformation("{Count} owners created", owners.Count);

                    var instruments = rows.Select(r => r.Instrument).Distinct().Select(p => new Instrument { Name = p }).ToList();
                    await dbContext.BulkInsertAsync(instruments);
                    logger.LogInformation("{Count} instruments created", instruments.Count);

                    var portfoliosMap = await dbContext.Portfolios.AsNoTracking().ToDictionaryAsync(p => p.Name, v => v.Id);
                    var ownersMap = await dbContext.Owners.AsNoTracking().ToDictionaryAsync(p => p.Name, v => v.Id);
                    var instrumentsMap = await dbContext.Instruments.AsNoTracking().ToDictionaryAsync(p => p.Name, v => v.Id);

                    dbContext.ChangeTracker.AutoDetectChangesEnabled = false;
                    
                    var ownerPortfolios = new HashSet<OwnerPortfolio>(new OwnerPortfoliosComparer());
                    var ownerInstruments = new HashSet<OwnerInstrument>(new OwnerInstrumentComparer());
                    var instrumentPrices = new HashSet<Price>();

                    foreach (var r in rows)
                    {
                        ownerPortfolios.Add(new OwnerPortfolio
                        {
                            OwnerId = ownersMap[r.Owner],
                            PortfolioId = portfoliosMap[r.Portfolio]
                        });

                        ownerInstruments.Add(new OwnerInstrument
                        {
                            OwnerId = ownersMap[r.Owner],
                            InstrumentId = instrumentsMap[r.Instrument]
                        });

                        instrumentPrices.Add(new Price
                        {
                            PortfolioId = portfoliosMap[r.Portfolio],
                            OwnerId = ownersMap[r.Owner],
                            InstrumentId = instrumentsMap[r.Instrument],
                            Date = DateTime.Parse(r.Date),
                            Value = r.Price
                        });
                    }

                    await dbContext.BulkInsertAsync(instrumentPrices);
                    logger.LogInformation("{Count} prices created", instrumentPrices.Count);

                    await dbContext.BulkInsertAsync(ownerPortfolios);
                    await dbContext.BulkInsertAsync(ownerInstruments);
                }
            }
        }
    }
}