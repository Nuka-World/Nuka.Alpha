using AutoMapper;
using Nuka.Core.Mappers;
using Nuka.Sample.API.Data.Entities;
using Nuka.Sample.API.Models;

namespace Nuka.Sample.API.Mappers
{
    /// <summary>
    /// AutoMapper configuration for models
    /// </summary>
    public class MapperConfiguration : Profile, IMapperProfile
    {
        #region Ctor

        public MapperConfiguration()
        {
            CreateSampleItemMaps();
            CreateSampleTypeMaps();
        }

        #endregion

        private void CreateSampleItemMaps()
        {
            CreateMap<SampleItem, SampleItemModel>();
            CreateMap<SampleItemModel, SampleItem>();
        }

        private void CreateSampleTypeMaps()
        {
            CreateMap<SampleType, SampleTypeModel>();
            CreateMap<SampleTypeModel, SampleType>();
        }
    }
}