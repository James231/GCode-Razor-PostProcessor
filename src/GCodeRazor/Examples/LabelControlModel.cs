using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCodeRazor.Examples
{
    public class LabelControlModel: IExampleControlModel
    {
        [JsonProperty("type")]
        public string Type { get; } = "label";

        [JsonProperty("content")]
        public string Content { get; set; }

        [JsonProperty("top_margin")]
        public int MarginAbove { get; set; }

        [JsonProperty("bottom_margin")]
        public int MarginBelow { get; set; }
    }
}
