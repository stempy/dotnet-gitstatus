using System.IO;

namespace dotnet_gitstatus
{
    public class GitReader
    {
        public GitInfo ProcessPath(string path){
            Directory.SetCurrentDirectory(path);
            var simpleExec = new SimpleExec();
            
            var branch = simpleExec.Exec("git","rev-parse --abbrev-ref HEAD").Trim();
            var remote = simpleExec.Exec("git","config --get remote.origin.url").Trim();
            var topLevel = simpleExec.Exec("git","rev-parse --show-toplevel").Trim();
            var fullhash = simpleExec.Exec("git","log --pretty=format:%H -n 1").Trim();
            var hash = simpleExec.Exec("git","rev-parse --short HEAD").Trim();

            var o = new GitInfo {
                Branch = branch,
                Remote = ParseRemote(remote,fullhash),
                TopLevel = Path.GetFileName(topLevel),
                Hash = hash,
                FullHash = fullhash
            };
            return o;
        }

        static string ParseRemote(string remote, string fullHash)
        {
            string newRemote=remote;
            if (remote.Contains(".visualstudio.com")){
                newRemote = $"{remote}/commit/{fullHash}";
            }
            if (remote.Contains("github.com")){
                newRemote = $"{remote.Remove(remote.IndexOf(".git"))}/commit/{fullHash}";
            }
            if (remote.Contains("bitbucket.org")){
                if (remote.Contains("@"))
                {
                    // remove username:pw string from remote
                    var split = remote.Split('@');
                    var scheme = split[0].Remove(split[0].IndexOf("//")+2);
                    remote = $"{scheme}{split[1]}";
                }
                newRemote = $"{remote.Remove(remote.IndexOf(".git"))}/commits/{fullHash}";
            }
            return newRemote;
        }
    }
}
