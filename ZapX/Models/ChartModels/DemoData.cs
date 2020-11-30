using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ZapX.Models.ChartModels
{
    public class DemoData
    {
        public string Name { get; set; }

        public int Count { get; set; }

    }

    public class DonutChart
    {
        public string Label { get; set; }
        public int Value { get; set; }
    }
}
