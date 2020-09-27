using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace TOAS_apartment_watch
{
    class ApartmentWatch
    {
        private DataFetcher _fetcher;
        private List<MonthlyApartmentContainer> _apartments;

        public ApartmentWatch()
        {
            _fetcher = new DataFetcher();
            _apartments = new List<MonthlyApartmentContainer>();
        }

        public async Task run()
        {
            string data = await _fetcher.FetchApartments();
            var fetchedApartments = parseHtmlString(data);
        }

        private List<MonthlyApartmentContainer> parseHtmlString(string data)
        {
            List<MonthlyApartmentContainer> currentApartments = new List<MonthlyApartmentContainer>();
            var html = new HtmlDocument();
            html.LoadHtml(data);

            var monthNodes = html.DocumentNode.SelectNodes("//div[contains(@class, 'estate-types')]");

            foreach (var monthNode in monthNodes)
            {
                var titleNode = monthNode.SelectSingleNode(".//h3");
                Console.WriteLine("\n\n" + titleNode.InnerText + "\n");
                string theadString = "{0,-20}{1,-60}{2,-8}{3,-8}{4}";
                Console.WriteLine(theadString, "Kohde", "Asuntotyyppi", "Neliöt", "Kerros", "Vuokra €/kk");


                var apartmentNodes = monthNode.SelectNodes(".//tbody/tr");
                List<ApartmentModel> apartments = new List<ApartmentModel>();

                foreach (var apartmentNode in apartmentNodes)
                {
                    var apartmentData = apartmentNode.SelectNodes("./td");

                    string apartmentString = "{0,-20}{1,-60}{2,-8}{3,-8}{4}";
                    Console.WriteLine(apartmentString,
                        apartmentData[0].InnerText,
                        apartmentData[1].InnerText,
                        apartmentData[2].InnerText,
                        apartmentData[3].InnerText,
                        apartmentData[4].InnerText);

                    apartments.Add(new ApartmentModel()
                    {
                        Target = apartmentData[0].InnerText,
                        ApartmentType = apartmentData[1].InnerText,
                        Area = apartmentData[2].InnerText,
                        Floor = apartmentData[3].InnerText,
                        Rent = apartmentData[4].InnerText,
                        TimeStamp = DateTime.Now
                    });
                }

                currentApartments.Add(new MonthlyApartmentContainer()
                {
                    Title = titleNode.InnerText,
                    Apartments = apartments
                });
            }

            return currentApartments;
        }

        /*private void compareApartments(List<MonthlyApartmentContainer> data)
        {
            foreach (var month in data)
            {
                foreach (var apartment in month.Apartments)
                {
                    if(apartment)
                }
            }
        }*/
    }
}
