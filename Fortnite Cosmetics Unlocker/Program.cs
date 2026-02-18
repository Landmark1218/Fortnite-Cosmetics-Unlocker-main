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
        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        public delegate bool PHANDLER_ROUTINE(uint CtrlType);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool SetConsoleCtrlHandler(PHANDLER_ROUTINE HandlerRoutine, bool Add);

        // ガベージコレクションによってハンドラーが破棄されないよう、静的変数で保持します
        private static PHANDLER_ROUTINE _handler;

        private const uint CTRL_C_EVENT = 0;
        private const uint CTRL_BREAK_EVENT = 1;
        private const uint CTRL_CLOSE_EVENT = 2; // 「×」ボタン
        private const uint CTRL_LOGOFF_EVENT = 5;
        private const uint CTRL_SHUTDOWN_EVENT = 6;

        static void Main(string[] args)
        {
            _handler = new PHANDLER_ROUTINE(HandlerRoutine);
            SetConsoleCtrlHandler(_handler, true);

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

            // 通常の終了イベント（これらも残しておきます）
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
                    var processes = Process.GetProcessesByName("UnrealEditorFortnite-Win64-Shipping");
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

            // 入力待ち
            Console.ReadKey(true);

            Shutdown();
        }

       
        private static bool HandlerRoutine(uint type)
        {
            switch (type)
            {
                case CTRL_CLOSE_EVENT:
                case CTRL_LOGOFF_EVENT:
                case CTRL_SHUTDOWN_EVENT:
                    // コンソールが閉じられる際のクリーンアップ
                    Shutdown();
                    return false; // OSに終了処理を継続させる
                default:
                    return false;
            }
        }

        private static void Shutdown()
        {
            // 重複実行を防ぐため、一度だけ呼び出されるように配慮
            Console.WriteLine("Shutting down fiddler application and cleaning up...");
            FiddlerApplication.Shutdown();
            FortniteLauncher.KillFortniteProcess();
        }
    }
}