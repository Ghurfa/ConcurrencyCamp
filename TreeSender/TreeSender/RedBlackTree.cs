using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TreeSender
{
    class RedBlackNode
    {
        public RedBlackNode LeftChild;
        public RedBlackNode RightChild;
        public bool IsRed;
        public int Value;

        public RedBlackNode(int value, bool isRed)
        {
            Value = value;
            IsRed = isRed;
        }
        public RedBlackNode(int value, bool isRed, RedBlackNode leftChild, RedBlackNode rightChild)
        {
            Value = value;
            LeftChild = leftChild;
            RightChild = rightChild;
            IsRed = isRed;
        }
        public void FlipColor()
        {
            IsRed = !IsRed;
            if (LeftChild != null) LeftChild.IsRed = !LeftChild.IsRed;
            if (RightChild != null) RightChild.IsRed = !RightChild.IsRed;
        }
        public RedBlackNode RotateLeft()
        {
            if (RightChild == null) return this;
            RightChild.IsRed = IsRed;
            IsRed = true;

            RedBlackNode newParent = RightChild;
            RightChild = RightChild.LeftChild;
            newParent.LeftChild = this;

            return newParent;
        }
        public RedBlackNode RotateRight()
        {
            if (LeftChild == null) return this;
            LeftChild.IsRed = IsRed;
            IsRed = true;

            RedBlackNode newParent = LeftChild;
            LeftChild = LeftChild.RightChild;
            newParent.RightChild = this;

            return newParent;
        }
    }
    class RedBlackTree
    {
        private RedBlackNode rootNode;
        public RedBlackTree()
        {
            rootNode = null;
        }
        public RedBlackTree(RedBlackNode rootNode)
        {
            this.rootNode = rootNode;
        }
        public bool IsEmpty => rootNode == null;
        public int Count { get; private set; }
        public void Insert(int value)
        {
            if (IsEmpty)
            {
                rootNode = new RedBlackNode(value, false);
                Count++;
                return;
            }
            RedBlackNode Insert(RedBlackNode node, int valueToInsert)
            {
                bool isLeftChildRed() => node.LeftChild == null ? false : node.LeftChild.IsRed;
                bool isRightChildRed() => node.RightChild == null ? false : node.RightChild.IsRed;
                if (isLeftChildRed() && isRightChildRed()) node.FlipColor();
                if (valueToInsert < node.Value)
                {
                    if (node.LeftChild == null) node.LeftChild = new RedBlackNode(valueToInsert, true);
                    else node.LeftChild = Insert(node.LeftChild, valueToInsert);
                }
                else
                {
                    if (node.RightChild == null) node.RightChild = new RedBlackNode(valueToInsert, true);
                    else node.RightChild = Insert(node.RightChild, valueToInsert);
                }
                if (isRightChildRed()) node = node.RotateLeft();
                if (isLeftChildRed())
                {
                    bool isLeftLeftChildRed = node.LeftChild.LeftChild == null ? false : node.LeftChild.LeftChild.IsRed;
                    if (isLeftLeftChildRed) node = node.RotateRight();
                }
                return node;
            }
            rootNode = Insert(rootNode, value);
            Count++;
            rootNode.IsRed = false;
        }
        public bool Delete(int value)
        {
            if (IsEmpty) return false;
            bool deleted = false;
            RedBlackNode MoveRedLeft(RedBlackNode node)
            {
                node.FlipColor();
                if (node.RightChild != null) node.RightChild = node.RightChild.RotateRight();
                node = node.RotateLeft();
                node.FlipColor();
                if (node.RightChild != null)
                {
                    if (isRed(node.RightChild.RightChild)) node.RightChild = node.RightChild.RotateLeft();
                }
                return node;
            }
            RedBlackNode MoveRedRight(RedBlackNode node)
            {
                node.FlipColor();
                if (node.LeftChild != null)
                {
                    if (isRed(node.LeftChild.LeftChild))
                    {
                        node = node.RotateRight();
                        node.FlipColor();
                    }
                }
                return node;
            }
            RedBlackNode Delete(RedBlackNode node, int valueToDelete)
            {
                if (valueToDelete < node.Value)
                {
                    if (node.LeftChild != null)
                    {
                        if (!isRed(node.LeftChild) && !isRed(node.LeftChild.LeftChild))
                        {
                            if (!node.IsRed && !isRed(node.RightChild))
                            {
                                node.LeftChild.IsRed = true;
                                if (node.RightChild != null) node.RightChild.IsRed = true;
                            }
                            else node = MoveRedLeft(node);
                        }
                        node.LeftChild = Delete(node.LeftChild, valueToDelete);
                    }
                }
                else
                {
                    if (isRed(node.LeftChild)) node = node.RotateRight();
                    if (node.Value == valueToDelete && node.LeftChild == null && node.RightChild == null)
                    {
                        deleted = true;
                        return null;
                    }
                    if (isTwoNode(node.RightChild)) node = MoveRedRight(node);
                    if (node.Value == valueToDelete)
                    {
                        deleted = true;
                        if (node.RightChild != null)
                        {
                            RedBlackTree rightSubTree = new RedBlackTree(node.RightChild);
                            node.Value = rightSubTree.Minimum().Value;
                            node.RightChild = Delete(node.RightChild, node.Value);
                        }
                        else
                        {
                            node.Value = node.LeftChild.Value;
                            node.RightChild = node.LeftChild.RightChild;
                            node.LeftChild = node.LeftChild.LeftChild;
                        }
                    }
                    else if (node.RightChild != null)
                    {
                        node.RightChild = Delete(node.RightChild, valueToDelete);
                    }
                }
                if (isRed(node.RightChild)) node = node.RotateLeft();
                if (isRed(node.LeftChild))
                {
                    if (isRed(node.LeftChild.LeftChild)) node = node.RotateRight();
                }
                if (isRed(node.LeftChild) && isRed(node.RightChild)) node.FlipColor();
                if (node.LeftChild != null)
                {
                    if (isRed(node.LeftChild.RightChild)) node.LeftChild = node.LeftChild.RotateLeft();
                    if (isRed(node.LeftChild.LeftChild))
                    {
                        if (isRed(node.LeftChild.LeftChild.LeftChild)) node.LeftChild = node.LeftChild.RotateRight();
                    }
                }
                return node;
            }
            rootNode = Delete(rootNode, value);
            Count--;
            if (isRed(rootNode)) rootNode.IsRed = false;
            return deleted;
        }
        public RedBlackNode Minimum()
        {
            if (IsEmpty) return null;
            RedBlackNode node = rootNode;
            while (node.LeftChild != null)
            {
                node = node.LeftChild;
            }
            return node;
        }
        public RedBlackNode Maximum()
        {
            if (IsEmpty) return null;
            RedBlackNode node = rootNode;
            while (node.RightChild != null)
            {
                node = node.RightChild;
            }
            return node;
        }
        public RedBlackNode Search(int value)
        {
            RedBlackNode currentNode = rootNode;
            while (currentNode != null)
            {
                if (value < currentNode.Value) currentNode = currentNode.LeftChild;
                else if (value > currentNode.Value) currentNode = currentNode.RightChild;
                else break;
            }
            return currentNode;
        }
        public IEnumerable LevelOrder()
        {
            if (!IsEmpty)
            {
                Queue<RedBlackNode> values = new Queue<RedBlackNode>();
                values.Enqueue(rootNode);

                while (values.Count > 0)
                {
                    RedBlackNode value = values.Dequeue();
                    if (value.LeftChild != null) values.Enqueue(value.LeftChild);
                    if (value.RightChild != null) values.Enqueue(value.RightChild);
                    yield return value;
                }
            }
        }
        public IEnumerable InOrder()
        {
            RedBlackNode[] inOrder(RedBlackNode node)
            {
                if (node == null) return null;
                RedBlackNode[] combineArrays(RedBlackNode[] firstArray, RedBlackNode[] secondArray)
                {
                    if (firstArray == null) return secondArray;
                    if (secondArray == null) return firstArray;
                    RedBlackNode[] newArray = new RedBlackNode[firstArray.Length + secondArray.Length];
                    for (int i = 0; i < firstArray.Length; i++)
                    {
                        newArray[i] = firstArray[i];
                    }
                    for (int i = 0; i < secondArray.Length; i++)
                    {
                        newArray[i + firstArray.Length] = secondArray[i];
                    }
                    return newArray;
                }
                return combineArrays(combineArrays(inOrder(node.LeftChild), new RedBlackNode[] { node }), inOrder(node.RightChild));
            }
            RedBlackNode[] nodes = inOrder(rootNode);
            for (int i = 0; i < nodes.Length; i++)
            {
                yield return nodes[i];
            }
        }
        public IEnumerable PreOrder()
        {
            Stack<RedBlackNode> nodeStack = new Stack<RedBlackNode>();
            if (!IsEmpty)
            {
                nodeStack.Push(rootNode);
                while (nodeStack.Count > 0)
                {
                    RedBlackNode leftNode = nodeStack.Peek().LeftChild;
                    RedBlackNode rightNode = nodeStack.Peek().RightChild;

                    yield return nodeStack.Pop();
                    if (rightNode != null) nodeStack.Push(rightNode);
                    if (leftNode != null) nodeStack.Push(leftNode);
                }
            }
        }
        public IEnumerable PostOrder()
        {
            Stack<RedBlackNode> nodeStack = new Stack<RedBlackNode>();
            if (!IsEmpty)
            {
                nodeStack.Push(rootNode);
                bool delete = false;
                while (nodeStack.Count > 0)
                {
                    if (nodeStack.Peek().LeftChild != null && !delete)
                    {
                        nodeStack.Push(nodeStack.Peek().LeftChild);
                    }
                    else if (nodeStack.Peek().RightChild != null && !delete)
                    {
                        nodeStack.Push(nodeStack.Peek().RightChild);
                    }
                    else
                    {
                        delete = true;
                        RedBlackNode node = nodeStack.Pop();
                        yield return node;
                        if (nodeStack.Count == 0) break;
                        if (node == nodeStack.Peek().LeftChild && nodeStack.Peek().RightChild != null)
                        {
                            nodeStack.Push(nodeStack.Peek().RightChild);
                            delete = false;
                        }
                    }
                }
            }
        }
        private bool isRed(RedBlackNode node)
        {
            if (node == null) return false;
            return node.IsRed;
        }
        private bool isTwoNode(RedBlackNode node)
        {
            if (node == null) return false;
            return !isRed(node.LeftChild) && !isRed(node.RightChild) && !node.IsRed;
        }
    }
}
