using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameAITest
{
    public class treeNode<T, X>
    {
        public T board { get; set; }
        public X validSpot { get; set; }
        public int n { get; set; } // visited times
        public int N { get; set; } // deepth
        public int childNum { get; set; }
        public treeNode<T, X> parent { get; set; }
        public List<treeNode<T, X>> child = new List<treeNode<T, X>>();
    }

    public class Tree<T, X>
    {
        public treeNode<T, X> root;
        //constructor
        public Tree(T currentBoard, X spot)
        {
            this.root = new treeNode<T, X>();
            root.board = currentBoard;
            root.validSpot = spot;
            root.n = 0;
            root.N = 0;
            root.parent = null;
            root.childNum = 0;
        }

        //constructor if you want to use a specific treeNode as the root
        public Tree(treeNode<T, X> p)
        {
            this.root = new treeNode<T, X>();
            root.board = p.board;
            root.validSpot = p.validSpot;
            root.n = 0;
            root.N = 0;
            root.parent = null;
            root.childNum = 0;
        }

        public void inserttreeNode(treeNode<T, X> p, T currentBoard, X spot)
        {
            treeNode<T, X> temp = new treeNode<T, X>();
            temp.board = currentBoard;
            temp.validSpot = spot;
            temp.n = 0;
            root.N += p.N;
            temp.parent = p;
            p.child.Add(temp);
            p.childNum++;
        }



    }
}
