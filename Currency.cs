using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Bitcoin
{

    //[JsonObject(MemberSerialization.OptIn)]
    public class Currency
    {
      //  [JsonProperty("code")]
        public string Code { get; set; }
      //  [JsonProperty("name")]
        public string Name { get; set; }
        //[JsonProperty("statuses")]
        public IEnumerable<string> Statuses { get; set; }
    }


    //[JsonObject(MemberSerialization.OptIn)]
    public class CurrencyList
    {
      //  [JsonProperty("row")]
        List<Currency> CurList { get; set;}

        //public void Add(Currency cur){
        //    CurList.ad
        //}
    }

    public class Ticker
    {
        [JsonProperty("base")]
        public string basis {get; set;}
        public string target { get; set; }
        public double price { get; set; }
        public string volume { get; set; }
        public string change { get; set; }

    }

    public class Course
    { 
        Ticker Tiker {get; set;}
        string timestamp{get; set;}
        string success	{get; set;}
        string error {get; set;}

    }

    public class Presettings
    {
        public string p1 { get; set; }
        public string p2 { get; set; }
        public string p3 { get; set; }
        public string p4 { get; set; }
        public string p5 { get; set; }
    }
   
}
