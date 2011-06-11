using System;
using System.Collections.Generic;
using System.Text;

namespace Amp
{
    /// <summary>
    /// Represents a parameter touple.
    /// i.e. "fn", "filename"
    /// </summary>
    public class ParamPair
    {
        /// <summary>
        /// The first string in the touple.
        /// </summary>
        public string First = "";

        /// <summary>
        /// The second string in the touple.
        /// </summary>
        public string Second = "";

        public ParamPair(string first, string second)
        {
            First = first;
            Second = second;
        }
    }
}
