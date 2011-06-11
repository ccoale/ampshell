using System;
using System.Collections.Generic;
using System.Text;

namespace Amp
{
    /// <summary>
    /// Stores information about the arguments that an internal command takes.
    /// </summary>
    public class ParameterInfo
    {
        /// <summary>
        /// A dictionary of short name conversions for parameter names.
        /// The key is the short name of the parameter, the value is the long name.
        /// I.e.
        /// ShortConversion.Add("fn", "filename");
        /// If a parameter does not exist in this dictionary, an error will be thrown.
        /// </summary>
        public Dictionary<string, string> ParameterNames = new Dictionary<string, string>();

        /// <summary>
        /// A dictionary of argument values that were passed to the command.
        /// Key is the longname version of the command.
        /// </summary>
        public Dictionary<string, string> ArgumentValues = new Dictionary<string, string>();

        /// <summary>
        /// Generic arguments with no names.
        /// </summary>
        public List<string> GenericArgs = new List<string>();
    }
}
