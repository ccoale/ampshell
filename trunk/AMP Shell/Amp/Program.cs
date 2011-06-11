using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Amp
{
    class Program
    {
        

        public static void DisplayLogo()
        {
            OperatingSystem systemInfo = Environment.OSVersion;

            Console.WriteLine("AMP Shell 0.1.0");
            Console.WriteLine("Copyright (c) 2010 Blox Software, All Rights Reserved.");
            Console.WriteLine(systemInfo.ToString());
            Console.WriteLine("");
        }

        static void Main(string[] args)
        {
            List<string> commandArgs = new List<string>();
            DisplayLogo();
            ShellConsole console = new ShellConsole();
            console.RegisterCommand(new string[] { "cd" },
                new ParamPair[]
                {
                    new ParamPair("d", "directory")
                },
                CommonCommands.ChangeDir
                );
            while (true)
            {
                console.DisplayPrompt();
                string str = console.ReadCommandLine();
                CommandLineInfo info = console.ParseCommandLine(str, ref commandArgs);
                console.HandleCommand(info);
            }
        }
    }
}
