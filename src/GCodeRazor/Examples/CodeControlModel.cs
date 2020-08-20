using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCodeRazor.Examples
{
    public class CodeControlModel: IExampleControlModel
    {
        [JsonProperty("type")]
        public string Type { get; } = "code";

        [JsonProperty("razor_code")]
        public string RazorCode { get; set; }

        [JsonProperty("g_code")]
        public string GCode { get; set; }

        [JsonProperty("top_margin")]
        public int MarginAbove { get; set; }

        [JsonProperty("bottom_margin")]
        public int MarginBelow { get; set; }
    }
}
