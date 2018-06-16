using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace dotnet_gitstatus
{
    partial class Program
    {
        static void Main(string[] args)
        {
            var gitReader = new GitReader();
            
            if (!args.Any()){
                var res= gitReader.ProcessPath(Directory.GetCurrentDirectory());
                PrintLine(res);
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
                        var gi= gitReader.ProcessPath(d);
                        items.Add(gi);
                    }
                }
                PrintLines(items);
            }
        }

        static void PrintLine(GitInfo g)
        {
            var infoRender = new InfoRender();
            infoRender.PrintLineDetails(g);
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
            
            var remotePad = 0;
            var remotePadStr=new string(' ',remotePad);

            var renderer = new InfoRender()
            {
                RepoPad = maxLevel+5,
                BranchPad = branchMaxLevel+2,
                RemotePad = remotePad
            };

            var colorWriter = new ConsoleColorWriter();
            colorWriter.WL($"Repo{"".PadRight(renderer.RepoPad-4)}Branch{"".PadRight(renderer.BranchPad-6)}Hash{"".PadRight(10-4)}Remote",ConsoleColor.Blue)
                       .WL(new string('=',160),ConsoleColor.Cyan);

            foreach(var l in g){
                renderer.PrintLineRow(l);
            }
        }
    }
}
