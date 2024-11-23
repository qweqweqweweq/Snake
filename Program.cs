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

        public static void Receiver()
        {
            UdpClient receivingUdpClient = new UdpClient(localPort);
            IPEndPoint RemoteIpEndPoint = null;

            try
            {
                Console.WriteLine("Команды сервера:");
                while (true)
                {
                    byte[] receiveBytes = receivingUdpClient.Receive(
                        ref RemoteIpEndPoint);

                    string returnData = Encoding.UTF8.GetString(receiveBytes);

                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("Получил команду: " + returnData.ToString());

                    if (returnData.ToString().Contains("/start"))
                    {
                        string[] dataMessage = returnData.ToString().Split('|');
                        ViewModelUserSettings viewModelUserSettings = JsonConvert.DeserializeObject<ViewModelUserSettings>(dataMessage[1]);
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine($"Подключился пользователь: {viewModelUserSettings.IPAddress}:{viewModelUserSettings.Port}");
                        remoteIPAddress.Add(viewModelUserSettings);
                        viewModelUserSettings.IdSnake = AddSnake();
                        viewModelGames[viewModelUserSettings.IdSnake].IdSnake = viewModelUserSettings.IdSnake;
                    }
                    else
                    {
                        string[] dataMessage = returnData.ToString().Split('|');
                        ViewModelUserSettings viewModelUserSettings = JsonConvert.DeserializeObject<ViewModelUserSettings>(dataMessage[1]);
                        int IdPlayer = -1;
                        IdPlayer = remoteIPAddress.FindIndex(x => x.IPAddress == viewModelUserSettings.IPAddress && x.Port == viewModelUserSettings.Port);
                        if (IdPlayer != -1)
                        {
                            if (dataMessage[0] == "Up" &&
                                viewModelGames[IdPlayer].SnakesPlayer.direction != Snakes.Direction.Down)
                                viewModelGames[IdPlayer].SnakesPlayer.direction = Snakes.Direction.Up;
                            else if(dataMessage[0] == "Down" &&
                                viewModelGames[IdPlayer].SnakesPlayer.direction != Snakes.Direction.Up)
                                viewModelGames[IdPlayer].SnakesPlayer.direction = Snakes.Direction.Down;
                            else if(dataMessage[0] == "Left" &&
                                viewModelGames[IdPlayer].SnakesPlayer.direction != Snakes.Direction.Right)
                                viewModelGames[IdPlayer].SnakesPlayer.direction = Snakes.Direction.Left;
                            else if (dataMessage[0] == "Right" &&
                                viewModelGames[IdPlayer].SnakesPlayer.direction != Snakes.Direction.Left)
                                viewModelGames[IdPlayer].SnakesPlayer.direction = Snakes.Direction.Right;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Возникло исключение: " + ex.ToString() + "\n" + ex.Message);
            }
        }
    }
}
