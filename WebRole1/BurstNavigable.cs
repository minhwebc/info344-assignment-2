using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebRole1
{
    [Serializable]
    abstract class BurstNavigable
    {
        public abstract void Add(char[] word, int pageCount);
        public abstract void Add(char[] word, int start, int pageCount);
        public abstract BurstNavigable[] GetNexts();
        public abstract SortedSet<Word> GetChildren();
        public abstract char getType();
        public abstract BurstNavigable GetChild(int num);
        //public abstract List<string> GetWords(string prefix);
        public bool ShouldBurst;
        public bool End = false; // note: can save a lot of space by moving this to a static / separate 'ends' hash/flag set OR
        // using a special termination slot in the next[] array.
    }
}