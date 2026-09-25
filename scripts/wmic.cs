using System;
using System.Diagnostics;
using System.Management;

public class Program
{
    public static void Main(string[] args)
    {
        Console.WriteLine("Name                          ParentProcessId  ProcessId   Status");
        try
        {
            using (ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT Name, ProcessId, ParentProcessId FROM Win32_Process"))
            {
                foreach (ManagementObject obj in searcher.Get())
                {
                    string name = obj["Name"] != null ? obj["Name"].ToString() : "unknown";
                    string ppid = obj["ParentProcessId"] != null ? obj["ParentProcessId"].ToString() : "0";
                    string pid = obj["ProcessId"] != null ? obj["ProcessId"].ToString() : "0";
                    Console.WriteLine(string.Format("{0,-30}{1,-17}{2,-12}", name, ppid, pid));
                }
            }
        }
        catch
        {
            try
            {
                foreach (Process p in Process.GetProcesses())
                {
                    Console.WriteLine(string.Format("{0,-30}{1,-17}{2,-12}", p.ProcessName, "0", p.Id));
                }
            }
            catch {}
        }
    }
}
