using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientWPF.Converters
{
    public static class StaticRandom
    {
        private static Random _random = new Random();
        public static int Random(int number) { return _random.Next(number); }
    }
}
