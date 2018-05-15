using System.Collections.Generic;
using System.Threading.Tasks;
using GitReleaseNotes.Models;
using Refit;

namespace GitReleaseNotes
{
    [Headers("Content-Type: application/json", "Accept: application/json")]
    public interface IGitHubClient
    {
        [Get("/repos/{owner}/{repository}/commits?per_page={pageSize}&page={page}")]
        Task<List<Commit>> GetCommits([Header("Authorization")] string token, [Header("User-Agent")] string userAgent, string owner, string repository, int pageSize, int page);

        [Get("/repos/{owner}/{repository}/tags?per_page={pageSize}&page={page}")]
        Task<List<Tag>> GetTags([Header("Authorization")] string token, [Header("User-Agent")] string userAgent, string owner, string repository, int pageSize, int page);
    }
}