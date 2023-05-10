using System.Text.Json.Serialization;

namespace HPAExample.Model
{
    //public class JokeModel
    //{
    //    //[JsonPropertyName("id")]
    //    //public string Id { get; set; }
    //    //[JsonPropertyName("joke")]
    //    //public string Joke { get; set; }
    //    //[JsonPropertyName("status")]
    //    //public int Status { get; set; }

    //    public string type { get; set; }

    //    public string setup { get; set; }
    //    public int id { get; set; }
    //    public string punchline { get; set; }



    //}

    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
    public class Bpi
    {
        public USD USD { get; set; }
        public GBP GBP { get; set; }
        public EUR EUR { get; set; }
    }

    public class EUR
    {
        public string code { get; set; }
        public string symbol { get; set; }
        public string rate { get; set; }
        public string description { get; set; }
        public double rate_float { get; set; }
    }

    public class GBP
    {
        public string code { get; set; }
        public string symbol { get; set; }
        public string rate { get; set; }
        public string description { get; set; }
        public double rate_float { get; set; }
    }

    public class JokeModel
    {
        public Time time { get; set; }
        public string disclaimer { get; set; }
        public string chartName { get; set; }
        public Bpi bpi { get; set; }
    }

    public class Time
    {
        public string updated { get; set; }
        public DateTime updatedISO { get; set; }
        public string updateduk { get; set; }
    }

    public class USD
    {
        public string code { get; set; }
        public string symbol { get; set; }
        public string rate { get; set; }
        public string description { get; set; }
        public double rate_float { get; set; }
    }

}
