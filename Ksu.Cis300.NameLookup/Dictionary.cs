/* Dictionary.cs
 * Author: Ryan Smith
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KansasStateUniversity.TreeViewer2;
using Ksu.Cis300.ImmutableBinaryTrees;

namespace Ksu.Cis300.NameLookup
{
    /// <summary>
    /// A generic dictionary in which keys must implement IComparable.
    /// </summary>
    /// <typeparam name="TKey">The key type.</typeparam>
    /// <typeparam name="TValue">The value type.</typeparam>
    public class Dictionary<TKey, TValue> where TKey : IComparable<TKey>
    {
        /// <summary>
        /// The keys and values in the dictionary.
        /// </summary>
        private BinaryTreeNode<KeyValuePair<TKey, TValue>> _elements = null;

        /// <summary>
        /// Gets a drawing of the underlying binary search tree.
        /// </summary>
        public TreeForm Drawing => new TreeForm(_elements, 100);

        /// <summary>
        /// Checks to see if the given key is null, and if so, throws an
        /// ArgumentNullException.
        /// </summary>
        /// <param name="key">The key to check.</param>
        private static void CheckKey(TKey key)
        {
            if (key == null)
            {
                throw new ArgumentNullException();
            }
        }

        /// <summary>
        /// Finds the given key in the given binary search tree.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="t">The binary search tree.</param>
        /// <returns>The node containing key, or null if the key is not found.</returns>
        private static BinaryTreeNode<KeyValuePair<TKey, TValue>> Find(TKey key, BinaryTreeNode<KeyValuePair<TKey, TValue>> t)
        {
            if (t == null)
            {
                return null;
            }
            else
            {
                int comp = key.CompareTo(t.Data.Key);
                if (comp == 0)
                {
                    return t;
                }
                else if (comp < 0)
                {
                    return Find(key, t.LeftChild);
                }
                else
                {
                    return Find(key, t.RightChild);
                }
            }
        }

        /// <summary>
        /// Builds the binary search tree that results from adding the given key and value to the given tree.
        /// If the tree already contains the given key, throws an ArgumentException.
        /// </summary>
        /// <param name="t">The binary search tree.</param>
        /// <param name="k">The key.</param>
        /// <param name="v">The value.</param>
        /// <returns>The binary search tree that results from adding k and v to t.</returns>
        private static BinaryTreeNode<KeyValuePair<TKey, TValue>> Add(BinaryTreeNode<KeyValuePair<TKey, TValue>> t, TKey k, TValue v)
        {
            if (t == null)
            {
                return new BinaryTreeNode<KeyValuePair<TKey, TValue>>(new KeyValuePair<TKey, TValue>(k, v), null, null);
            }
            else
            {
                int comp = k.CompareTo(t.Data.Key);
                if (comp == 0)
                {
                    throw new ArgumentException();
                }
                else if (comp < 0)
                {
                    return new BinaryTreeNode<KeyValuePair<TKey, TValue>>(t.Data, Add(t.LeftChild, k, v), t.RightChild);
                }
                else
                {
                    return new BinaryTreeNode<KeyValuePair<TKey, TValue>>(t.Data, t.LeftChild, Add(t.RightChild, k, v));
                }
            }
        }

        /// <summary>
        /// Tries to get the value associated with the given key.
        /// </summary>
        /// <param name="k">The key.</param>
        /// <param name="v">The value associated with k, or the default value if
        /// k is not in the dictionary.</param>
        /// <returns>Whether k was found as a key in the dictionary.</returns>
        public bool TryGetValue(TKey k, out TValue v)
        {
            CheckKey(k);
            BinaryTreeNode<KeyValuePair<TKey, TValue>> p = Find(k, _elements);
            if (p == null)
            {
                v = default(TValue);
                return false;
            }
            else
            {
                v = p.Data.Value;
                return true;
            }
        }

        /// <summary>
        /// Adds the given key with the given associated value.
        /// If the given key is already in the dictionary, throws an
        /// InvalidOperationException.
        /// </summary>
        /// <param name="k">The key.</param>
        /// <param name="v">The value.</param>
        public void Add(TKey k, TValue v)
        {
            CheckKey(k);
            _elements = Add(_elements, k, v);
        }

        private static BinaryTreeNode<KeyValuePair<TKey, TValue>> RemoveMininumKey(BinaryTreeNode<KeyValuePair<TKey, TValue>> t, out KeyValuePair<TKey, TValue> min)
        {
            BinaryTreeNode<KeyValuePair<TKey, TValue>> tempTree = null;

            if (t.LeftChild != null)
            {
                tempTree = new BinaryTreeNode<KeyValuePair<TKey, TValue>>(t.Data, RemoveMininumKey(t.LeftChild, out min), t.RightChild);
                return tempTree;

            }
            else if (t.RightChild != null)
            {
                min = t.Data;
                tempTree = new BinaryTreeNode<KeyValuePair<TKey, TValue>>(t.Data, null, t.RightChild);
                return tempTree;
            }
            else
            {
                min = t.Data;
                return tempTree = null;
            }
        }

        private static BinaryTreeNode<KeyValuePair<TKey, TValue>> Remove(TKey key, BinaryTreeNode<KeyValuePair<TKey, TValue>> t, out bool removed)
        {

            if (key == null)
            {
                throw new ArgumentNullException();
            }
            if (t == null)
            {
                removed = false;
                return t;
            }

            BinaryTreeNode<KeyValuePair<TKey, TValue>> returnTree = null;
            int compareKey = key.CompareTo(t.Data.Key);

            
            if (compareKey > 0)
            {
                returnTree = new BinaryTreeNode<KeyValuePair<TKey, TValue>>(t.Data, t.LeftChild, Remove(key, t.RightChild, out removed));
                return returnTree;
            }
            else if (compareKey < 0)
            {
                returnTree = new BinaryTreeNode<KeyValuePair<TKey, TValue>>(t.Data, Remove(key, t.LeftChild, out removed), t.RightChild );
                return returnTree;
            }
            else
            {
                KeyValuePair<TKey, TValue> min;
                removed = true;
                if (t.RightChild != null)
                {   
                    BinaryTreeNode<KeyValuePair<TKey, TValue>> tempNode = RemoveMininumKey(t.RightChild, out min);
                    returnTree = new BinaryTreeNode<KeyValuePair<TKey, TValue>>(min, t.LeftChild, tempNode);
                }
                else if (t.LeftChild != null)
                {
                    BinaryTreeNode<KeyValuePair<TKey, TValue>> tempNode = RemoveMininumKey(t.LeftChild, out min);
                    returnTree = new BinaryTreeNode<KeyValuePair<TKey, TValue>>(min, tempNode, t.RightChild);
                }
                else
                {
                    returnTree = null;
                }
                
                return returnTree;
            }
            
        }

        public bool Remove(TKey k)
        {
            bool removed;
            _elements = Remove(k, _elements, out removed);
            return removed;
        }
    }
}
