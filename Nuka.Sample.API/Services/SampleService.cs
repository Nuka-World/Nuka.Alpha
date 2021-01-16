using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Nuka.Core.Data.Repositories;
using Nuka.Sample.API.Data.Entities;

namespace Nuka.Sample.API.Services
{
    public class SampleService
    {
        #region Fields

        private readonly IRepository<SampleItem> _sampleItemRepository;
        private readonly IRepository<SampleType> _sampleTypeRepository;

        #endregion

        #region Ctor

        public SampleService(
            IRepository<SampleItem> sampleItemRepository,
            IRepository<SampleType> sampleTypeRepository)
        {
            _sampleItemRepository = sampleItemRepository;
            _sampleTypeRepository = sampleTypeRepository;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Inserts a sample item
        /// </summary>
        /// <param name="id">ID</param>
        public SampleItem GetItemById(int id)
        {
            if (id == 0)
                return null;

            // Get Item By Id
            var query = _sampleItemRepository.Table;
            query = query.Include(@class => @class.SampleType);

            return query.SingleOrDefault(item => item.Id == id);
        }

        /// <summary>
        /// Inserts a sample item
        /// </summary>
        /// <param name="item">Sample Item</param>
        public void Insert(SampleItem item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            //insert
            _sampleItemRepository.Insert(item);
        }

        #endregion
    }
}