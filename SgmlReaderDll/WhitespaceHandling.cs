using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SgmlReaderDll
{
#if WINDOWS_UWP
    //
    // Summary:
    //     Specifies how white space is handled.
    //     (Not defined in PORTABLE framework, so we added it here).
    public enum WhitespaceHandling
    {
        //
        // Summary:
        //     Return Whitespace and SignificantWhitespace nodes. This is the default.
        All = 0,
        //
        // Summary:
        //     Return SignificantWhitespace nodes only.
        Significant = 1,
        //
        // Summary:
        //     Return no Whitespace and no SignificantWhitespace nodes.
        None = 2
    }
#endif
}
