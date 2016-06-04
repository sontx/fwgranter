using Core.Win32.System32;
using FWGranter.Properties;
using System;
using System.IO;
using System.Windows.Forms;

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
            Console.WriteLine(" -a -g");
            Console.WriteLine(" -rP [appPath]");
            Console.WriteLine(" -rP -g");
            Console.WriteLine(" -rN [appName]");
            Console.WriteLine("Example:");
            Console.WriteLine(" fwgranter -a \"c:\\myApp.exe\"");
            Console.WriteLine(" fwgranter -a \"c:\\myApp.exe\" \"superApp\"");
            Console.WriteLine(" fwgranter -a -g");
            Console.WriteLine(" fwgranter -rP \"c:\\myApp.exe\"");
            Console.WriteLine(" fwgranter -rP -g");
            Console.WriteLine(" fwgranter -rN \"superApp\"");
            Console.WriteLine(" fwgranter -s");
        }

        private static string PickAFile()
        {
            using (OpenFileDialog dlg = new OpenFileDialog())
            {
                dlg.Title = "FWGranter - Select an application.";
                dlg.Filter = "Excutable files|*.exe|All files|*.*";
                if (dlg.ShowDialog() == DialogResult.OK)
                    return dlg.FileName;
                return null;
            }
        }

        private static void AddException(string[] args, FirewallHelper fwHelper)
        {
            string appName;
            if (args.Length == 2)
            {
                string appPath = args[1] == "-g" ? PickAFile() : Path.GetFullPath(args[1]);
                if (appPath != null)
                {
                    appName = Path.GetFileNameWithoutExtension(appPath);
                    fwHelper.GrantRule(Path.GetFullPath(appPath), appName, Resources.description);
                }
                else
                {
                    return;
                }
            }
            else if (args.Length == 3)
            {
                string appPath = args[1];
                appName = args[2];
                fwHelper.GrantRule(Path.GetFullPath(appPath), appName, Resources.description);
            }
            else
            {
                throw new ArgumentOutOfRangeException("-a just has 1 or 2 options.");
            }
            Console.WriteLine("Added {0} to exceptions list.", appName);
        }

        private static void RemoveExceptionByPath(string[] args, FirewallHelper fwHelper)
        {
            if (args.Length == 2)
            {
                string appPath = args[1] == "-g" ? PickAFile() : Path.GetFullPath(args[1]);
                if (appPath != null)
                {
                    string appName = Path.GetFileNameWithoutExtension(appPath);
                    fwHelper.RemoveRule(appName);
                    Console.WriteLine("Removed {0} to exceptions list.", appName);
                }
                else
                {
                    return;
                }
            }
            else
            {
                throw new ArgumentOutOfRangeException("-rP just has option.");
            }
        }

        private static void RemoveExceptionByName(string[] args, FirewallHelper fwHelper)
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

        private static void ShowFWStatus(string[] args, FirewallHelper fwHelper)
        {
            Console.WriteLine("Firewall is installed: {0}", fwHelper.IsFirewallInstalled ? "YES" : "NO");
            Console.WriteLine("Firewall is enabled: {0}", fwHelper.IsFirewallEnabled ? "YES" : "NO");
            Console.WriteLine("Firewall allows exceptions: {0}", fwHelper.AppAuthorizationsAllowed ? "YES" : "NO");
        }

        [STAThread]
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
                        AddException(args, fwHelper);
                    }
                    else if (func == "-rP")
                    {
                        RemoveExceptionByPath(args, fwHelper);
                    }
                    else if (func == "-rN")
                    {
                        RemoveExceptionByName(args, fwHelper);
                    }
                    else if (func == "-s")
                    {
                        ShowFWStatus(args, fwHelper);
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