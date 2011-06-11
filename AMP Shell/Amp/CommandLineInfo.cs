using System;
using System.Collections.Generic;
using System.Text;

namespace Amp
{
    /// <summary>
    /// Provides information about a parsed command-line.
    /// </summary>
    public class CommandLineInfo
    {
        /// <summary>
        /// A normalized (all lowercase, no whitespace) command name.
        /// </summary>
        public string CommandName = "";

        /// <summary>
        /// A list of arguments to pass to the command.
        /// </summary>
        public List<string> Arguments = new List<string>();

        /// <summary>
        /// The raw, as typed, command line.
        /// </summary>
        public string RawCommandLine = "";

        /// <summary>
        /// The raw, as typed, argument line.
        /// </summary>
        public string RawArgumentLine = "";
    }
}
