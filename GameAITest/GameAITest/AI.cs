using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameAITest
{
    static class BoardRelationship
    {
        public static List<int> BranchToNode(BranchState branch) // given branch id(0,1,...), export Node id list
        {
            List<int> result = new List<int>();
            if (branch.location == 0)
            {
                result.Add(0);
                result.Add(1);
            }
            else if (branch.location >= 3 && branch.location <= 5)
            {
                result.Add(branch.location - 1);
                result.Add(branch.location);
            }
            else if (branch.location >= 10 && branch.location <= 14)
            {
                result.Add(branch.location - 4);
                result.Add(branch.location - 3);
            }
            else if (branch.location >= 21 && branch.location <= 25)
            {
                result.Add(branch.location - 9);
                result.Add(branch.location - 8);
            }
            else if (branch.location >= 30 && branch.location <= 32)
            {
                result.Add(branch.location - 12);
                result.Add(branch.location - 11);
            }
            else if (branch.location == 35)
            {
                result.Add(22);
                result.Add(23);
            }
            else if (branch.location >= 1 && branch.location <= 2)
            {
                result.Add(branch.location - 1);
                result.Add(branch.location + 2);
            }
            else if (branch.location >= 6 && branch.location <= 9)
            {
                result.Add(branch.location - 4);
                result.Add(branch.location + 1);
            }
            else if (branch.location >= 15 && branch.location <= 20)
            {
                result.Add(branch.location - 9);
                result.Add(branch.location - 3);
            }
            else if (branch.location >= 26 && branch.location <= 29)
            {
                result.Add(branch.location - 13);
                result.Add(branch.location - 8);
            }
            else if (branch.location >= 33 && branch.location <= 34)
            {
                result.Add(branch.location - 14);
                result.Add(branch.location - 11);
            }
            return result;
        }
        public static List<int> NodeToBranch(NodeState node) // given node id(0,1,...), export Branch id list
        {
            List<int> result = new List<int>();
            if (node.location == 0 || node.location == 1)
            {
                result.Add(0);
                result.Add(node.location + 1);
            }
            else if (node.location >= 2 && node.location <= 5)
            {
                if (node.location > 2)
                {
                    result.Add(node.location);
                }
                if (node.location - 2 >= 1 && node.location - 2 <= 2)
                {
                    result.Add(node.location - 2);
                }
                if (node.location + 1 >= 3 && node.location + 1 <= 5)
                {
                    result.Add(node.location + 1);
                }
                result.Add(node.location + 4);
            }
            else if (node.location >= 6 && node.location <= 11)
            {
                if (node.location + 3 >= 10)
                {
                    result.Add(node.location + 3);
                }
                if (node.location - 1 >= 6 && node.location - 1 <= 9)
                {
                    result.Add(node.location - 1);
                }
                if (node.location + 4 >= 10 && node.location + 1 <= 14)
                {
                    result.Add(node.location + 4);
                }
                result.Add(node.location + 9);
            }
            else if (node.location >= 12 && node.location <= 17)
            {
                if (node.location + 8 >= 21)
                {
                    result.Add(node.location + 8);
                }
                result.Add(node.location + 3);
                if (node.location + 9 >= 21 && node.location + 9 <= 25)
                {
                    result.Add(node.location + 9);
                }
                if (node.location + 13 >= 26 && node.location + 13 <= 29)
                {
                    result.Add(node.location + 13);
                }
            }
            else if (node.location >= 18 && node.location <= 21)
            {
                if (node.location + 11 >= 30)
                {
                    result.Add(node.location + 11);
                }
                result.Add(node.location + 8);
                if (node.location + 12 >= 30 && node.location + 12 <= 32)
                {
                    result.Add(node.location + 12);
                }
                if (node.location + 14 >= 33 && node.location + 14 <= 34)
                {
                    result.Add(node.location + 14);
                }
            }
            else if(node.location == 22 || node.location == 23)
            {
                result.Add(node.location + 11);
                result.Add(35);
            }
            return result;
        }
        public static List<int> SquareToBranch(SquareState square) // given square id(0,1,...), export Branch id list(4 directly connected branches)
        {
            List<int> result = new List<int>();
            if(square.location == 0)
            {
                result.Add(0);
                result.Add(1);
                result.Add(2);
                result.Add(4);
            }
            else if(square.location >= 1 && square.location <= 3)
            {
                result.Add(square.location + 5);
                result.Add(square.location + 2);
                result.Add(square.location + 6);
                result.Add(square.location + 10);
            }
            else if (square.location >= 4 && square.location <= 8)
            {
                result.Add(square.location + 11);
                result.Add(square.location + 6);
                result.Add(square.location + 12);
                result.Add(square.location + 17);
            }
            else if (square.location >= 9 && square.location <= 11)
            {
                result.Add(square.location + 17);
                result.Add(square.location + 13);
                result.Add(square.location + 18);
                result.Add(square.location + 21);
            }
            if (square.location == 12)
            {
                result.Add(33);
                result.Add(31);
                result.Add(34);
                result.Add(35);
            }
            return result;
        }

        public static List<int> BranchToBranch(BranchState branch) // given Branch id(0,1,...), export connected Branch id list
        {
            List<int> result = new List<int>();
            if(branch.location == 0)
            {
                result.Add(1);
                result.Add(2);
            }
            else if(branch.location>= 3 && branch.location <= 5)
            {
                if(branch.location - 1 >= 3)
                {
                    result.Add(branch.location - 1);
                }
                if (branch.location + 1 <= 5)
                {
                    result.Add(branch.location + 1);
                }
                if (branch.location -3 >= 1)
                {
                    result.Add(branch.location - 3);
                }
                if(branch.location - 2 <= 2)
                {
                    result.Add(branch.location - 2);
                }
                result.Add(branch.location + 3);
                result.Add(branch.location + 4);
            }
            else if (branch.location >= 10 && branch.location <= 14)
            {
                if (branch.location - 5 >= 6)
                {
                    result.Add(branch.location - 5);
                }
                if (branch.location - 4 <= 9)
                {
                    result.Add(branch.location - 4);
                }
                if (branch.location - 1 >= 10)
                {
                    result.Add(branch.location - 1);
                }
                if (branch.location + 1 <= 14)
                {
                    result.Add(branch.location + 1);
                }
                result.Add(branch.location + 5);
                result.Add(branch.location + 6);
            }
            else if (branch.location >= 21 && branch.location <= 25)
            {
                if (branch.location - 1 >= 21)
                {
                    result.Add(branch.location - 1);
                }
                if (branch.location + 1 <= 25)
                {
                    result.Add(branch.location + 1);
                }
                result.Add(branch.location - 6);
                result.Add(branch.location - 5);
                if (branch.location + 4 >= 26)
                {
                    result.Add(branch.location + 4);
                }
                if (branch.location + 5 <= 29)
                {
                    result.Add(branch.location + 5);
                }
            }
            else if (branch.location >= 30 && branch.location <= 32)
            {
                if (branch.location - 1 >= 30)
                {
                    result.Add(branch.location - 1);
                }
                if (branch.location + 1 <= 32)
                {
                    result.Add(branch.location + 1);
                }
                result.Add(branch.location - 4);
                result.Add(branch.location - 3);
                if (branch.location + 2 >= 33)
                {
                    result.Add(branch.location + 2);
                }
                if (branch.location + 3 <= 34)
                {
                    result.Add(branch.location + 3);
                }
            }
            else if(branch.location == 35)
            {
                result.Add(33);
                result.Add(34);
            }
            else if (branch.location == 1 || branch.location == 2)
            {
                result.Add(branch.location + 2);
                result.Add(branch.location + 3);
                result.Add(branch.location + 6);
            }
            else if (branch.location >= 6 && branch.location <= 9)
            {
                result.Add(branch.location + 4);
                result.Add(branch.location + 5);
                result.Add(branch.location + 10);
                if (branch.location - 4 >= 3)
                {
                    result.Add(branch.location - 4);
                }
                if (branch.location - 3 <= 5)
                {
                    result.Add(branch.location - 3);
                }
                if (branch.location - 6 >= 1 && branch.location - 6 <= 2)
                {
                    result.Add(branch.location - 6);
                }
            }
            else if (branch.location >= 15 && branch.location <= 20)
            {
                if (branch.location - 6 >= 10)
                {
                    result.Add(branch.location - 6);
                }
                if (branch.location - 5 <= 14)
                {
                    result.Add(branch.location - 5);
                }
                if (branch.location - 10 >= 6 && branch.location - 10 <= 9)
                {
                    result.Add(branch.location - 10);
                }

                if (branch.location + 5 >= 21)
                {
                    result.Add(branch.location + 5);
                }
                if (branch.location + 6 <= 25)
                {
                    result.Add(branch.location + 6);
                }
                if (branch.location + 10 >= 26 && branch.location + 10 <= 29)
                {
                    result.Add(branch.location + 10);
                }
            }
            else if (branch.location >= 26 && branch.location <= 29)
            {
                result.Add(branch.location - 4);
                result.Add(branch.location - 5);
                result.Add(branch.location - 10);
                if (branch.location + 4 <= 32)
                {
                    result.Add(branch.location + 4);
                }
                if (branch.location + 3 >= 30)
                {
                    result.Add(branch.location + 3);
                }
                if (branch.location + 6 >= 33 && branch.location + 6 <= 34)
                {
                    result.Add(branch.location + 6);
                }
            }
            else if (branch.location == 33 || branch.location == 34)
            {
                result.Add(31);
                result.Add(35);
                if(branch.location == 33)
                {
                    result.Add(30);
                    result.Add(27);
                }
                else
                {
                    result.Add(28);
                    result.Add(32);
                }
            }
            return result;
        }
        public static List<double> Evaluation(BoardState board)
        {
            List<double> result = new List<double>();

            return result;
        }
    }

    class AIMove
    {
        private PlayerColor AIcolor;
        private BoardState my_board;
        private List<List<int>> result_SquareToBranch;
        private int flagOpeningPhase = 0;
        public AIMove(BoardState board)
        {
            my_board = new BoardState();
            my_board = board;
        }

        private void CheckExhaustedSquare(NodeState node)
        {
            for(int i = 0; i < 12; i++)
            {
                int num = 0;
                foreach(int j in ReferenceScript.tileConnectsToTheseNodes[i])
                {
                    if(node.location == j) // found a connected tile
                    {
                        foreach (int v in ReferenceScript.tileConnectsToTheseNodes[i])
                        {
                            if(my_board.nodeStates[v].nodeColor != PlayerColor.Blank)
                            {
                                num++;
                            }
                        }
                        if(num != 0)
                        {
                            if((int)my_board.squareStates[i].resourceAmount < num && my_board.squareStates[i].resourceState != SquareStatus.Captured)
                            {
                                my_board.squareStates[i].resourceState = SquareStatus.Blocked;
                            }
                            num = 0;
                        }
                    }
                }

            }
        }

        public BoardState MakeRandomMove()
        {
            int temp = 0;
            BoardState result = new BoardState();
            if(flagOpeningPhase != 1)
            {
                foreach (NodeState i in my_board.nodeStates)
                {
                    if (i.nodeColor != PlayerColor.Blank)
                    {
                        temp++;
                    }
                }
                if (temp > 3)
                {
                    flagOpeningPhase = 1;
                    result = RandomMove();
                }
                else
                {
                    if (temp == 0 || temp == 3)
                    {
                        AIcolor = PlayerColor.Orange;
                    }
                    else
                    {
                        AIcolor = PlayerColor.Purple;
                        result = OpeningMoves();
                    }
                    result = OpeningMoves();
                }
            }
            else
            {
                    result = RandomMove();
            }
          
            return result;
        }

        private BoardState OpeningMoves()
        {
            int loc = 0;
            double max = -1;
            BoardState result = new BoardState();
            result = my_board;
            foreach(SquareState i in result.squareStates)
            {
                result_SquareToBranch.Add(BoardRelationship.SquareToBranch(i));
            }
            List<double> tempTable = new List<double>( BoardRelationship.Evaluation(result));
            for(int i = 0; i < tempTable.Count; i++)
            {
                if(tempTable[i] > max)
                {
                    loc = i;
                    max = tempTable[i];
                }
            }
            result.nodeStates[loc].nodeColor = AIcolor;
            my_board = result;
            return result;
        }
        private BoardState RandomMove()
        {
            BoardState result = new BoardState();

            return result;
        }
    }
}

















/*
class main
{
   static void Main()
    {
        Tree<int, int> tree = new Tree<int, int>(1, 2);
        tree.inserttreeNode(tree.root, 10, 20);
        Tree<int, int> otherTree = new Tree<int, int>(tree.root.child[0]);

    }

}
*/
