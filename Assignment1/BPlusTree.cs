using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assignment1
{
    class BPlusTree
    {
        public BPlusTree()
        {
            root = new node(true, true);
        }
        public void insert(dynamic key)
        {
            root.insert(key, this);
        }

        public bool find(dynamic key)
        {
            return root.search(key);
        }

        public void setRoot(node root)
        {
            this.root = root;
        }

        public void printAll()
        {
            root.printAll();
        }

        internal void findLowerBound(HashSet<dynamic> ary, int v)
        {
            root.findLowerBound(ary, v);
        }

        internal void findUpperBound(HashSet<dynamic> ary, int v)
        {
            root.findUpperBound(ary, v);
        }

        private node root;
    }

    class node
    {
        static int ID = 0;
        public node(bool isRoot, bool isleaf)
        {
            id = ID;
            ID++;
            root = isRoot;
            leaf = isleaf;
            parent = null;
            childs = new List<node>(min_degree);
            keys = new SortedSet<dynamic>();
        }
        public int id;
        public bool root;
        public bool leaf;
        public node parent;
        public List<node> childs;
        public SortedSet<dynamic> keys;
        public int min_degree = 31;

        internal bool search(dynamic key)
        {
            List<dynamic> l = new List<dynamic>(keys);
            if (leaf)
            {
                for (int i = 0; i < keys.Count; i++)
                {
                    if (l[i] == key) return true;
                }
                return false;
            }
            for (int i = 0; i < keys.Count; i++)
            {
                if (key < l[i])
                {
                    return childs[i].search(key);
                }
            }
            return childs[keys.Count].search(key);
        }

        internal void insert(dynamic key, BPlusTree b)
        {
            if (!leaf)
            {
                bool getIn = true;
                int i = 0;
                List<dynamic> l = new List<dynamic>(keys);
                foreach (dynamic d in l)
                {
                    if (key < d)
                    {
                        getIn = false;
                        childs[i].insert(key, b);
                        break;
                    }
                    i++;
                }
                if (getIn) childs[i].insert(key, b);
            }
            else
            {
                keys.Add(key);
                if (keys.Count == min_degree)
                {
                    List<dynamic> l = new List<dynamic>(keys);
                    if (root)
                    {
                        dynamic middle = l[keys.Count / 2];
                        node newRoot = new node(true, false);
                        newRoot.keys.Add(middle);
                        node left = new node(false, true);
                        node right = new node(false, true);
                        for (int i = 0; i < keys.Count / 2; i++)
                        {
                            left.keys.Add(l[i]);
                        }
                        for (int i = keys.Count / 2; i < keys.Count; i++)
                        {
                            right.keys.Add(l[i]);
                        }
                        left.parent = newRoot;
                        right.parent = newRoot;
                        newRoot.childs.Add(left);
                        newRoot.childs.Add(right);
                        b.setRoot(newRoot);
                    }
                    else
                    {
                        dynamic middle = l[keys.Count / 2];
                        SortedSet<dynamic> left = new SortedSet<dynamic>();
                        node right = new node(false, true);
                        for (int i = keys.Count / 2; i < keys.Count; i++)
                        {
                            right.keys.Add(l[i]);
                        }
                        right.parent = parent;
                        for (int i = 0; i < keys.Count / 2; i++)
                        {
                            left.Add(l[i]);
                        }
                        this.keys = left;

                        parent.parentInsert(middle, right, b);
                    }
                }
            }
        }

        private void parentInsert(dynamic key, node rright, BPlusTree b)
        {
            keys.Add(key);
            List<dynamic> l = new List<dynamic>(keys);
            childs.Insert(l.IndexOf(key) + 1, rright);
            if (keys.Count == min_degree)
            {
                if (root)
                {
                    dynamic middle = l[keys.Count / 2];
                    node newRoot = new node(true, false);
                    newRoot.keys.Add(middle);
                    node left = new node(false, false);
                    node right = new node(false, false);
                    for (int i = 0; i < keys.Count / 2; i++)
                    {
                        left.keys.Add(l[i]);
                    }
                    for (int i = keys.Count / 2 + 1; i < keys.Count; i++)
                    {
                        right.keys.Add(l[i]);
                    }
                    for (int i = 0; i < childs.Count / 2; i++)
                    {
                        childs[i].parent = left;
                        left.childs.Add(childs[i]);
                    }
                    for (int i = childs.Count / 2; i < childs.Count; i++)
                    {
                        childs[i].parent = right;
                        right.childs.Add(childs[i]);
                    }
                    left.parent = newRoot;
                    right.parent = newRoot;
                    newRoot.childs.Add(left);
                    newRoot.childs.Add(right);
                    b.setRoot(newRoot);
                }
                else
                {
                    dynamic middle = l[keys.Count / 2];
                    SortedSet<dynamic> left = new SortedSet<dynamic>();
                    List<node> dick = new List<node>();
                    node right = new node(false, false);
                    for (int i = keys.Count / 2 + 1; i < keys.Count; i++)
                    {
                        right.keys.Add(l[i]);
                    }
                    for (int i = 0; i < keys.Count / 2; i++)
                    {
                        left.Add(l[i]);
                    }
                    this.keys = left;
                    for (int i = childs.Count / 2; i < childs.Count; i++)
                    {
                        childs[i].parent = right;
                        right.childs.Add(childs[i]);
                    }
                    for (int i = 0; i < childs.Count / 2; i++)
                    {
                        childs[i].parent = this;
                        dick.Add(childs[i]);
                    }
                    this.childs = dick;
                    right.parent = parent;
                    parent.parentInsert(middle, right, b);
                }
            }
        }

        internal void printAll()
        {
            Console.Write(("/ID:" + this.id).PadRight(7));
            Console.Write(("/root:" + this.root).PadRight(12));
            Console.Write(("/leaf:" + this.leaf).PadRight(12));
            if (!root) Console.Write(("/parentID:" + this.parent.id).PadRight(12));
            Console.Write("/keys:");
            string tmp = "";
            foreach (dynamic a in keys)
            {
                tmp = tmp + a + " ";
            }
            Console.Write(tmp.PadRight(15) + "/");
            Console.Write("Childs:");
            tmp = "";
            foreach (node a in childs)
            {
                tmp = tmp + a.id + " ";
            }
            Console.Write(tmp.PadRight(15) + "/");
            Console.WriteLine("");
            foreach (node a in childs)
            {
                a.printAll();
            }
        }

        internal void findLowerBound(HashSet<dynamic> ary, int v)
        {
            List<dynamic> l = new List<dynamic>(keys);
            if (!leaf)
            {
                int i;
                for (i = 0; i < l.Count && v > l[i]; i++) ;
                for(int j = i; j < childs.Count; j++)
                {
                    childs[j].findLowerBound(ary, v);
                }
            }
            else
            {
                for (int i = 0; i < l.Count; i++)
                {
                    if (v < l[i])
                    {
                        ary.Add(l[i]);
                    }
                }
            }
        }

        internal void findUpperBound(HashSet<dynamic> ary, int v)
        {
            List<dynamic> l = new List<dynamic>(keys);
            if (!leaf)
            {
                int i;
                for (i = 0; i < l.Count && v > l[i]; i++) ;
                for (int j = 0; j < i+1; j++)
                {
                    childs[j].findUpperBound(ary, v);
                }
            }
            else
            {
                for (int i = 0; i < l.Count; i++)
                {
                    if (v > l[i])
                    {
                        ary.Add(l[i]);
                    }
                }
            }
        }
    }
}
