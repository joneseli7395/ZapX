using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ZapX.Models.ChartModels
{
    public class ChartJSModel
    {
        public ChartJSModel()
        {
            Labels = new List<string>();
            Data = new List<int>();
            BackgroundColor = new List<string>();
        }

        public List<string> Labels { get; set; }

        public List<int> Data { get; set; }

        public List<string> BackgroundColor { get; set; }

    }
}
