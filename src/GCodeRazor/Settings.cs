using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Automation.Peers;

namespace GCodeRazor
{
    public class Settings
    {
        [JsonProperty("open_file_when_generated")]
        public bool OpenFile { get; set; }

        [JsonProperty("trim_output")]
        public bool TrimOutput { get; set; }

        [JsonProperty("files_to_concat")]
        public string[] FilesToConcat { get; set; }

        [JsonProperty("font_size")]
        public int FontSize { get; set; }

        [JsonProperty("line_numbers")]
        public bool LineNumbers { get; set; }

        [JsonProperty("word_wrap")]
        public bool WordWrap { get; set; }

        [JsonProperty("ask_before_closing")]
        public bool AskBeforeClosing { get; set; }

        public void SetDefaults()
        {
            OpenFile = false;
            TrimOutput = true;
            FilesToConcat = new string[0];
            FontSize = 20;
            LineNumbers = true;
            WordWrap = false;
            AskBeforeClosing = true;
        }
    }
}
