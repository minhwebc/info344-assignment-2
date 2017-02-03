using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebRole1
{
    public class Word
    {
        public string content { get; set; }
        public int pageCount { get; set; }

        public Word(string content, int pageCount)
        {
            this.content = content;
            this.pageCount = pageCount;
        }

        public int CompareTo(Word obj)
        {
            return this.pageCount - obj.pageCount;
        }
    }

    public class WordComparer : IComparer<Word>
    {
        public int Compare(Word x, Word y)
        {
            // TODO: Handle x or y being null, or them not having names
            int result = x.pageCount - y.pageCount;
            if (result < 0)
            {
                return 1;
            }
            else if (result > 0)
            {
                return -1;
            }
            else
            {
                return x.content.CompareTo(y.content);
            }
        }
    }
}