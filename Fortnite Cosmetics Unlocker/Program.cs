using Fiddler;
using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;

namespace Fortnite_Cosmetics_Unlocker
{
    internal class Program
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool SetConsoleCtrlHandler(HandlerRoutine handler, bool add);

        private delegate bool HandlerRoutine(uint dwCtrlType);

        private const uint CTRL_C_EVENT = 0;
        private const uint CTRL_BREAK_EVENT = 1;
        private const uint CTRL_CLOSE_EVENT = 2; // 「×」ボタン
        private const uint CTRL_LOGOFF_EVENT = 5;
        private const uint CTRL_SHUTDOWN_EVENT = 6;

        private static bool ConsoleCtrlHandler(uint dwCtrlType)
        {
            switch (dwCtrlType)
            {
                case CTRL_C_EVENT:
                case CTRL_BREAK_EVENT:
                case CTRL_CLOSE_EVENT:
                    Shutdown();
                    return false;
                default:
                    return false;
            }
        }

        static void Main(string[] args)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Welcome to Cosmetics Unlocker For PIE!");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("Credit BiruFN,Landmark0920");
            Console.ResetColor();

            KillExistingFortnite();
            if (!CertMaker.createRootCert() || !CertMaker.trustRootCert())
            {
                Console.WriteLine("Failed to create/trust root certificate");
                return;
            }
            Console.WriteLine("Root certificate created successfully.");
            FiddlerApplication.BeforeRequest += FiddlerHandlers.OnBeforeRequest;
            FiddlerApplication.BeforeResponse += FiddlerHandlers.OnBeforeResponse;

            var handler = new HandlerRoutine(ConsoleCtrlHandler);
            SetConsoleCtrlHandler(handler, true);
            GC.KeepAlive(handler);

            var startupSettings = new FiddlerCoreStartupSettingsBuilder()
                .ListenOnPort(9999)
                .DecryptSSL()
                .RegisterAsSystemProxy()
                .Build();

            Console.WriteLine("Starting fiddler application");
            FiddlerApplication.Startup(startupSettings);
            Backend.Listen();
            Console.WriteLine("Listening to backend");
            FortniteLauncher.TryLaunchPlayInFrontEnd();

            // プロセス監視スレッド
            new Thread(() =>
            {
                var fortniteProcs = Process.GetProcessesByName("UnrealEditorFortnite-Win64-Shipping");
                while (fortniteProcs.Length > 0)
                {
                    Thread.Sleep(3000);
                    fortniteProcs = Process.GetProcessesByName("UnrealEditorFortnite-Win64-Shipping");
                }

                Console.WriteLine("Fortnite process ended. Shutting down...");
                Shutdown();
                Environment.Exit(0);
            })
            { IsBackground = true }.Start();

            Console.WriteLine("Starting PIE...");
            Console.WriteLine("To exit, press any key in this window to exit");

            // 入力待ち
            Console.ReadKey(true);

            Shutdown();
        }

        private static void KillExistingFortnite()
        {
            try
            {
                foreach (var proc in Process.GetProcessesByName("FortniteClient-Win64-Shipping"))
                {
                    proc.Kill();
                    Thread.Sleep(500);
                }
            }
            catch { }
        }

        private static void Shutdown()
        {
            Console.WriteLine("Cleaning up...");
            try
            {
                FiddlerApplication.Shutdown();
            }
            catch { }

            try
            {
                FortniteLauncher.KillFortniteProcess();
            }
            catch { }
        }
    }
}
