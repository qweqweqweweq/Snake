using Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Snake_FilimonovaPleshkova
{
    class Program
    {
        public static List<Leaders> leaders = new List<Leaders>();
        public static List<ViewModelUserSettings> remoteIPAddress = new List<ViewModelUserSettings>();
        public static List<ViewModelGames> viewModelGames = new List<ViewModelGames>();
        private static int localPort = 5003;
        public static int MaxSpeed = 15;

        private static void Send()
        {
            foreach(ViewModelUserSettings User in remoteIPAddress)
            {
                UdpClient sender = new UdpClient();
                IPEndPoint endPoint = new IPEndPoint(
                    IPAddress.Parse(User.IPAddress),
                    int.Parse(User.Port));
                try
                {
                    byte[] bytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(viewModelGames.Find(x => x.IdSnake == User.IdSnake)));

                    sender.Send(bytes, bytes.Length, endPoint);

                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"Отправил данные пользователю: {User.IPAddress}:{User.Port}");
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Возникло исключение: " + ex.ToString() + "\n" + ex.Message);
                }
                finally
                {
                    sender.Close();
                }
            }
        }
    }
}
