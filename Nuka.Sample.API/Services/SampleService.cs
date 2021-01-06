using System;
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
        /// <param name="item">Sample Item</param>
        /// <param name="type">Sample Type</param>
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