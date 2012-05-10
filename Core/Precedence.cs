using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Softweyr.Configuration
{
    public enum Precedence : int
    {
        /// <summary>
        /// The configuration method is configured globally (i.e. the same value would be for every machine on a WAN).
        /// </summary>
        Global = -1000000,

        /// <summary>
        /// The configuration method is configured at a site (i.e. the same value would be for every machine on a LAN).
        /// </summary>
        Site = -100000,

        /// <summary>
        /// The configuration method is configured for the entire machine (i.e. the same value would be used for all applications on this machine).
        /// </summary>
        Machine = 0,

        /// <summary>
        /// The configuration method is configured for all instances of this application. 
        /// </summary>
        Application = 100000,

        /// <summary>
        /// The configuration method is configured based on the user settings.
        /// </summary>
        User = 1000000
    }
}
