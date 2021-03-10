using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WiseduConsole.Models;

namespace WiseduConsole
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            string loginUrl = "https://aust.campusphere.net/iap/login?service=https%3A%2F%2Faust.campusphere.net%2Fportal%2Flogin";
            Dictionary<string, string> pairs = new Dictionary<string, string>();
            pairs.Add("username", "");
            pairs.Add("password", "");


            LoginProcess login = new LoginProcess(loginUrl,pairs);
            await login.Login();
        }
    }
}
