using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebRole1
{
    sealed class MyTrieNode : BurstNavigable
    {
        const int TRIE_WIDTH = 128;
        const int MINIMAL_WIDTH_USE = 32;  // standard ascii
        const int MAXIMAL_WIDTH_USE = 126;  // standard ascii
        public BurstNavigable[] _next;

        public void Add(string word, int pageCount) { Add(word.ToCharArray(), pageCount); }

        public override void Add(char[] word, int pageCount)
        {
            Add(word, 0, pageCount);
        }

        //To be used only by containers
        public bool ShouldBust()
        {
            return false;
        }

        public override void Add(char[] word, int start, int pageCount)
        {
            if (word.Length == start)
            {
                End = true;
                return;
            }
            int c = (int)word[start];

            if (_next == null)
                _next = new BurstNavigable[TRIE_WIDTH];

            BurstNavigable target = _next[c];

            if (target == null)
            {
                // always start with a bucket.
                target = new MyContainer();
                _next[c] = target;
            }
            else if (target.ShouldBurst)  // else OK because there's never any need to burst a brand new bucket
            {
                target = BurstThisContainer(target);
                _next[c] = target;
            }

            target.Add(word, start + 1, pageCount);
        }

        public override char getType()
        {
            return 'n';
        }

        public override BurstNavigable GetChild(int num)
        {
            return _next[num];
        }

        public override SortedSet<Word> GetChildren()
        {
            return null;
        }

        public MyTrieNode BurstThisContainer(BurstNavigable temp)
        {
            MyTrieNode node = new MyTrieNode();
            IEnumerator<Word> _enumerator = temp.GetChildren().GetEnumerator();
            while (_enumerator.MoveNext())
            {
                node.Add(_enumerator.Current.content.ToCharArray(), 0, _enumerator.Current.pageCount);
            }
            return node;
        }

        public List<string> GetWords(BurstNavigable temp, string word)
        {
            List<string> result = new List<string>();
            GetWords(temp, word.ToCharArray(),result, 0, "");
            return result;
        }

        private void GetWords(BurstNavigable temp, List<string> result, string resultString)
        {
            if (result.Count >= 10)
                return;
            if(temp == null)
            {
                return;
            }else
            {
                if (temp.End)
                {//if end then add
                    if (!result.Contains(resultString))
                        result.Add(resultString);
                }
                if (temp.getType() == 'n') //if node traverse every node in the children
                {
                    BurstNavigable[] array = temp.GetNexts();
                    for (int i = 0; i < array.Length; i++)
                    {
                        GetWords(array[i], result, resultString + (char)i);
                    }
                }
                else //if container, add into all the string inside the container
                {
                    IEnumerator<Word> _enumerator = temp.GetChildren().GetEnumerator(); ;
                    while (_enumerator.MoveNext())
                    {
                        if (result.Count >= 10)
                            break;
                        if (!result.Contains(resultString + _enumerator.Current.content))
                            result.Add(resultString + _enumerator.Current.content);

                    }
                }
            }
        }

        private void GetWords(BurstNavigable temp, char[] word, List<string> result, int start, string resultString)
        {
            if (temp == null)
                return;
            if (start == word.Length)
            {
                //if this is a node, traverse through each node of the children to get all the children nodes
                if(temp.getType() == 'n')
                {
                    if (temp.End)
                    {
                        if (!result.Contains(resultString))
                            result.Add(resultString);
                    }
                    if(result.Count >= 10)
                    {
                        return;
                    }
                    else
                    {
                        BurstNavigable[] array = temp.GetNexts();
                        for (int i = 0; i < array.Length; i++)
                        {
                            GetWords(array[i], result, resultString + (char) i );
                        }
                    }
                }else //if not just add all the words of the container into the list and return;
                {
                    IEnumerator<Word> _enumerator = temp.GetChildren().GetEnumerator(); ;
                    while (_enumerator.MoveNext())
                    {
                        if (result.Count >= 10)
                            break;
                        if (!result.Contains(resultString + _enumerator.Current.content))
                            result.Add(resultString + _enumerator.Current.content);
                    }
                }
            }
            else if(temp.getType() == 'n')
            {
                int c = (int)word[start];
                temp = temp.GetChild(c);
                if (temp == null)
                    return;
                GetWords(temp, word, result, start + 1, resultString + word[start]);
            }
            else
            {
                string left = "";
                for(int i = start; i < word.Length; i++)
                {
                    left = left + word[i];
                }
                IEnumerator<Word> _enumerator = temp.GetChildren().GetEnumerator(); ;
                while (_enumerator.MoveNext())
                {
                    if (result.Count >= 10)
                        break;
                    if (_enumerator.Current.content.StartsWith(left))
                        if (!result.Contains(resultString + _enumerator.Current.content))
                            result.Add(resultString + _enumerator.Current.content);
                }
            }
        }

        public override BurstNavigable[] GetNexts()
        {
            return _next;
        }

        public List<string> GetSuggestions(MyTrieNode temp, string prefix)
        {
            List<string> result = new List<string>();
            GetSuggestionsWithLimits(temp, result, "", prefix, prefix.Length, 0);
            return result;
        }

        //Will get back all the suggestions that is the same length with the prefix and within 1,2 Levenshtein Distance
        private void GetSuggestionsWithLimits(BurstNavigable temp, List<string> result, string resultString, string prefix, int limit, int start)
        {
            if (temp == null)
                return;
            if (result.Count >= 10)
                return;
            if (start == limit)
            {
                int distance = CalcLevenshteinDistance(prefix, resultString);
                if (temp.End && (distance == 1 || distance == 2))
                {
                    if (!result.Contains(resultString))
                        result.Add(resultString);
                    return;
                }
                if (distance == 1 || distance == 2)
                {
                    if (result.Count < 10)
                        result.Add(resultString);
                }
                return;
            }
            if (temp.getType() == 'n') //if node traverse every node in the children
            {
                BurstNavigable[] array = temp.GetNexts();
                for (int i = 0; i < array.Length; i++)
                {
                    GetSuggestionsWithLimits(array[i], result, resultString + (char)i, prefix, limit, start+1);
                }
            }
            else //if container, add into all the string inside the container
            {
                IEnumerator<Word> _enumerator = temp.GetChildren().GetEnumerator(); ;
                while (_enumerator.MoveNext())
                {
                    if (result.Count >= 10)
                        break;
                    string left = "";
                    for(int i = start; i < limit && i < _enumerator.Current.content.Length; i++)
                    {
                        left = left + _enumerator.Current.content[i];
                    }
                    resultString = resultString + left;
                    int distance = CalcLevenshteinDistance(prefix, resultString);
                    if (distance == 1 || distance == 2)
                    {
                        result.Add(resultString);
                    }
                }
            }
        }

        //method to caculate levenshtein distance between two words to generate suggestions 
        private int CalcLevenshteinDistance(string a, string b)
        {
            if (String.IsNullOrEmpty(a) || String.IsNullOrEmpty(b)) return 0;

            char[] charsInA = a.ToCharArray();
            char[] charsInB = b.ToCharArray();
            int lengthA = a.Length;
            int lengthB = b.Length;
            var distances = new int[lengthA + 1, lengthB + 1];
            for (int i = 0; i <= lengthA; i++)
            {
                distances[i, 0] = i;
            }
            for (int j = 0; j <= lengthB; j++)
            {
                distances[0, j] = j;
            }

            for (int i = 1; i <= lengthA; i++)
                for (int j = 1; j <= lengthB; j++)
                {
                    int cost = 0;
                    if (charsInB[j - 1] == charsInA[i - 1])
                        cost = 0;
                    else
                        cost = 1;

                    distances[i, j] = Math.Min
                        (
                        Math.Min(distances[i - 1, j] + 1, distances[i, j - 1] + 1),
                        distances[i - 1, j - 1] + cost
                        );
                }
            return distances[lengthA, lengthB];
        }
    }
}