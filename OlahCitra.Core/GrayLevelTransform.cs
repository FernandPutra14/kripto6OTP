using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OlahCitra.Core
{
    public static class GrayLevelTransform
    {
        public static Func<int, int> Negative = (g) => 255 - g;
        public static Func<int, int> Identity = (g) => g;
    }
}
