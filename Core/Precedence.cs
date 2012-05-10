using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Softweyr.Configuration
{
    public enum Precedence : int
    {
        VeryLow = -1000000,
        Low = -100000,
        Medium = 0,
        High = 100000,
        VeryHigh = 1000000
    }
}
