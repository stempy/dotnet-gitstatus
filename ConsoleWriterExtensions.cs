using System;

namespace dotnet_gitstatus
{
    public static class ConsoleWriterExtensions
    {
        public static ConsoleColorWriter W(this ConsoleColorWriter writer, string message, ConsoleColor color){
            writer.Write(color,message);
            return writer;
        }
        public static ConsoleColorWriter WL(this ConsoleColorWriter writer, string message, ConsoleColor color){
            writer.WriteLine(color,message);
            return writer;
        }
        public static ConsoleColorWriter WriteLine(this ConsoleColorWriter writer, string message){
            writer.WriteLine(message);
            return writer;
        }

         public static ConsoleColorWriter Write(this ConsoleColorWriter writer, string message){
            writer.Write(message);
            return writer;
        }
    }
}
