using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Nuka.Core.Data.Repositories;
using Nuka.Sample.API.Data;
using Nuka.Sample.API.Data.Entities;
using Nuka.Sample.API.Services;
using Xunit;

namespace Nuka.Sample.API.Unit.Tests.Services
{
    public class SampleServiceTest
    {
        private readonly DbContextOptions<SampleDbContext> _dbOptions;

        public SampleServiceTest()
        {
            _dbOptions = new DbContextOptionsBuilder<SampleDbContext>()
                .UseInMemoryDatabase(databaseName: "in-memory")
                .Options;

            using var dbContext = new SampleDbContext(_dbOptions);
            dbContext.AddRange(GetFakeSampleItems());
            dbContext.SaveChanges();
        }

        [Fact(DisplayName = "Get Sample Items Successfully")]
        public void GetSampleItems_ShouldSuccess()
        {
            var sampleDbContext = new SampleDbContext(_dbOptions);
            var sampleRepository = new CommonRepository<SampleItem>(sampleDbContext);
            var sampleService = new SampleService(sampleRepository, null);

            var sampleItem = sampleService.GetItemById(1);

            Assert.Equal(sampleItem.ItemId, GetFakeSampleItems()[0].ItemId);
        }

        private List<SampleItem> GetFakeSampleItems()
        {
            return new List<SampleItem>
            {
                new SampleItem
                {
                    Id = 1,
                    ItemId = "00001",
                    ItemName = "Item01",
                    Description = "Desciption01",
                    Price = 10.01m,
                    SampleType = new SampleType() {Type = "Sample"}
                }
            };
        }
    }
}