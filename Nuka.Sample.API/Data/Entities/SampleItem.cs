﻿using Nuka.Core.Data.Entities;

namespace Nuka.Sample.API.Data.Entities
{
    public class SampleItem : BusinessEntity
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int SampleTypeId { get; set; }
        public SampleType SampleType { get; set; }
    }
}