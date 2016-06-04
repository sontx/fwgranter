using Core.Win32.System32;
using FWGranter.Properties;
using System;
using System.IO;

namespace FWGranter
{
    internal class Program
    {
        private static void ShowHelp()
        {
            Console.WriteLine("Firewall Granter v1.0.0");
            Console.WriteLine("Author: Gin");
            Console.WriteLine("\nAllows applications can transfer data through the firewall. Below is how to use it.\n");
            Console.WriteLine("fwgranter [param1] [param2]");
            Console.WriteLine("param1:");
            Console.WriteLine(" -a\tadd an app to Firewall exceptions list.");
            Console.WriteLine(" -rP\tremove an app from Firewall exceptions list by app path.");
            Console.WriteLine(" -rN\tremove an app from Firewall exceptions list by app name.");
            Console.WriteLine(" -s\tdisplay firewall status.");
            Console.WriteLine("param2:");
            Console.WriteLine(" -a [appPath]");
            Console.WriteLine(" -a [appPath] [appName]");
            Console.WriteLine(" -rP [appPath]");
            Console.WriteLine(" -rN [appName]");
            Console.WriteLine("Example:");
            Console.WriteLine(" fwgranter -a \"c:\\myApp.exe\"");
            Console.WriteLine(" fwgranter -a \"c:\\myApp.exe\" \"superApp\"");
            Console.WriteLine(" fwgranter -rP \"c:\\myApp.exe\"");
            Console.WriteLine(" fwgranter -rN \"superApp\"");
            Console.WriteLine(" fwgranter -s");
        }

        private static bool AnalyseArguments(string[] args, out string param1, out string param2)
        {
            param1 = null;
            param2 = null;
            if (args.Length != 2) return false;
            param1 = args[0];
            param2 = args[1];
            switch (param1)
            {
                case "-add":
                    return File.Exists(param2);

                case "-rem":
                    return true;
            }
            return false;
        }

        private static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                ShowHelp();
            }
            else
            {
                try
                {
                    string func = args[0];
                    FirewallHelper fwHelper = FirewallHelper.Instance;
                    if (func == "-a")
                    {
                        string appName;
                        if (args.Length == 2)
                        {
                            string appPath = args[1];
                            appName = Path.GetFileNameWithoutExtension(appPath);
                            fwHelper.GrantRule(Path.GetFullPath(appPath), appName, Resources.description);
                        }
                        else if (args.Length == 3)
                        {
                            string appPath = args[1];
                            appName = args[2];
                            fwHelper.GrantRule(Path.GetFullPath(appPath), appName, Resources.description);
                            Console.WriteLine("Added {0} to exceptions list.", appName);
                        }
                        else
                        {
                            throw new ArgumentOutOfRangeException("-a just has 1 or 2 options.");
                        }
                        Console.WriteLine("Added {0} to exceptions list.", appName);
                    }
                    else if (func == "-rP")
                    {
                        if (args.Length == 2)
                        {
                            string appPath = args[1];
                            string appName = Path.GetFileNameWithoutExtension(appPath);
                            fwHelper.RemoveRule(appName);
                            Console.WriteLine("Removed {0} to exceptions list.", appName);
                        }
                        else
                        {
                            throw new ArgumentOutOfRangeException("-rP just has option.");
                        }
                    }
                    else if (func == "-rN")
                    {
                        if (args.Length == 2)
                        {
                            string appName = args[1];
                            fwHelper.RemoveRule(appName);
                            Console.WriteLine("Removed {0} to exceptions list.", args[1]);
                        }
                        else
                        {
                            throw new ArgumentOutOfRangeException("-rN just has option.");
                        }
                    }
                    else if (func == "-s")
                    {
                        Console.WriteLine("Firewall is installed: {0}", fwHelper.IsFirewallInstalled ? "YES" : "NO");
                        Console.WriteLine("Firewall is enabled: {0}", fwHelper.IsFirewallEnabled ? "YES" : "NO");
                        Console.WriteLine("Firewall allows exceptions: {0}", fwHelper.AppAuthorizationsAllowed ? "YES" : "NO");
                    }
                    else
                    {
                        ShowHelp();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }
    }
}