using System;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using Microsoft.Bot.Builder.Luis;
using System.Collections.Generic;
using Microsoft.Bot.Builder.Luis.Models;
using Newtonsoft.Json;
using System.Net.Http;
using System.Net;
using System.IO;
using System.Xml;
using Newtonsoft.Json.Linq;
using BookFlight.Model;
using Microsoft.Bot.Builder.Internals.Fibers;

namespace BookFlight.Dialogs
{
    [LuisModel("24f74546-ee76-47c6-a37a-8d74f78615ca", "853678ead1b4402daba2a5aec96fae4b")]
    [Serializable]
    public class RootDialog : LuisDialog<string>
    {
        private static Dictionary<string, string> queryString = new Dictionary<string, string>();
        string text;
        public const string DefaultQueryWhat = "default";

        public void TryFindQuery(IDialogContext context,LuisResult result, out Query query)
        {
            query = new Query();

            //string what;
            EntityRecommendation title;
            foreach (EntityRecommendation e in result.Entities)
            {
                if (result.TryFindEntity(Entity_Query_ToLocation, out title))
                {
                   
                    query.toLocation = title.Entity;                        
                }
                if (result.TryFindEntity(Entity_Query_FromLocation, out title))
                {
                    query.fromLocation = title.Entity;
                }
                if (result.TryFindEntity(Entity_Query_Date, out title))
                {
                    query.dateTime = title.Entity;
                }
            }

            //return query;
            //return this.queryString.TryGetValue(what, out query);
        }
        public const string Entity_Query_ToLocation = "Location::ToLocation";
        public const string Entity_Query_FromLocation = "Location::FromLocation";
        public const string Entity_Query_Date = "builtin.datetime.date";

        [LuisIntent("BookFlight")]
        public async Task FindQuery(IDialogContext context, LuisResult result)
        {
            Query query;
            TryFindQuery(context,result, out query);

            Chronic.Parser parser = new Chronic.Parser();
            var dateResult = parser.Parse(query.dateTime);


            List<Currencies> currency = new List<Currencies>();
            List<Quotes> quote = new List<Quotes>();
            List<AirPlaces> places = new List<AirPlaces>();
            List<Carriers> carriers = new List<Carriers>();

            string resultQeruy = getData(query.fromLocation, query.toLocation, dateResult.ToTime().ToString("yyyy-MM-dd"), context);


            //await context.PostAsync(resultQeruy);


            string re_Url = getRedirect(query.fromLocation, query.toLocation, dateResult.ToTime().ToString("yyyy-MM-dd"));

            JObject o = JObject.Parse(resultQeruy);

            currency = JsonConvert.DeserializeObject<List<Currencies>>(o["Currencies"].ToString());

            quote = JsonConvert.DeserializeObject<List<Quotes>>(o["Quotes"].ToString());

            JArray ja = JArray.Parse(o["Quotes"].ToString());

            for (int i = 0; i < ja.Count; i++)
            {
                JObject j = JObject.Parse(ja[i].ToString());

                OutboundLeg ob = new OutboundLeg();
                ob = JsonConvert.DeserializeObject<OutboundLeg>(j["OutboundLeg"].ToString());
                quote[i].Outboudleg = ob;
                //JArray jarry1 = JArray.Parse(j["OutboundLeg"].ToString());
            }


            places = JsonConvert.DeserializeObject<List<AirPlaces>>(o["Places"].ToString());

            carriers = JsonConvert.DeserializeObject<List<Carriers>>(o["Carriers"].ToString());


            var incomingMessage = context.MakeMessage();


            incomingMessage.Attachments = new List<Attachment>();

            List<CardImage> cardimages = new List<CardImage>();
            cardimages.Add(new CardImage(url: "https://timgsa.baidu.com/timg?image&quality=80&size=b9999_10000&sec=1490980048159&di=104c5ef2ae24b270e1976fd06577a3e8&imgtype=0&src=http%3A%2F%2Fimgm.ph.126.net%2FYy3TW3UU8mlOj8jodUuq9A%3D%3D%2F6597605228821133585.png"));

            List<CardAction> cardButtons = new List<CardAction>();



            CardAction plButton = new CardAction()
            {
                Value = re_Url,
                Type = "openUrl",
                Title = "More information"
            };
            cardButtons.Add(plButton);
            List<ReceiptItem> receiptList = new List<ReceiptItem>();

            foreach (Quotes q in quote)
            {
                string title = "";
                foreach (AirPlaces i in places)
                {
                    if (q.Outboudleg.OriginId == i.PlaceId)
                    {
                        title = i.Name;
                    }
                }

                foreach (AirPlaces i in places)
                {
                    if (q.Outboudleg.DestinationId == i.PlaceId)
                    {
                        title = title + "  --  " + i.Name;
                    }
                }

                string subtitle = "";

                foreach (Carriers i in carriers)
                {
                    if (q.Outboudleg.CarrierIds[0].ToString() == i.CarrierId)
                    {
                        subtitle = i.Name;
                    }
                }

                subtitle = subtitle + " " + Convert.ToDateTime(q.Outboudleg.DepartureDate).ToString("yyyy-MM-dd");

                ReceiptItem lineItem = new ReceiptItem()
                {
                    Title = title,
                    Subtitle = subtitle,
                    Price = q.MinPrice + currency[0].Symbol,
                    Image = new CardImage(url: "https://timgsa.baidu.com/timg?image&quality=80&size=b9999_10000&sec=1492622273490&di=d35bdd3145d751c386540cf90798cea0&imgtype=0&src=http%3A%2F%2Fimg01.taopic.com%2F150630%2F240402-1506300U40237.jpg")
                };
                receiptList.Add(lineItem);
            }

            ReceiptCard plCard = new ReceiptCard()
            {
                Title = "Booking Details",
                Buttons = cardButtons,
                Items = receiptList
            };

            Attachment plAttachment = plCard.ToAttachment();

            //incomingMessage.Attachments.Add(receiptCard.ToAttachment());
            incomingMessage.Attachments.Add(plAttachment);
            await (context.PostAsync(incomingMessage));

            //else
            //{
            //    await context.PostAsync("did not find alarm");
            //}
            //context.Wait(MessageReceived);

        }

