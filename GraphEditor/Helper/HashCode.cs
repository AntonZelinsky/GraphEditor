using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphEditor.Helper
{
    public static class HashCode
    {
        public static int GetHashCode(int fromId, int toId)
        {
            int salt = 100;
            unchecked
            {
                return fromId * toId + salt;
            }
        }
    }
}
