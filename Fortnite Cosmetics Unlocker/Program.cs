using Fiddler;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace Fortnite_Cosmetics_Unlocker
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Downloader.EnsureProfilesExist();

            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Welcome to Cosmetics Unlocker For PIE!");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("Credit BiruFN,Landmark0920");
            Console.ResetColor();

            if (!Fiddler.Setup())
            {
                Console.WriteLine("Fiddler setup failed");
                return;
            }

            var startupSettings = new FiddlerCoreStartupSettingsBuilder()
                .ListenOnPort(9999)
                .DecryptSSL()
                .RegisterAsSystemProxy()
                .Build();

            FiddlerApplication.BeforeRequest += FiddlerHandlers.OnBeforeRequest;
            FiddlerApplication.BeforeResponse += FiddlerHandlers.OnBeforeResponse;

            AppDomain.CurrentDomain.ProcessExit += (sender, e) => Shutdown();
            Console.CancelKeyPress += (sender, e) => { Shutdown(); Environment.Exit(0); };

            Console.WriteLine("Starting fiddler application");
            FiddlerApplication.Startup(startupSettings);

            Backend.Listen();
            Console.WriteLine("Listening to backend");

            FortniteLauncher.TryLaunchPlayInFrontEnd();

            // プロセス監視スレッド
            new Thread(() =>
            {
                while (true)
                {
                    var processes = Process.GetProcessesByName("UnrealEditorFortnite-Win64-Shipping-PlayInFrontEnd");
                    if (processes.Length == 0)
                    {
                        Console.WriteLine("PlayInFrontEnd プロセスが終了しました。ツールを終了します。");
                        Shutdown();
                        Environment.Exit(0);
                    }
                    Thread.Sleep(3000);
                }
            })
            { IsBackground = true }.Start();

            Console.WriteLine("Starting PIE...");
            Console.WriteLine("To exit, press any key in this window to exit");
            Console.ReadKey(true);

            Shutdown();
        }

        private static void Shutdown()
        {
            Console.WriteLine("Shutting down fiddler application");
            FiddlerApplication.Shutdown();
            FortniteLauncher.KillFortniteProcess();
        }
    }
}
