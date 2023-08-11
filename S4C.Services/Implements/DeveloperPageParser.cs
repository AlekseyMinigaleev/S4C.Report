using AngleSharp;
using AngleSharp.Dom;
using S4C.Services.Interfaces;
using System;
using System.Runtime.Intrinsics.Arm;

namespace S4C.Services.Implements
{
    public class DeveloperPageParser : IParser
    {
        public const string Url = "https://yandex.ru/games/developer?name=C4S.SHA#app=188809";

        public async Task ParseAsync()
        {
            //var config = Configuration.Default.WithDefaultLoader();
            //var context = BrowsingContext.New(config);
            //var document = await context.OpenAsync(Url);

            //var body = document.All
            //    .Where(x => x.LocalName == "body").ToArray();

            //var a = body
            //    .SelectMany(x => x.Children);

            // var b = a.Where(x => x.LocalName == "div");

            //var result = GetElementWithOneMoreChild(b.ToArray());


            var content = new StringContent("your_data_here", System.Text.Encoding.UTF8, "application/x-www-form-urlencoded");

            using (var client = new HttpClient())
            {
                var response = await client.PostAsync(Url, content);

                if (response.IsSuccessStatusCode)
                {
                    var responseBody = await response.Content.ReadAsStringAsync();
                    Console.WriteLine(responseBody);
                }
            }


        }

        public IElement[] GetElementWithOneMoreChild(IElement[] divElements)
        {
            var next = divElements
                .SelectMany(x => x.Children)
                .ToArray();

            if (next.Length > 1)
                return next;
            else
               return GetElementWithOneMoreChild(next);
        }
    }
}
