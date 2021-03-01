using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

public class Tree : MonoBehaviour
{
    public class NtreeNode<T, X>
    {
        public T board { get; set; }
        public X validSpot { get; set; }
        public int n { get; set; } // visited times
        public int N { get; set; } // deepth
        public int childNum { get; set; }
        public NtreeNode<T, X> parent { get; set; }
        public List<NtreeNode<T, X>> child = new List<NtreeNode<T, X>>();
    }

    public class Ntree<T, X>
    {
        public NtreeNode<T, X> root;
        //constructor
        public Ntree(T currentBoard, X spot)
        {
            this.root = new NtreeNode<T, X>();
            root.board = currentBoard;
            root.validSpot = spot;
            root.n = 0;
            root.N = 0;
            root.parent = null;
            root.childNum = 0;
        }

        //constructor if you want to use a specific NtreeNode as the root
        public Ntree(NtreeNode<T, X> p)
        {
            this.root = new NtreeNode<T, X>();
            root.board = p.board;
            root.validSpot = p.validSpot;
            root.n = 0;
            root.N = 0;
            root.parent = null;
            root.childNum = 0;
        }

        public void insertNtreeNode(NtreeNode<T, X> p, T currentBoard, X spot)
        {
            NtreeNode<T, X> temp = new NtreeNode<T, X>();
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
