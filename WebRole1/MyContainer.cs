using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebRole1
{
    sealed class MyContainer : BurstNavigable
    {
        public static int BurstThreshold = 32;

        private SortedSet<Word> _records;

        public MyContainer()
        {
            _records = new SortedSet<Word>(new WordComparer());
        }

        public override char getType()
        {
            return 'c';
        }

        public override BurstNavigable GetChild(int num)
        {
            throw new NotImplementedException();
        }

        public override void Add(char[] word, int start, int pageCount)
        {
            if (word.Length == start)
            {
                End = true;
                return;
            }
            string result = "";
            for(int i = start; i < word.Length; i++)
            {
                result = result + word[i];
            }
            _records.Add(new Word(result, pageCount));
            if (_records.Count >= BurstThreshold)
                ShouldBurst = true;
        }


        public bool shouldBust()
        {
            return _records.Count >= BurstThreshold;
        }

        public void Add(string word, int pageCount)
        {
            _records.Add(new Word(word, pageCount));
            if (_records.Count >= BurstThreshold)
                ShouldBurst = true;
        }

        //Method only tobe used by the node class
        public override BurstNavigable[] GetNexts()
        {
            return null;
        }

        //TODO here how to traverse though a list of objects 
        //public bool contains(string key)
        //{
        //    return _records.Contains(key);
        //}

        public override SortedSet<Word> GetChildren()
        {
            return _records;
        }

        public override void Add(char[] word, int pageCount)
        {
            throw new NotImplementedException();
        }
    }

}