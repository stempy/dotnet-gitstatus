using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace dotnet_gitstatus
{
    class Program
    {
        static string Exec(string file, string args){
            System.Diagnostics.Process process = new System.Diagnostics.Process();
            System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
            startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            startInfo.FileName = file;
            startInfo.Arguments = args;
            startInfo.RedirectStandardInput=true;
            startInfo.RedirectStandardOutput=true;
            
            process.StartInfo = startInfo;

            process.Start();
            process.StandardInput.Flush();
            process.StandardInput.Close();
            process.WaitForExit();
            return process.StandardOutput.ReadToEnd();
        }
        
        static GitInfo ProcessPath(string path){
            Directory.SetCurrentDirectory(path);
            var branch = Exec("git","rev-parse --abbrev-ref HEAD").Trim();
            var remote = Exec("git","config --get remote.origin.url").Trim();
            var topLevel = Exec("git","rev-parse --show-toplevel").Trim();
            var fullhash = Exec("git","log --pretty=format:%H -n 1").Trim();
            var hash = Exec("git","rev-parse --short HEAD").Trim();

            var o = new GitInfo {
                Branch = branch,
                Remote = remote,
                TopLevel = Path.GetFileName(topLevel),
                Hash = hash,
                FullHash = fullhash
            };
            return o;
        }



        static void Main(string[] args)
        {
            if (!args.Any()){
                ProcessPath(Directory.GetCurrentDirectory());
            } else {
                var oldDir = Directory.GetCurrentDirectory();
                
                var pathSpec = args[0];
                var filePattern = args.Length>1? args[1]:"*";
                var lastPartOfPath = Path.GetFileName(pathSpec);
                if (lastPartOfPath.Contains("*")|| lastPartOfPath.Contains("?")){
                    pathSpec = Path.GetDirectoryName(pathSpec);
                    filePattern = lastPartOfPath;
                }
                
                var dirs = Directory.GetDirectories(pathSpec,filePattern);
                var items = new List<GitInfo>();
                foreach(var d in dirs){
                    if (Directory.Exists(Path.Combine(d,".git"))){
                        var gi=ProcessPath(d);
                        items.Add(gi);
                    }
                }
                PrintLines(items);

            }
        }
        
        public class GitInfo {
            public string Branch {get; set;}
            public string Remote {get; set;}
            public string TopLevel {get; set;}
            public string FullHash {get; set;}
            public string Hash {get; set;}
        }

        static void PrintLine(GitInfo g)
        {
            var line = $"{g.TopLevel.PadRight(50)}{g.Branch.PadRight(20)}{g.Hash.PadLeft(32)}{g.Remote}";
            Console.WriteLine(line);
        }

        static void PrintLines(IEnumerable<GitInfo> g)
        {
            g = g.ToList();
            foreach(var a in g){
                if (a.Branch.Length>30){
                    a.Branch=a.Branch.Substring(0,27) + "...";
                }

                if (a.Remote.Contains(".visualstudio.com")){
                    a.Remote = $"{a.Remote}/commit/{a.FullHash}";
                }
            }
            
            var topLevelLengths = g.Select(x=>x.TopLevel.Length);
            var maxLevel=topLevelLengths.OrderByDescending(x=>x).FirstOrDefault();
            var branchMaxLevel = g.Select(x=>x.Branch.Length).OrderByDescending(x=>x).FirstOrDefault();

            var topLevelPad = maxLevel+5;
            var branchPad = branchMaxLevel+2;
            
            var colorWriter = new ConsoleColorWriter();

            colorWriter.WriteLine($"Repo{"".PadRight(topLevelPad-4)}Branch{"".PadRight(branchPad-6)}Hash{"".PadRight(10-4)}Remote",ConsoleColor.Blue)
                       .WriteLine(new string('=',160),ConsoleColor.Cyan);


            var repoNameColor = ConsoleColor.DarkGreen;
            var branchColor = ConsoleColor.DarkGray;
            var hashColor = ConsoleColor.DarkMagenta;
            var remoteColor = ConsoleColor.DarkBlue;

            foreach(var l in g){
                var line = $"{l.TopLevel.PadRight(topLevelPad)}{l.Branch.PadRight(branchPad)}{l.Hash.PadRight(10)}{l.Remote}";
                
                colorWriter.Write($"{l.TopLevel.PadRight(topLevelPad)}",repoNameColor)
                           .Write($"{l.Branch.PadRight(branchPad)}",branchColor)
                           .Write($"{l.Hash.PadRight(10)}",hashColor)
                           .Write($"{l.Remote}\n",remoteColor);
            }
        }
    }

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

    public static class ConsoleWriterExtensions
    {
        public static ConsoleColorWriter Write(this ConsoleColorWriter writer, string message, ConsoleColor color){
            writer.Write(color,message);
            return writer;
        }
        public static ConsoleColorWriter WriteLine(this ConsoleColorWriter writer, string message, ConsoleColor color){
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
