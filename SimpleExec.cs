namespace dotnet_gitstatus
{
    public class SimpleExec
    {
        public string Exec(string file, string args){
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
    }
}
