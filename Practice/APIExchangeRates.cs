using System;
using System.Data;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading;

namespace Practice
{
    class APIExchangeRates
    {
        static string userinput;
        static DateTime time = DateTime.Now;
        static void Main(string[] args)
        {
            while(true)
            {
                double input = getUserInput();
                double exchangeRate = SearchForGBP(GetFromApi("https://api.exchangeratesapi.io/latest"));
                
                double eurInPound = Math.Round(input * exchangeRate, 2);

                Console.WriteLine("\nIhr genannter Euro Betrag: " + input + " Euro\nDer aktuelle Kurs von Pfund: £" + exchangeRate + "\nZeitpunkt des Umrechungskurses: " + time.ToString());
                Console.WriteLine("-------------------------------------------------------------------");
                Console.WriteLine("Daraus resultieren £" + eurInPound + "\n");
            }

        }

        private static double getUserInput()
        {
            double input;
            Console.WriteLine("Bitte geben Sie einen Betrag (in Euro) an, welcher daraufhin in Pfund umgerechnet wird mit dem aktuellen Kurs: ");
            userinput = Console.ReadLine();

            // Schleife, die erst endet wenn der eingegebene Wert auch in ein Double konvertiert werden kann
            while(!Double.TryParse(userinput, out input))
            {
                Console.WriteLine("Achten Sie darauf, dass keine Buchstaben vorkommen und geben Sie den Betrag nochmal an");
                userinput = Console.ReadLine();
            }
            return input;
        }

        private static string GetFromApi(string url)
        {
            
            try
            {
                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                httpWebRequest.Method = WebRequestMethods.Http.Get; // Get Methode für den api call
                httpWebRequest.Accept = "application/json";
                string file;
                var response = (HttpWebResponse)httpWebRequest.GetResponse();
                using (var sr = new StreamReader(response.GetResponseStream()))
                {
                    file = sr.ReadToEnd();
                }
                return file;
            } 
            catch (WebException)
            {
                Console.WriteLine("Fehler bei der Verbindung zur Rest API");
                Thread.Sleep(3000);
                Environment.Exit(0);
                return "";
            }
            
        }

        private static double SearchForGBP(string json)
        {
            if(json == "")
            {
                return 0;
            }
            // string umwandeln in ein JSON object mit dem Aufbau der Klasse Exchangerates (Newtonsoft json nuget package)
            ExchangeRates rate = JsonConvert.DeserializeObject<ExchangeRates>(json);
            double exchangerate = 0.0;
            // Schleife durch das Dictionary von allen Kursen und suchen nach dem GBP Kurs. Davon die Value speichern (von dem Key value paar)
            foreach (KeyValuePair<string, double> entry in rate.rates)
            {
                if (entry.Key == "GBP")
                {
                    Double.TryParse(entry.Value.ToString(), out exchangerate);
                    time = rate.date;
                    return exchangerate;
                }
            }
            return 0;
        }

    }
    // Diese Klasse repräsentiert den JSON Aufbau
    public class ExchangeRates
    {
        public Dictionary<string, double> rates { get; set; }
        public DateTime date { get; set; }
    }
}
