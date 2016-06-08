using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace pcb.core.autocomplete
{
    public class AutocompleteParseException : Exception
    {
        public AutocompleteParseException() : base("unknown error has occured when parsing autocomplete script") { }
        public AutocompleteParseException(string message) : base(message) { }
        public AutocompleteParseException(string message, Exception innerException) : base(message, innerException) { }
    }
}
