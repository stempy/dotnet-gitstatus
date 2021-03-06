using System;
using System.IO;

namespace dotnet_gitstatus
{
    public class GitReader
    {
        public GitInfo ProcessPath(string path)
        {
            var oldDir = Directory.GetCurrentDirectory();
            Directory.SetCurrentDirectory(path);
            var simpleExec = new SimpleExec();

            var branch = simpleExec.Exec("git", "rev-parse --abbrev-ref HEAD").Trim();
            var remote = simpleExec.Exec("git", "config --get remote.origin.url").Trim();
            var topLevel = simpleExec.Exec("git", "rev-parse --show-toplevel").Trim();
            var fullhash = simpleExec.Exec("git", "log --pretty=format:%H -n 1").Trim();
            var hash = simpleExec.Exec("git", "rev-parse --short HEAD").Trim();

            var o = new GitInfo
            {
                Branch = branch,
                Remote = ParseRemote(remote, fullhash),
                TopLevel = Path.GetFileName(topLevel),
                Hash = hash,
                FullHash = fullhash
            };
            Directory.SetCurrentDirectory(oldDir);
            return o;
        }

        static string ParseRemote(string remote, string fullHash)
        {
            string newRemote = remote;
            try
            {
                if (remote.Contains(".visualstudio.com"))
                {
                    newRemote = $"{remote}/commit/{fullHash}";
                }
                if (remote.Contains("github.com"))
                {
                    var gitIdx = remote.IndexOf(".git");
                    if (gitIdx!=-1){
                        remote = remote.Remove(gitIdx);
                    }
                    newRemote = $"{remote}/commit/{fullHash}";
                }
                if (remote.Contains("bitbucket.org"))
                {
                    if (remote.Contains("@"))
                    {
                        // remove username:pw string from remote
                        var split = remote.Split('@');
                        var scheme = split[0].Remove(split[0].IndexOf("//") + 2);
                        remote = $"{scheme}{split[1]}";
                    }

                    var idxGit = remote.IndexOf(".git");
                    if (idxGit != -1)
                    {
                        newRemote = $"{remote.Remove(remote.IndexOf(".git"))}/commits/{fullHash}";
                    }
                    else
                    {
                        newRemote = $"{remote}/commits/{fullHash}";
                    }
                }

            } catch(Exception ex)
            {
                throw new Exception("Unale to parse remote:" +remote, ex);
            }
            return newRemote;
        }
    }
}
