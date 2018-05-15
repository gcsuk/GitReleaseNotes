using System.Collections.Generic;
using System.Text;

namespace GitReleaseNotes.Models
{
    public class ReleaseNotes : List<ReleaseNote>
    {
        public string ToHtmlString()
        {
            var result = new StringBuilder("<ul class=\"release-notes\">");

            foreach (var releaseNote in this)
            {
                result.Append("<li>");
                result.Append($"<p>{releaseNote.Version}</p>");
                result.Append("<div>");

                foreach (var note in releaseNote.Notes)
                {
                    result.Append($"{note.Replace("\n", "<br />")}<br />");
                }

                result.Append("</div>");
                result.Append("</li>");
            }

            result.Append("</ul>");

            return result.ToString();
        }
    }
}