        public string getRedirect(string from_location, string to_location, string outboundPartialDate)
        {
            from_location = from_location == "" ? "" : getCityName(from_location);
            to_location = to_location == "" ? "" : getCityName(to_location);

            string re_Url = String.Format("https://partners.api.skyscanner.net/apiservices/referral/v1.0/US/USD/en-US/{0}/{1}/{2}?apiKey=ma45219778513101", from_location, to_location, outboundPartialDate);

            return re_Url;
        }

        public string getData(string from_location, string to_location, string outboundPartialDate, IDialogContext context)
        {


            from_location = from_location == "" ? "" : getCityName(from_location);
            to_location = to_location == "" ? "" : getCityName(to_location);

          



                //DateTime t = DateTime.Today;
                //if(outboundPartialDate=="today"||outboundPartialDate=="Today")
                //{
                //    string month="";
                //    if(t.Month<10)
                //    {
                //        month = "0" + t.Month.ToString();
                //    }

                //    outboundPartialDate=t.Year.ToString()+"-"+month+"-"+t.Day.ToString();
                //}
                //if (outboundPartialDate == "tomorrow" || outboundPartialDate == "Tomorrow")
                //{
                //    t = t.AddDays(1);
                //    string month = "";
                //    if (t.Month < 10)
                //    {
                //        month = "0" + t.Month.ToString();
                //    }

                //    outboundPartialDate = t.Year.ToString() + "-" + month + "-" + t.Day.ToString();
                //}

                string Flighturl = string.Format("http://partners.api.skyscanner.net/apiservices/browsequotes/v1.0/US/USD/en-US/{0}/{1}/{2}?apiKey=ma452197785131011345379685638992", from_location, to_location, outboundPartialDate);
                HttpWebRequest request = WebRequest.Create(Flighturl) as HttpWebRequest;


                using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
                {
                    StreamReader reader = new StreamReader(response.GetResponseStream());

                    return reader.ReadToEnd();
                
            }
            //Uri uri = new Uri("http://partners.api.skyscanner.net/apiservices/browsequotes/v1.0/UK/GBP/en-GB/EDI/LHR/2017-04-22/2017-04-29?apiKey=ma452197785131011345379685638992");            
        }



        private string getCityName(string cityName)
        {
            string findCityNameUrl = string.Format("http://partners.api.skyscanner.net/apiservices/autosuggest/v1.0/CN/CNY/zh-CN?query={0}&apiKey=ma452197785131011345379685638992", cityName);
            HttpWebRequest request = WebRequest.Create(findCityNameUrl) as HttpWebRequest;
            using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
            {
                StreamReader reader = new StreamReader(response.GetResponseStream());
                //Console.WriteLine(reader.ReadToEnd());
                //Console.ReadKey();
                List<Places> p = new List<Places>();

                string jsonContent = reader.ReadToEnd();


                JObject o = JObject.Parse(jsonContent);
                p = JsonConvert.DeserializeObject<List<Places>>(o["Places"].ToString());



                return p[0].CityId.ToString();
            }


        }

        public RootDialog()
        {

        }

        //private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<object> result)
        //{
        //    var activity = await result as Activity;

        //    // calculate something for us to return
        //    int length = (activity.Text ?? string.Empty).Length;

        //    // return our reply to the user
        //    await context.PostAsync($"You sent {activity.Text} which was {length} characters");

        //    context.Wait(MessageReceivedAsync);
        //}

        [Serializable]
        public sealed class Query : IEquatable<Query>
        {
            public string toLocation { get; set; }
            public string fromLocation { get; set; }
            public string dateTime { get; set; }

            public bool Equals(Query other)
            {
                return other != null
                    && this.toLocation == other.toLocation
                    && this.fromLocation == other.fromLocation
                    && this.dateTime == other.dateTime;
            }

            public override bool Equals(Object other)
            {
                return Equals(other as Query);
            }

        }

    }
}