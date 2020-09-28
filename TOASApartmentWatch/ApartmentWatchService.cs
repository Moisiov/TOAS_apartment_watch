using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;
using Microsoft.Extensions.Configuration;
using TOASApartmentWatch.Models.ApartmentData;
using TOASApartmentWatch.Models.Options;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics;
using Microsoft.Extensions.Hosting;
using System.Threading;

namespace TOASApartmentWatch
{
    class ApartmentWatchService : BackgroundService
    {
        private static IServiceProvider _serviceProvider;
        private DataFetcher _fetcher;
        private List<MonthlyApartmentContainer> _apartments;
        private ITelegramAPI _tgAPI;

        public ApartmentWatchService(IServiceProvider provider)
        {
            _serviceProvider = provider;
            _fetcher = new DataFetcher();
            _apartments = new List<MonthlyApartmentContainer>();
        }

        protected override async Task ExecuteAsync(CancellationToken stopToken)
        {
            while (!stopToken.IsCancellationRequested)
            {
                var options = _serviceProvider.GetService<IConfiguration>().Get<Options>();

                await Run();
                await Task.Delay(TimeSpan.FromMinutes(options.GeneralOptions.FetchIntervalMinutes), stopToken);
            }
        }

        public async Task Run()
        {
            var data = await _fetcher.FetchApartments();
            var fetchedApartments = ParseHtmlString(data);
            var newApartmentsFound = CompareApartments(fetchedApartments);

            
        }

        private List<MonthlyApartmentContainer> ParseHtmlString(string data)
        {
            List<MonthlyApartmentContainer> currentApartments = new List<MonthlyApartmentContainer>();
            var html = new HtmlDocument();
            html.LoadHtml(data);

            var monthNodes = html.DocumentNode.SelectNodes("//div[contains(@class, 'estate-types')]");

            foreach (var monthNode in monthNodes)
            {
                var titleNode = monthNode.SelectSingleNode(".//h3");

                var apartmentNodes = monthNode.SelectNodes(".//tbody/tr");
                List<ApartmentModel> apartments = new List<ApartmentModel>();

                foreach (var apartmentNode in apartmentNodes)
                {
                    var apartmentData = apartmentNode.SelectNodes("./td");

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

        private bool CompareApartments(List<MonthlyApartmentContainer> data)
        {
            bool newApartmentDetected = false;
            bool initialData = _apartments.Count == 0 ? true : false;

            foreach (var month in data)
            {
                foreach (var apartment in month.Apartments)
                {
                    var monthlyApartments = _apartments.Find(x => x.Title == month.Title);
                    if (monthlyApartments != null)
                    {
                        var foundApartment = monthlyApartments.Apartments.Find(x => x.Target == apartment.Target
                                                                            && x.ApartmentType == apartment.ApartmentType
                                                                            && x.Area == apartment.Area
                                                                            && x.Floor == apartment.Floor
                                                                            && x.Rent == apartment.Rent);

                        if (foundApartment != null)
                        {
                            apartment.Target = foundApartment.Target;
                            apartment.ApartmentType = foundApartment.ApartmentType;
                            apartment.Area = foundApartment.Area;
                            apartment.Floor = foundApartment.Floor;
                            apartment.Rent = foundApartment.Rent;
                            apartment.TimeStamp = foundApartment.TimeStamp;
                        }
                        else
                        {
                            newApartmentDetected = true;
                        }
                    }
                    else
                    {
                        newApartmentDetected = true;
                    }
                }
            }

            _apartments = data;

            return newApartmentDetected;
        }
    }
}
