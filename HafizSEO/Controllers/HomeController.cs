using HafizSEO.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using HtmlAgilityPack;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;


namespace HafizSEO.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public async Task<IActionResult> Index(InputText modelInputText)
        {
            if (!string.IsNullOrEmpty(modelInputText.Text) && ModelState.IsValid)
            {
                try
                {
                    HttpClient client = new HttpClient();
                    var response = await client.GetAsync(modelInputText.Text);
                    var pageContents = await response.Content.ReadAsStringAsync();
                    HtmlDocument pageDocument = new HtmlDocument();
                    pageDocument.LoadHtml(pageContents);

                    if (modelInputText.checkWord)
                    {
                        var getPage = pageDocument.DocumentNode.InnerHtml;
                        var pageOccurrence = NumberOfOccurrence(getPage);
                        modelInputText.occurencePages = pageOccurrence;

                        var meta = new StringBuilder();
                        var getMeta = pageDocument.DocumentNode.SelectNodes("//meta");
                        foreach (var node in getMeta)
                        {
                            meta.Append(node.GetAttributeValue("content", string.Empty));
                        }
                        var metaOccurrence = NumberOfOccurrence(meta.ToString());
                        modelInputText.occurenceMetas = metaOccurrence;
                    }

                    if (modelInputText.checkLink)
                    {
                        var links = new StringBuilder();
                        var getLinks = pageDocument.DocumentNode.SelectNodes("//a[@href]");
                        foreach (var node in getLinks)
                        {
                            links.Append(node.GetAttributeValue("href", string.Empty)).Append("|");
                        }
                        var linkOccurrence = NumberOfOccurrence(links.ToString(), "link");
                        modelInputText.occurenceLinks = linkOccurrence;
                    }
                }
                catch (Exception e)
                {
                    ModelState.AddModelError("Text", "Please enter a valid URL");
                }
                
            }

            return View(modelInputText);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        private Dictionary<string, int> NumberOfOccurrence(string text, string option = null)
        {
            Dictionary<string, int> numOccur = new Dictionary<string, int>();

            if (option.Equals("link", StringComparison.OrdinalIgnoreCase) && !string.IsNullOrEmpty(option))
            {
                char[] separators = { '|' };
                string[] words = text.Split(separators, StringSplitOptions.RemoveEmptyEntries);

                Uri uri;
                foreach (var word in words)
                {
                    bool isValidLink = Uri.TryCreate(word, UriKind.Absolute, out uri) && (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps);

                    if (isValidLink && numOccur.ContainsKey(word))
                    {
                        numOccur[word] += 1;
                    }
                    else if (isValidLink && !numOccur.ContainsKey(word))
                    {
                        numOccur.Add(word, 1);
                    }
                    
                }
            }
            else
            {
                Regex matchingWord = new Regex(@"\p{L}+");
                var words = matchingWord.Matches(text).Select(x => x.Value);
                var stopwords = new StopWord().Stopwords;

                foreach (string word in words)
                {
                    string x = word.ToLower().Trim();

                    if (!stopwords.ContainsKey(x) && numOccur.ContainsKey(x))
                    {
                        numOccur[x] += 1;
                    }
                    else if (!stopwords.ContainsKey(x) && !numOccur.ContainsKey(x))
                    {
                        numOccur.Add(x, 1);
                    }
                    
                }
            }

            return numOccur;
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
