using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebRole1
{
    public class MyTrie
    {
        private MyTrieNode _root; 
        public MyTrie()
        {
            _root = new MyTrieNode();
        }

        public void Add(string word, int pageCount) { Add(word.ToLower().ToCharArray(), pageCount); }
        public void Add(char[] word, int pageCount)
        {
            _root.Add(word, 0, pageCount);
        }

        public List<string> GetWords(string prefix)
        {
            return _root.GetWords(_root, prefix);
        }

        public List<string> GetSuggestions(string prefix)
        {
            return _root.GetSuggestions(_root, prefix);
        }
    }
}