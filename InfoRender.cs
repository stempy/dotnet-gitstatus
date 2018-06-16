using System;

namespace dotnet_gitstatus
{
    partial class Program
    {
        public class InfoRender
        {
            public ConsoleColor HeaderColor {get;set;} = ConsoleColor.Blue;
            public ConsoleColor RepoColor   {get;set;} = ConsoleColor.DarkGreen;
            public ConsoleColor BranchColor {get;set;} = ConsoleColor.DarkGray;
            public ConsoleColor HashColor   {get;set;} = ConsoleColor.DarkMagenta;
            public ConsoleColor RemoteColor {get;set;} = ConsoleColor.DarkBlue;

            public int RepoPad   {get;set;}
            public int BranchPad {get;set;}
            public int RemotePad {get;set;}
            public int HashPad   {get;set;}

            private ConsoleColorWriter _writer;

            public InfoRender()
            {
                _writer = new ConsoleColorWriter();
            }

            public void PrintLineRow(GitInfo l)
            {
                _writer.W($"{l.TopLevel.PadRight(RepoPad)}",RepoColor)
                       .W($"{l.Branch.PadRight(BranchPad)}",BranchColor)
                       .W($"{l.Hash.PadRight(10)}",HashColor)
                       .W($"{l.Remote}\n",RemoteColor);
            }

            public void PrintLineDetails(GitInfo l)
            {
                _writer.W("Repo   : ",HeaderColor).WL(l.TopLevel,RepoColor)
                       .W("Branch : ",HeaderColor).WL(l.Branch,BranchColor)
                       .W("Hash   : ",HeaderColor).WL(l.FullHash,HashColor)
                       .W("Remote : ",HeaderColor).WL(l.Remote,RemoteColor);
            }
        }
    }
}
