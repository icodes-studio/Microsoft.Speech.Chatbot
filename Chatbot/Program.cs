using System;
using System.Configuration;
using System.Diagnostics;

namespace Chatbot
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Debug.Listeners.Add(new TextWriterTraceListener(Console.Out));
            Log.Initialize(
                ConfigurationManager.AppSettings["LogPath"], nameof(Chatbot),
                (LogLevel)Enum.Parse(typeof(LogLevel), ConfigurationManager.AppSettings["LogLevel"]),
                (_, message) => Debug.WriteLine(message));

            if (Chatbot.i.Start() == false)
                return;

            Console.WriteLine("\nType 'exit' to stop the server.\n");

            while (true)
            {
                var command = Console.ReadLine();
                if (command == "exit")
                {
                    Chatbot.i.Stop();
                    break;
                }
                else
                {
                    Chatbot.i.Speak(command);
                }
            }
        }
    }
}
