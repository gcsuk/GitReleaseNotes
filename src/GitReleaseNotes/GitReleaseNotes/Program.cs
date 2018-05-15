using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using GitReleaseNotes.Models;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Refit;

namespace GitReleaseNotes
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

            var configuration = builder.Build();

            var username = configuration["Credentials:Username"];
            var password = configuration["Credentials:Password"];
            var owner = configuration["Repository:Owner"];
            var repository = configuration["Repository:Name"];

            if (string.IsNullOrEmpty(username))
            {
                Console.Write("Username: ");
                username = Console.ReadLine();
            }

            if (string.IsNullOrEmpty(password))
            {
                Console.Write("Password: ");
                password = Console.ReadLine();
            }

            if (string.IsNullOrEmpty(owner))
            {
                Console.Write("Repository Owner: ");
                owner = Console.ReadLine();
            }

            if (string.IsNullOrEmpty(repository))
            {
                Console.Write("Repository Name: ");
                repository = Console.ReadLine();
            }

            var token = "Basic " + Convert.ToBase64String(System.Text.Encoding.GetEncoding("ISO-8859-1").GetBytes($"{username}:{password}"));

            var gitHubClient = RestService.For<IGitHubClient>("https://api.github.com/");

            var commits = new List<Commit>();
            var tags = new List<Tag>();

            // Get commits until results no longer returned, adding to list
            for (var i = 1; i < 1000; i++)
            {
                try
                {
                    Console.Clear();
                    Console.WriteLine($"Processing {i * 100} Commits");

                    var pageCommits = await gitHubClient.GetCommits(token, repository, owner, repository, 100, i);

                    if (pageCommits.Any())
                        commits.AddRange(pageCommits);
                    else
                        break;
                }
                catch (ApiException ex)
                {
                    switch (ex.StatusCode)
                    {
                        case HttpStatusCode.Unauthorized:
                            Console.WriteLine("Invalid Git credentials.");
                            break;
                        case HttpStatusCode.NotFound:
                            Console.WriteLine("Unable to load repository, check owner and name are correct.");
                            break;
                        default:
                            Console.WriteLine($"Unexpected error contacting Git. {ex.Content}");
                            break;
                    }

                    Console.WriteLine("Press Enter to restart the application and try again.");
                    Console.ReadLine();
                    return;
                }
            }

            commits.Reverse();

            // Get tags until results no longer returned, adding to list
            for (var i = 1; i < 1000; i++)
            {
                var pageTags = await gitHubClient.GetTags(token, repository, owner, repository, 100, i);

                if (pageTags.Any())
                    tags.AddRange(pageTags);
                else
                    break;
            }

            if (!tags.Any())
            {
                Console.WriteLine("You have no tags in your repository. Please ensure that you tag your releases.");
                Console.WriteLine("Press Enter to close the application.");
                Console.ReadLine();
                return;
            }

            var releaseNotes = new ReleaseNotes();
            var releaseNote = new ReleaseNote();

            foreach (var commit in commits)
            {
                var tag = tags.SingleOrDefault(t => t.Sha == commit.Sha);

                if (tag != null)
                {
                    releaseNote = new ReleaseNote
                    {
                        Version = tag.Name,
                        ReleaseDate = commit.CommitMessage.Author.Date.ToShortDateString(),
                        Notes = new List<string> { commit.Message }
                    };

                    releaseNotes.Add(releaseNote);
                }
                else
                {
                    releaseNote.Notes.Add(commit.Message);
                }
            }

            releaseNotes.Reverse();

            // JSON Output
            var json = JsonConvert.SerializeObject(releaseNotes);

            File.WriteAllText("releaseNotes.json", json);
            Console.WriteLine(json);

            Console.WriteLine();
            Console.WriteLine("Done. Press Enter to exit.");
            Console.ReadLine();
        }
    }
}
