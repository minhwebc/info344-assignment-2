using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebRole1
{
    class Trie
    {
        private TrieNode root;

        public Trie()
        {
            root = new TrieNode();
        }

        public void addWord(string word, int pageCount)
        {
            if (word == null)
            {
                throw new ArgumentNullException(nameof(word));
            }
            word = word.ToLower();
            AddWord(root, word.ToCharArray(), 0, pageCount);
        }

        private void AddWord(TrieNode node, char[] word, int index, int pageCount)
        {
            if (index >= word.Length)
            {
                node.pageCount = pageCount;
                node.EndWord = true;
                return;
            }
            else
            {
                var child = node.GetChild(word[index]);
                if (child == null)
                {
                    child = new TrieNode(word[index], pageCount);
                    node.SetChild(word[index], child);
                }
                if (child.pageCount < pageCount)
                    child.pageCount = pageCount;
                AddWord(child, word, index + 1, pageCount);
            }
        }

        private TrieNode GetTrieNode(string prefix)
        {
            TrieNode current = root;
            foreach (var character in prefix)
            {
                Console.WriteLine(character);
                current = current.GetChild(character);
                if (current == null)
                    break;
            }
            return current;
        }

        public List<string> GetWords(string prefix)
        {
            if (prefix == null)
            {
                throw new ArgumentNullException(nameof(prefix));
            }
            // Empty list if no prefix match
            //Console.WriteLine(prefix);
            var words = new List<Word>();
            prefix = prefix.ToLower();
            GetWords(GetTrieNode(prefix), words, prefix);
            words = words.OrderByDescending(o => o.pageCount).ToList();
            List<string> result = new List<string>();
            words.ForEach(i => result.Add(i.content));
            if (result.Count < 10)
            {
                result = GetPrefixSuggestions(prefix.Length, prefix);
            }
            return result;
        }

        public List<string> GetPrefixSuggestions(int length, string input)
        {
            List<string> result = new List<string>();
            TrieNode current = root;
            GetWordsWithLimit(result, root, length, 0, "", input);
            return result;
        }

        private void GetWordsWithLimit(List<string> result, TrieNode current, int limit, int index, string word, string input)
        {
            if (result.Count >= 10)
                return;
            if (index > limit)
            {
                Console.WriteLine("dealing with word " + word + " input " + input);
                int distance = CalcLevenshteinDistance(word.Trim(), input);
                if ((distance == 1 || distance == 2) && !result.Contains(word))
                    result.Add(word);
                else
                    return;
            }
            else if (current == null)
            {
                return;
            }
            else
            {
                foreach (var child in current.GetChildren())
                {
                    GetWordsWithLimit(result, child, limit, index + 1, word + current.getCharacter(), input);
                }
            }
        }

        private void GetWords(TrieNode node, List<Word> words, string result)
        {
            if (words.Count >= 10)
                return;
            else if (node == null)
            {
                return;
            }
            else 
            {
                if (node.EndWord)
                {
                    Word newWord = new Word(result, node.pageCount);
                    words.Add(newWord);
                }
                List<TrieNode> children = node.GetChildren();
                children = children.OrderByDescending(o => o.pageCount).ToList();
                foreach (var child in children)
                {
                    GetWords(child, words, result + child.getCharacter());

                }
            }
        }

        private static int CalcLevenshteinDistance(string a, string b)
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