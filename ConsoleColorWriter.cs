using System;

namespace dotnet_gitstatus
{
    public class ConsoleColorWriter
    {
        public void Write(ConsoleColor color, string message){
            var prevFgColor = Console.ForegroundColor;
            Console.ForegroundColor = color;
            Console.Write(message);
            Console.ForegroundColor = prevFgColor;
        }

        public void WriteLine(ConsoleColor color, string message){
            var prevFgColor = Console.ForegroundColor;
            Console.ForegroundColor = color;
            Console.WriteLine(message);
            Console.ForegroundColor = prevFgColor;
        }

        public void Write(string message){
            Console.Write(message);
        }
    }
}
