using System;
using System.Collections.Generic;
using System.Linq;
using System.Text; 

namespace RC_FE_Design___Analysis_and_synthesis.SchemeEditor.Core
{
    public class IdCounter
    {
        private int count = 0;
        public int Count { get { return count; } }

        public void Reset()
        {
            count = 0;
        }

        public int Next()
        {
            return count++;
        }

        public void Set(int count)
        {
            this.count = count;
        }
    } 
}
