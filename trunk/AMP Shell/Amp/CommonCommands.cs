using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Amp
{
    public class CommonCommands
    {
        /// <summary>
        /// Clears the screen.
        /// Alias: clear, cls
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        static public int ClearConsole(CommandLineInfo info, ParameterInfo args)
        {
            Console.Clear();
            return 0;
        }

        /// <summary>
        /// Lists all files and directories in the current working directory.
        /// If no argument is given, lists files in the current directory.
        /// Otherwise, in the specified directory.
        /// Alias: ls, dir
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        static public int ListFiles(CommandLineInfo info, ParameterInfo args)
        {
            return 0;
        }

        /// <summary>
        /// Change the directory.
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public static int ChangeDir(CommandLineInfo info, ParameterInfo args)
        {
            string dir = "";
            if (args.ArgumentValues.ContainsKey("directory")) {
                dir = args.ArgumentValues["directory"].Trim();
            }

            if (dir != "")
            {
                if (Directory.Exists(Path.GetFullPath(dir)))
                    Directory.SetCurrentDirectory(Path.GetFullPath(dir));
                else
                    Console.WriteLine("Directory does not exist.");
            }
            else if (args.GenericArgs.Count > 0)
            {
                dir = args.GenericArgs[0].Trim();
                if (dir != "")
                {
                    if (Directory.Exists(Path.GetFullPath(dir)))
                        Directory.SetCurrentDirectory(Path.GetFullPath(dir));
                    else
                        Console.WriteLine("Directory does not exist.");
                }
            }
            else
            {
                Console.WriteLine("Invalid command usage. Try help --command cd");
            }

            args.ArgumentValues.Clear();
            args.GenericArgs.Clear();
            return 0;
        }
    }
}
