using System.Collections.Generic;


namespace P3tr0viCh.Utils
{
#pragma warning disable IDE1006
    public static class Reo
    {
        public class WeightControl
        {
            public string id { get; set; }
            public string dateBefore { get; set; }
            public string dateAfter { get; set; }
            public string registrationNumber { get; set; }
            public string garbageTruckType { get; set; }
            public string garbageTruckBrand { get; set; }
            public string garbageTruckModel { get; set; }
            public string companyName { get; set; }
            public string companyInn { get; set; }
            public string companyKpp { get; set; }
            public int weightBefore { get; set; }
            public int weightAfter { get; set; }
            public int weightDriver { get; set; }
            public int coefficient { get; set; }
            public int garbageWeight { get; set; }
            public string garbageType { get; set; }
        }

        public class Data
        {
            public Data()
            {
                weightControls = new List<WeightControl>();
            }

            public string objectId { get; set; }
            public string accessKey { get; set; }

            public List<WeightControl> weightControls { get; }
        }
    }
#pragma warning restore IDE1006
}