using System;
using Newtonsoft.Json;

namespace GitReleaseNotes.Models
{
    public class Commit
    {
        public string Sha { get; set; }
        public string Message => CommitMessage.Message;

        [JsonProperty("commit")]
        public CommitModel CommitMessage { get; set; }
        public class CommitModel
        {
            public string Message { get; set; }
            public AuthorModel Author { get; set; }

            public class AuthorModel
            {
                public DateTime Date { get; set; }
            }
        }
    }
}