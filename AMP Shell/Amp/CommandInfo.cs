using System;
using System.Collections.Generic;
using System.Text;

namespace Amp
{
    /// <summary>
    /// Stores information about a command.
    /// </summary>
    public class CommandInfo
    {
        public delegate int CommandDelegate(CommandLineInfo info, ParameterInfo args);

        /// <summary>
        /// The list of names of the command (i.e. aliases).
        /// </summary>
        public List<string> Names = new List<string>();

        /// <summary>
        /// The list of parameters.
        /// </summary>
        public ParameterInfo ParameterInfo = new ParameterInfo();

        /// <summary>
        /// Whether or not this command is internal.
        /// Internal commands are callback/delegates whereas
        /// external commands are executable files.
        /// External commands will only have ONE alias.
        /// </summary>
        public bool IsInternal = false;

        /// <summary>
        /// The actually handler, if this command is an internal command.
        /// </summary>
        public CommandDelegate Handler = null;

        /// <summary>
        /// Checks if this command is equal based on a name.
        /// </summary>
        /// <param name="name">The name to check</param>
        /// <returns>True if it is equal, false otherwise.</returns>
        public bool IsCommandName(string name)
        {
            foreach (string str in Names)
            {
                if (name == str)
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Executes this command with the given arguments.
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public int ExecuteCommand(CommandLineInfo info, ParameterInfo args)
        {
            if (IsInternal)
            {
                if (Handler == null)
                    return -1;
                else
                    return Handler(info, args);
            }

            return -1;
        }
    }
}
