using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            //string findCityNameUrl = string.Format("http://partners.api.skyscanner.net/apiservices/browsequotes/v1.0/UK/CNY/en-GB/BJSA-sky/CSHA-sky/2017-04-19/?apiKey=ma452197785131011345379685638992", "beijing");
            //HttpWebRequest request = WebRequest.Create(findCityNameUrl) as HttpWebRequest;
            //using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
            //{
            //    StreamReader reader = new StreamReader(response.GetResponseStream());
            //    //Console.WriteLine(reader.ReadToEnd());
            //    //Console.ReadKey();
            //    //List<Places> p = new List<Places>();

            //    string jsonContent = reader.ReadToEnd();


            //    List<Currencies> currency = new List<Currencies>();
            //    List<Quotes> quote = new List<Quotes>();
            //    List<AirPlaces> places = new List<AirPlaces>();
            //    List<Carriers> carriers = new List<Carriers>();



            //    //string resultQeruy = getData(query.fromLocation, query.toLocation, query.dateTime, query.dateTime);
            //    JObject o = JObject.Parse(jsonContent);

            //    currency = JsonConvert.DeserializeObject<List<Currencies>>(o["Currencies"].ToString());



            //    quote = JsonConvert.DeserializeObject<List<Quotes>>(o["Quotes"].ToString());
            //    JArray ja = JArray.Parse(o["Quotes"].ToString());

            //    for(int i = 0;i<ja.Count;i++)
            //    {
            //        JObject j = JObject.Parse(ja[i].ToString());

            //        OutboundLeg ob = new OutboundLeg();
            //        ob = JsonConvert.DeserializeObject<OutboundLeg>(j["OutboundLeg"].ToString());
            //        quote[i].Outboudleg=ob;


            //       string ji = ob.CarrierIds[0].ToString();
            //        //JArray jarry1 = JArray.Parse(j["OutboundLeg"].ToString());
            //    }


            //    places = JsonConvert.DeserializeObject<List<AirPlaces>>(o["Places"].ToString());

            //    carriers = JsonConvert.DeserializeObject<List<Carriers>>(o["Carriers"].ToString());



            //DateTime t = DateTime.Today;
            //Console.WriteLine(t.Year.ToString());
            //Console.WriteLine(t.Month.ToString());
            //Console.WriteLine(t.Day.ToString());
            //Console.WriteLine(t.DayOfYear);
            //Console.WriteLine(t.AddDays(1).Day);
            Chronic.Parser p = new Chronic.Parser();
            Chronic.Options op = new Chronic.Options();
            var r=p.Parse("today");
            var t =DateTime.Now.ToString("yyyy-MM-dd");


            Console.WriteLine(r.ToTime().ToString("yyyy-MM-dd"));
            Console.ReadKey();



                //JObject o = JObject.Parse(jsonContent);
                //p =JsonConvert.DeserializeObject<List<Places>>(o["Places"].ToString());

                //p[0].CityId.ToString();



                //XmlReader xr = XmlReader.Create(reader);


                //DataSet objDataSet = new DataSet();
                //objDataSet.ReadXml(reader.ReadToEnd());
            //}
        }
    }
}
