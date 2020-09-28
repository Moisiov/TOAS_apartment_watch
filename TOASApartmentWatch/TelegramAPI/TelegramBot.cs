using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using TOASApartmentWatch.Models.ApartmentData;

namespace TOASApartmentWatch.TelegramAPI
{
    class TelegramBot : ITelegramAPI
    {
        private List<MonthlyApartmentContainer> _apartments;
        private static TelegramBotClient _botClient;
        public TelegramBot()
        {
            _botClient = new TelegramBotClient("1303471573:AAGcvtZ03loibXS28t1ndXShmC2gqkcxp9k");
            _botClient.OnMessage += BotOnMessageReceived;
            _botClient.StartReceiving();
            Console.WriteLine("Telegrambot ready to receive");
        }


        private async void BotOnMessageReceived(object sender, MessageEventArgs messageEventArgs)
        {
            var message = messageEventArgs.Message;
            if (message == null || message.Type != MessageType.Text)
                return;

            switch (message.Text.Split(' ').First())
            {
                case "/help":
                    await SendHelpMenu(message);
                    break;
                case "/availability":
                    await SendAvailability(message);
                    break;
            }
            if (messageEventArgs.Message.Type == Telegram.Bot.Types.Enums.MessageType.Text)
            {
                await _botClient.SendTextMessageAsync(messageEventArgs.Message.Chat.Id, "Testi onnistunut");
                Console.WriteLine(messageEventArgs.Message.Text);
            }
            async Task SendHelpMenu(Message message)
            {
                const string helpMenuText = "Usage:\n" +
                                        "/help   - See help menu for advice\n" +
                                        "/subscribe - Subscribe to our awesome service\n" +
                                        "/availability    - See current apartment availability\n";
                await _botClient.SendTextMessageAsync(
                    chatId: message.Chat.Id,
                    text: helpMenuText
                    );
            }
            async Task SendAvailability(Message message)
            {
                string apartmentTextData = string.Empty;
                foreach (var month in _apartments)
                {
                    apartmentTextData += "\n" + month.Title + "\n\n";
                    foreach (var apartment in month.Apartments)
                    {
                        string squareMeters = apartment.Area == "-" ? string.Empty : apartment.Area + "m2 - ";
                        apartmentTextData += String.Format("{0} {1}\n{2}{3}\n", apartment.Target, apartment.ApartmentType, squareMeters, apartment.Rent);
                    }
                }
                await _botClient.SendTextMessageAsync(
                chatId: message.Chat.Id,
                text: apartmentTextData
                );
            }
        }

        public void UpdateApartmentData(List<MonthlyApartmentContainer> data)
        {
            _apartments = data;
        }
    }
}
