using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebRole1
{
    class TrieNode
    {
        private char data;
        private INodeCollection children;
        internal bool EndWord { get; set; }
        public int pageCount { get; set; }

        public char getCharacter()
        {
            return this.data;
        }

        public TrieNode()
        {
            data = ' ';
            children = new SingleNode(' ', this);
            EndWord = false;
        }

        public TrieNode(char data, int pageCount)
        {
            this.pageCount = pageCount;
            this.data = data;
            children = new SingleNode(data, this);
            EndWord = false;
        }

        internal List<TrieNode> GetChildren()
        {
            return children.GetNodes();
        }

        public TrieNode GetChild(char c)
        {
            TrieNode child;
            if (children.TryGetNode(c, out child))
                return child;
            else
                return null;
        }

        public void SetChild(char c, TrieNode node)
        {
            children = children.Add(c, node);
        }
    }

    interface INodeCollection
    {
        bool TryGetNode(char key, out TrieNode node);
        INodeCollection Add(char key, TrieNode node);
        List<TrieNode> GetNodes();
    }

    class SingleNode : INodeCollection
    {
        internal readonly char data;
        internal readonly TrieNode trie;

        public SingleNode(char key, TrieNode trie)
        {
            this.data = key;
            this.trie = trie;
        }

        public bool TryGetNode(char key, out TrieNode node)
        {
            node = null;
            return false;
        }

        public List<TrieNode> GetNodes()
        {
            List<TrieNode> result = new List<TrieNode>();
            //result.Add(trie);
            return result;
        }

        public INodeCollection Add(char key, TrieNode node)
        {
            SmallNodeCollection result = new SmallNodeCollection(this, key, node);
            return result;
        }
    }

    class SmallNodeCollection : INodeCollection
    {
        const int MaximumSize = 2;

        internal readonly List<KeyValuePair<char, TrieNode>> nodes;

        public SmallNodeCollection(SingleNode node, char key, TrieNode trie)
        {
            this.nodes = new List<KeyValuePair<char, TrieNode>>() { new KeyValuePair<char, TrieNode>(key, trie) };
        }

        public bool TryGetNode(char key, out TrieNode node)
        {
            foreach (KeyValuePair<char, TrieNode> item in nodes)
            {
                if (item.Key == key)
                {
                    node = item.Value;
                    return true;
                }
            }
            node = null;
            return false;
        }

        public List<TrieNode> GetNodes()
        {
            List<TrieNode> result = new List<TrieNode>();
            foreach (KeyValuePair<char, TrieNode> item in nodes)
            {
                result.Add(item.Value);
            }
            return result;
        }

        public INodeCollection Add(char key, TrieNode node)
        {
            KeyValuePair<char, TrieNode> newItem = new KeyValuePair<char, TrieNode>(key, node);
            nodes.Add(newItem);
            if (nodes.Count() > MaximumSize)
            {
                return new LargeNodeCollection(this, key, node);
            }
            else
            {
                return this;
            }
        }
        // Add adds to the list and returns the current instance until MaximumSize,
        // after which point it returns a LargeNodeCollection.
    }

    class LargeNodeCollection : INodeCollection
    {
        private readonly Dictionary<char, TrieNode> _nodes;

        public LargeNodeCollection(SmallNodeCollection nodes, char key, TrieNode trie)
        {
            _nodes = new Dictionary<char, TrieNode>();
            foreach (KeyValuePair<char, TrieNode> item in nodes.nodes)
            {
                _nodes.Add(item.Key, item.Value);
            }
        }

        public bool TryGetNode(char key, out TrieNode node)
        {
            if (_nodes.ContainsKey(key))
            {
                node = _nodes[key];
                return true;
            }
            else
            {
                node = null;
                return false;
            }
        }

        public List<TrieNode> GetNodes()
        {
            return _nodes.Values.ToList();
        }

        public INodeCollection Add(char key, TrieNode node)
        {
            _nodes.Add(key, node);
            return this;
        }
        // Add adds to the dictionary and returns the current instance.
    }
}