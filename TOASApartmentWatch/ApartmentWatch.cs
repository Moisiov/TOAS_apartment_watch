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

namespace TOASApartmentWatch
{
    class ApartmentWatch
    {
        private static IServiceProvider _serviceProvider;
        private DataFetcher _fetcher;
        private List<MonthlyApartmentContainer> _apartments;

        public ApartmentWatch(IServiceProvider provider)
        {
            _serviceProvider = provider;
            _fetcher = new DataFetcher();
            _apartments = new List<MonthlyApartmentContainer>();

            var textOutputOptions = _serviceProvider.GetService<IConfiguration>()
                .GetSection("TextOutputOptions")
                .Get<TextOutputOptions>();

            Console.OutputEncoding = Encoding.UTF8;
            Console.ForegroundColor = textOutputOptions.TextColor;
        }

        public async Task run()
        {
            while (true)
            {
                var generalOptions = _serviceProvider.GetService<IConfiguration>()
                    .GetSection("GeneralOptions")
                    .Get<GeneralOptions>();

                var data = await _fetcher.FetchApartments();
                var fetchedApartments = parseHtmlString(data);
                var newApartmentsFound = compareApartments(fetchedApartments);
                printApartmentData();
                if (newApartmentsFound && generalOptions.NotificationSoundOn) { notificationSound(); }

                await Task.Delay(TimeSpan.FromMinutes(generalOptions.FetchIntervalMinutes));
            }
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

        private bool compareApartments(List<MonthlyApartmentContainer> data)
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

        private void printApartmentData()
        {
            var textOutputOptions = _serviceProvider.GetService<IConfiguration>()
                .GetSection("TextOutputOptions")
                .Get<TextOutputOptions>();

            if (textOutputOptions.ClearConsoleAutomatically)
            {
                Console.Clear();
                Console.WriteLine("Apartment situation " + DateTime.Now.ToString("dd.MM.yyyy HH:mm"));
            }
            else
            {
                Console.WriteLine("\n\n\nApartment situation " + DateTime.Now.ToString("dd.MM.yyyy HH:mm"));
            }

            foreach (var month in _apartments)
            {
                Console.WriteLine("\n\n" + month.Title + "\n");

                var tableString = "{0,-20}{1,-60}{2,-8}{3,-8}{4, -24}{5, -16}";
                Console.WriteLine(tableString, "Target", "Apartment type", "Area", "Floor", "Rent €/kk", "Added");

                foreach (var apartment in month.Apartments)
                {
                    if (apartment.TimeStamp > DateTime.Now.AddMinutes(-textOutputOptions.NewApartmentHighLightMinutes))
                    {
                        Console.ForegroundColor = textOutputOptions.TextHighlightColor;
                    }

                    Console.WriteLine(tableString,
                        apartment.Target,
                        apartment.ApartmentType,
                        apartment.Area,
                        apartment.Floor,
                        apartment.Rent,
                        apartment.TimeStamp.ToString("dd.MM.yyyy HH:mm"));

                    if (Console.ForegroundColor == textOutputOptions.TextHighlightColor)
                    {
                        Console.ForegroundColor = textOutputOptions.TextColor;
                    }
                }
            }
        }

        private void notificationSound()
        {
            // TODO: Figure out better way of playing sounds (NAudio, Node tms?)
            Process.Start(@"powershell", $@"[console]::beep(1800,400)");
        }
    }
}
