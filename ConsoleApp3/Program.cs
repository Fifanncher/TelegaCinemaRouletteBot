using System;
using System.Web;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SmallBasic.Library;
using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;
using System.Web.Helpers;

namespace TelegaCinemaBot
{
    class Program
    {
        static void Main(string[] args)
        {

            string token = ""; // Телеграм токен бота

            var bot = new TelegramBotClient(token);

            var markup = new ReplyKeyboardMarkup(new[]
             {
                    new KeyboardButton("Roll film!"),

            })
            {
                OneTimeKeyboard = true,
                ResizeKeyboard = true
            };
            Console.WriteLine("Готово!");

            bot.OnMessage += (sender, e) =>
            {
                //Вывод информации о запросах к серверу
                string msg = $"{DateTime.Now}: {e.Message.Chat.Id} {e.Message.Chat.FirstName} {e.Message.Chat.Username} {e.Message.Text}";
                Console.WriteLine(msg);

                string response = null;
                string url = "https://www.kinopoisk.ru/chance/?item=true&not_show_rated=false&count=1";
                
                using (var webClient = new WebClient())
                {
                    response = webClient.DownloadString(url);
                }

                dynamic data = Json.Decode(response);
                response = data[0];

                string subString = "data-id-film=";
                var a = response.IndexOf(subString) + 14; //находим начальную позицию для парсинга id фильма из запроса
                var b = response.IndexOf('\"', a); //находим конечную позицию для парсинга id фильма из запроса
                string c = response.Substring(a, b - a);

                int filmId = Int32.Parse(c);

                string msg2 = "https://www.kinopoisk.ru/film/"+ c;
                Console.WriteLine(filmId);

                bot.SendTextMessageAsync(e.Message.Chat.Id, msg2, replyMarkup: markup);
            };

            bot.StartReceiving();
            Console.ReadKey();
        }
    }
}
