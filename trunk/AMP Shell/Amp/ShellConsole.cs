using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Security;
using System.Security.Principal;
using System.Runtime;
using System.Diagnostics;
using System.Threading;

namespace Amp
{
    public class ShellConsole
    {
        /// <summary>
        /// The current working directory of the shell.
        /// </summary>
        private DirectoryInfo CurrentDirectory = null;

        /// <summary>
        /// A list of commands.
        /// </summary>
        private List<CommandInfo> Commands = new List<CommandInfo>();

        public ShellConsole()
        {
            CurrentDirectory = new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments));
            Directory.SetCurrentDirectory(CurrentDirectory.FullName);
        }

        public void RegisterCommand(string[] names, ParamPair[] paramNames, CommandInfo.CommandDelegate handler)
        {
            
            CommandInfo info = new CommandInfo();
            foreach (string name in names)
                info.Names.Add(name);
            info.Handler = handler;
            info.IsInternal = true;
            // { {"fn", "filename"}, {"dp", "dirpath"} }
            // { {00, 01}, {10, 11} }
            foreach (ParamPair pair in paramNames)
            {
                info.ParameterInfo.ParameterNames[pair.First] = pair.Second;
            }

            Commands.Add(info);
        }

        /// <summary>
        /// Displays the command prompt.
        /// </summary>
        public void DisplayPrompt()
        {
            // gets the current working directory
            CurrentDirectory = new DirectoryInfo(Path.GetFullPath(Directory.GetCurrentDirectory()));

            string directory = "";
            string prompt = "";
            if (CurrentDirectory == null)
                directory = "~";
            else
                directory = CurrentDirectory.FullName;

            string userInfo = WindowsIdentity.GetCurrent().Name;
            string[] userParts = userInfo.Split(new char[] { '\\' }, StringSplitOptions.RemoveEmptyEntries);
            prompt = userParts[1].ToLower() + "@" + userParts[0].ToLower() + ": " + directory + "$ ";
            Console.Write(prompt);
        }

        /// <summary>
        /// Reads a command-line string. This function will not return until a valid command line is entered.
        /// </summary>
        /// <returns></returns>
        public string ReadCommandLine()
        {
            string line = Console.ReadLine().Trim();
            while (line.Length <= 0)
            {
                DisplayPrompt();
                line = Console.ReadLine().Trim();
            }

            return line;
        }

        /// <summary>
        /// Parses a read command-line.
        /// </summary>
        /// <param name="commandLine">The command-line to parse</param>
        /// <param name="args">The arguments that were parsed from the command-line</param>
        /// <returns>The name of the command</returns>
        public CommandLineInfo ParseCommandLine(string commandLine, ref List<string> args)
        {
            CommandLineInfo info = new CommandLineInfo();
            info.RawCommandLine = commandLine;
            commandLine = commandLine.Trim();
            if (commandLine.Length <= 0)
                return null;

            // create our argument list
            args = new List<string>();

            string commandPart = "";
            //string tempArgPart = "";
            int strLen = commandLine.Length;

            //int startSub = 0;
            //int endSub = 0;

            int i = 0;

            // first we are looking for the command-name.
            for (i = 0; i < strLen; i++)
            {
                if (commandLine[i] == ' ')
                    break;
            }
            commandPart = commandLine.Substring(0, i);
            info.CommandName = commandPart;

            string argString = commandLine.Substring(i).Trim();
            info.RawArgumentLine = argString;
            strLen = argString.Length;
            bool inQuotes = false;
            int lastStartIndex = 0;
            for (i = 0; i < strLen; i++)
            {
                if (argString[i] == '"')
                {
                    if (inQuotes)
                    {
                        string singleArgStr = argString.Substring(lastStartIndex + 1, i - (lastStartIndex + 1));
                        lastStartIndex = i + 1;
                        inQuotes = false;
                        args.Add(singleArgStr);
                    }
                    else
                    {
                        inQuotes = true;
                        lastStartIndex = i;
                    }
                }
                else if (Char.IsWhiteSpace(argString[i]) && !inQuotes)
                {
                    if (i - lastStartIndex <= 0)
                        continue;

                    string singleArgStr = argString.Substring(lastStartIndex, i - (lastStartIndex));
                    lastStartIndex = i + 1;
                    args.Add(singleArgStr);
                }
            }

            if (i - lastStartIndex > 0)
            {
                string singleArgStr = argString.Substring(lastStartIndex, i - (lastStartIndex));
                args.Add(singleArgStr);
            }

            foreach (string s in args)
                info.Arguments.Add(s);

            return info;
        }

        /// <summary>
        /// This method is called when a child process writes data to output or error.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            Console.WriteLine(e.Data);
        }

        /// <summary>
        /// Handles a command by executing it with the given arguments.
        /// </summary>
        /// <param name="commandName">The name of the command to execute</param>
        /// <param name="args">The list of arguments to pass to the command</param>
        /// <returns>The integer result of the command.</returns>
        public int HandleCommand(CommandLineInfo info)
        {
            if (info == null)
                return -1;

            // First we need to find the command to actually execute.
            string commandToFind = info.CommandName;
            ParameterInfo paramInfo = new ParameterInfo();

            // First let's check if it is a predefined/loaded command.
            foreach (CommandInfo command in Commands)
            {
                if (command.IsCommandName(commandToFind))
                {
                    paramInfo = command.ParameterInfo;
                    // we need to parse out the list of arguments based on long-name/short-name
                    for (int i = 0; i < info.Arguments.Count; i++)
                    {
                        if (info.Arguments[i].StartsWith("--") && (i < info.Arguments.Count - 1))
                        {
                            // it's already a long-name
                            string realArgName = info.Arguments[i].Substring(2);
                            string realArgValue = info.Arguments[i + 1];
                            i++;

                            paramInfo.ArgumentValues[realArgName] = realArgValue;
                        }
                        else if (info.Arguments[i].StartsWith("-") && (i < info.Arguments.Count - 1))
                        {
                            string realArgName = info.Arguments[i].Substring(1);
                            string realArgValue = info.Arguments[i + 1];
                            i++;

                            paramInfo.ArgumentValues[paramInfo.ParameterNames[realArgName]] = realArgValue;
                        }
                        else
                        {
                            paramInfo.GenericArgs.Add(info.Arguments[i]);
                        }
                    }

                    return command.ExecuteCommand(info, paramInfo);
                }
            }

            // It wasn't a predefined command, let's check to see if it is a file path
            string fullPathTest = Path.GetFullPath(commandToFind);
            if (fullPathTest.IndexOfAny(Path.GetInvalidPathChars()) == -1)
            {
                if (Directory.Exists(fullPathTest))
                {
                    Console.WriteLine(fullPathTest + " is a directory.");
                    return 0;
                }
            }
            if (File.Exists(fullPathTest))
            {
                ProcessStartInfo procInfo = new ProcessStartInfo(fullPathTest,
                    info.RawArgumentLine);
                procInfo.RedirectStandardError = true;
                procInfo.RedirectStandardInput = true;
                procInfo.RedirectStandardOutput = true;
                procInfo.WorkingDirectory = Path.GetFullPath(Directory.GetCurrentDirectory());
                procInfo.UseShellExecute = false;
                procInfo.WindowStyle = ProcessWindowStyle.Hidden;
                procInfo.CreateNoWindow = true;
                Process proc = new Process();
                proc.StartInfo = procInfo;
                //proc.OutputDataReceived += (s, e) => { Console.Write(e.Data); proc.StandardInput.Flush(); };
                //proc.ErrorDataReceived += (s, e) => { Console.Write(e.Data); proc.StandardInput.Flush(); };
                proc.Start();

                //Stream stream = Console.OpenStandardInput();
                bool isReading = false;

                Thread readerThread = new Thread(() =>
                    {
                        while (true)
                        {
                            int res = proc.StandardOutput.BaseStream.ReadByte();
                            if (res > -1)
                            {
                                Console.Write((char)res);
                                isReading = true;
                            }
                            else
                            {
                                isReading = false;
                            }
                        }
                    });
                Thread errorThread = new Thread(() =>
                    {
                        while (true)
                        {
                             if (!isReading)
                            {
                                proc.StandardError.BaseStream.Flush();
                                int res = proc.StandardError.BaseStream.ReadByte();
                                if (res > -1) { Console.Write((char)res); }
                            }
                        }
                    });

                readerThread.Start();
                errorThread.Start();

                while (!proc.HasExited)
                {
                    proc.StandardInput.Write((char)Console.Read());
                }

                readerThread.Abort();
                errorThread.Abort();
                // redisplay our prompt!
                Console.WriteLine("");
                Console.Out.Flush();

                DisplayPrompt();
            }

            return -1;
        }
    }
}
