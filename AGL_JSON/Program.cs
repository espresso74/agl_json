using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Linq;
using Newtonsoft.Json;
using System.Threading;

namespace AGL_JSON
{
    class Program
    {
        
        static void Main(string[] args)
        {
            var cts = new CancellationTokenSource();
            var persons = GetPersonsAsync(cts.Token).Result;

            DisplayList(persons);
        }

        static void DisplayList(List<Person> persons)
        {
            var personGroupedThenOrdered = persons.GroupBy(p => p.Gender).
                 Select(grp => new
                 {
                     Gender = grp.Key,
                     Names = grp.Where(x => x.Pets != null).SelectMany(x => x.Pets).Where(y => y.Type == "Cat").Select(y => y.Name).OrderBy(p => p)
                 }).OrderByDescending(p => p.Gender);

            foreach (var person in personGroupedThenOrdered)
            {
                Console.WriteLine($"{person.Gender}");

                foreach (var name in person.Names)
                {
                    Console.WriteLine($"{name.PadLeft(name.Length + 3)}");
                }
            }

            Console.ReadLine();
        }

        static async Task<List<Person>> GetPersonsAsync(CancellationToken token)
        {
            List<Person> persons = null;

            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            HttpResponseMessage response = await client.GetAsync("http://agl-developer-test.azurewebsites.net/people.json");
            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadAsStringAsync();
                persons = JsonConvert.DeserializeObject<List<Person>>(data);
            }

            return persons;
        }

        static HttpClient client = new HttpClient();
    }
}
