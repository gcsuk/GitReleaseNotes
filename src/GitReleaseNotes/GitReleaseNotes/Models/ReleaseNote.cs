using System.Collections.Generic;
using Newtonsoft.Json;

namespace GitReleaseNotes.Models
{
    public class ReleaseNote
    {
        public ReleaseNote()
        {
            Notes = new List<string>();
        }

        [JsonProperty("version")]
        public string Version { get; set; }
        [JsonProperty("releaseDate")]
        public string ReleaseDate { get; set; }
        [JsonProperty("notes")]
        public List<string> Notes { get; set; }
    }
}