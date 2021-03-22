using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SovChuck.Models;

namespace SovChuck.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private static ObservableCollection<People> Peoples { get; set; }

        public static string HttpWebRequest(string method, string url)
        {
            WebRequest request = WebRequest.Create(url);
            request.Method = method;

            try
            {
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                string streamResponses = string.Empty;
                using (Stream dataStream = response.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(dataStream);

                    streamResponses = reader.ReadToEnd();
                }

                response.Close();

                return streamResponses;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index(string search, string category)
        {

            ViewBag.Joke = GetJokeFromCategory(category);
            ViewBag.SearchedJoke = JokeFromSearch(search);
            ViewBag.GetPeople = GetPeoples();
            ViewBag.GetCategories = GetCategories();

            return View();
        }


        // Get random joke from a specific Chuck Norris API
        public static string GetJokeFromCategory(string category)
        {
            Joke joke = new Joke();
            string Url = "https://api.chucknorris.io/jokes/random?category=" + category;

            try
            {
                string stringResult = HttpWebRequest("GET", Url);
                joke = JsonConvert.DeserializeObject<Joke>(stringResult);

                return joke.Value;
            }
            catch (Exception ex)
            {
                return null; 
            }
        }

        // Get list of Chuck norris API Categories
        public List<string> GetCategories()
        {
            string Url = "https://api.chucknorris.io/jokes/categories";
            try
            {
                string stringResult = HttpWebRequest("GET", Url);
                List<string> chuckCategories = JsonConvert.DeserializeObject<List<string>>(stringResult);

                return chuckCategories;
            }
            catch (Exception ex)
            {
                //throw new Exception(ex.Message);
                return null;
            }

        }

        // Get Chuck norris Jokes based on search Query
        public static string JokeFromSearch(string search)
        {
            JokesResult jokeResult = new JokesResult();
            string Url = "https://api.chucknorris.io/jokes/search?query=" + search;

            try
            {
                string stringResult = HttpWebRequest("GET", Url);
                jokeResult = JsonConvert.DeserializeObject<JokesResult>(stringResult);

                Random randomNumber = new Random();
                string randomJokeFromList = jokeResult.result[randomNumber.Next(0, jokeResult.result.Count)].Value;
                return randomJokeFromList;
            }
            catch (Exception ex)
            {
                //throw new Exception("An error occured", ex.Message);
                return null;
            }
        }

        // Get star wars people
        public static ObservableCollection<People> GetPeoples()
        {
            string Url = "https://swapi.dev/api/people/";
            Peoples = new ObservableCollection<People>();
            try
            {
                string stringResult = HttpWebRequest("GET", Url);

                if (!string.IsNullOrEmpty(stringResult))
                {
                    var data = JsonConvert.DeserializeObject<RootObject>(stringResult);
                    foreach (var people in data.Peoples)
                    {
                        Peoples.Add(people);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("An error occured.", ex);
            }
            return Peoples;
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
