namespace Nuka.Sample.HttpAggregator.Models
{
    public class SampleItemModel
    {
        public int Id { get; set; }
        public string ItemId { get; set; }
        public string ItemName { get; set; }
        public string Description { get; set; }
        public double Price { get; set; }
        public SampleTypeModel SampleType { get; set; }
    }
    
    public class SampleTypeModel
    {
        public int Id { get; set; }
        public string Type { get; set; }
    }
}