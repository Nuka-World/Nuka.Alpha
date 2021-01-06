﻿using Nuka.Core.Models;

namespace Nuka.Sample.API.Models
{
    public class SampleItemModel: BusinessModel
    {
        #region Ctor

        public SampleItemModel()
        {
            SampleType = new SampleTypeModel();
        }
        
        #endregion
        
        #region Properties
        
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public SampleTypeModel SampleType { get; set; }

        #endregion
    }
}