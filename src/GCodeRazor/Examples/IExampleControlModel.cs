using JsonSubTypes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCodeRazor.Examples
{
    [JsonConverter(typeof(JsonSubtypes), "Type")]
    [JsonSubtypes.KnownSubType(typeof(CardControlModel), "card")]
    [JsonSubtypes.KnownSubType(typeof(CodeControlModel), "code")]
    [JsonSubtypes.KnownSubType(typeof(LabelControlModel), "label")]
    [JsonSubtypes.KnownSubType(typeof(HeadingControlModel), "heading")]
    [JsonSubtypes.KnownSubType(typeof(SubHeadingControlModel), "subheading")]
    public interface IExampleControlModel
    {
        [JsonProperty("type")]
        string Type { get; }
    }
}
