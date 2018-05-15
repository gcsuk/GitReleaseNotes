using Newtonsoft.Json;

namespace GitReleaseNotes.Models
{
    public class Tag
    {
        public string Name { get; set; }
        public string Sha => CommitMessage.Sha;

        [JsonProperty("commit")]
        public CommitModel CommitMessage { get; set; }
        public class CommitModel
        {
            public string Sha { get; set; }
        }
    }
}
