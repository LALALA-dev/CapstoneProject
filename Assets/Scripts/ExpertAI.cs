using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using static GameObjectProperties;
using static ReferenceScript;
using static TreeNode;
using System.Threading;
using UnityEngine;
using System.Threading.Tasks;

public class ExpertAI
{
    //after instantiate this class, call findNextMove with time limit(5 for exopert AI) 
    public class AI
    {
        private int t; //total number of simulations
        private PlayerColor AIcolor;
        private MyBoard beginBoard;
        private int[] aiResourcesForUpdateBoard;
        public AI(PlayerColor aiColor, BoardState openingBoardState, int[] aiResources, int[] playerResources)
        {
            AIcolor = aiColor;
            beginBoard.boardState = CopyBoard(openingBoardState);
            beginBoard.aiResources = (int[])aiResources.Clone();
            beginBoard.playerResources = (int[])playerResources.Clone();
            aiResourcesForUpdateBoard = aiResources;
        }

        private PlayerColor getcurrentPlayerColor(TreeNode node)
        {
            PlayerColor currentPlayer;
            if (node.level % 2 != 0)
            {
                currentPlayer = AIcolor;
            }
            else if (AIcolor == PlayerColor.Silver)
            {
                currentPlayer = PlayerColor.Gold;
            }
            else
            {
                currentPlayer = PlayerColor.Silver;
            }
            return currentPlayer;
        }

        private List<MyBoard> GetPossibleMoves(MyBoard currentBoard, PlayerColor currentPlayer)
        {
            int flag_moved = 0;
            int trade = 0;
            List<List<MyBoard>> storage = new List<List<MyBoard>>();
            storage.Add(new List<MyBoard>());
            MyBoard tem = currentBoard.Clone();
            storage[0].Add(tem);
            PlayerColor otherColor = new PlayerColor();
            do
            {

                flag_moved = 0;
                List<MyBoard> temp_result = new List<MyBoard>();
                for (int i = 0; i < storage[storage.Count - 1].Count; i++)
                {
                    temp_result.Clear();
                    MyBoard temp = storage[storage.Count - 1][i].Clone();
                    int[] resource;
                    if (currentPlayer == AIcolor)
                    {
                        resource = temp.aiResources;
                        if (AIcolor == PlayerColor.Silver)
                        {
                            otherColor = PlayerColor.Gold;
                        }
                        else
                        {
                            otherColor = PlayerColor.Silver;
                        }
                    }
                    else
                    {
                        otherColor = AIcolor;
                        resource = temp.playerResources;
                    }


                    //part1:branches
                    int[] temp_resource_branches = (int[])resource.Clone();
                    List<int> branches = CalculatePossibleBranches(temp.boardState, temp_resource_branches, currentPlayer);
                    if (branches.Count == 0 && trade == 0)
                    {
                        ResourceTradingForBranches(temp_resource_branches, BeginnerAI.CollectCurrentPlayerResources(currentBoard.boardState, currentPlayer), temp.boardState, currentPlayer, ref trade);
                        branches = CalculatePossibleBranches(temp.boardState, temp_resource_branches, currentPlayer);
                    }
                    foreach (int j in branches)
                    {
                        MyBoard tempp = temp.Clone();
                        tempp.boardState.branchStates[j].ownerColor = currentPlayer;
                        tempp.boardState.branchStates[j].branchColor = currentPlayer;
                        if (currentPlayer == AIcolor)
                        {
                            tempp.aiResources = temp_resource_branches;
                        }
                        else
                        {
                            tempp.playerResources = temp_resource_branches;
                        }
                        temp_result.Add(tempp);
                        flag_moved = 1;
                    }

                    //part2:nodes
                    int[] temp_resource_nodes = (int[])resource.Clone();
                    List<int> nodes = CalculatePossibleNodes(temp.boardState, temp_resource_nodes, currentPlayer);
                    if (nodes.Count == 0 && trade == 0)
                    {
                        ResourceTradingForNodes(temp_resource_nodes, BeginnerAI.CollectCurrentPlayerResources(currentBoard.boardState, currentPlayer), temp.boardState, currentPlayer, ref trade);
                        nodes = CalculatePossibleNodes(temp.boardState, temp_resource_nodes, currentPlayer);
                    }
                    foreach (int j in nodes)
                    {
                        MyBoard tempp = temp.Clone();
                        tempp.boardState.nodeStates[j].nodeColor = currentPlayer;
                        if (currentPlayer == AIcolor)
                        {
                            tempp.aiResources = temp_resource_nodes;
                        }
                        else
                        {
                            tempp.playerResources = temp_resource_nodes;
                        }
                        temp_result.Add(tempp);
                        flag_moved = 1;
                    }
                }
                storage.Add(temp_result);

            } while (flag_moved == 1);

            foreach (MyBoard i in storage[storage.Count - 2])
            {
                for (int j = 0; j < 24; j++)
                {
                    BeginnerAI.DetectLocalTileOverloads(i.boardState, j);
                }
                DetectMultiTileCaptures(i.boardState);
                int[] temp = BeginnerAI.CollectCurrentPlayerResources(i.boardState, otherColor);
                if (otherColor == AIcolor)
                {
                    for (int k = 0; k < 4; k++)
                    {
                        i.aiResources[k] += temp[k];
                    }
                }
                else
                {
                    for (int k = 0; k < 4; k++)
                    {
                        i.playerResources[k] += temp[k];
                    }
                }

            }
            for (int i = 0; i < storage.Count; i++)
            {
                if (storage[storage.Count - (1 + i)].Count != 0)
                {/*
                    for (int k = 0; k < storage[storage.Count - (1 + i)].Count - 1; k++)
                    {
                        int cc = 0;
                        int nn = 0;
                        for (int j = 0; j < 36; j++)
                        {
                            if (storage[storage.Count - (1 + i)][k].boardState.branchStates[j].branchColor == storage[storage.Count - (1 + i)][k + 1].boardState.branchStates[j].branchColor)
                            {
                                cc++;
                            }
                        }
                        for (int j = 0; j < 24; j++)
                        {
                            if (storage[storage.Count - (1 + i)][k].boardState.nodeStates[j].nodeColor == storage[storage.Count - (1 + i)][k + 1].boardState.nodeStates[j].nodeColor)
                            {
                                nn++;
                            }
                        }
                        if (nn == 24 && cc == 36)
                        {
                            storage[storage.Count - (1 + i)].RemoveAt(k);
                        }
                    }
                    */
                    return storage[storage.Count - (1 + i)];
                }
            }
            return storage[0];
        }

        private TreeNode traverse(TreeNode root)
        {
            TreeNode currentNode = root;
            // Debug.Log("child: " + currentNode.child.Count);
            while (currentNode.child.Count != 0)
            {
                currentNode = UCT.findBestNode(currentNode, root);
            }
            return currentNode;
        }

        private void expand(TreeNode node)
        {
            PlayerColor currentPlayer = getcurrentPlayerColor(node);
            if (currentPlayer == PlayerColor.Silver)
            {
                currentPlayer = PlayerColor.Gold;
            }
            else
            {
                currentPlayer = PlayerColor.Silver;
            }
            List<MyBoard> temp = GetPossibleMoves(node.localBoard, currentPlayer);
            foreach (MyBoard i in temp)
            {
                node.AddChild(i);
            }
        }

        private void backpropgation(TreeNode node, int winner)
        {
            TreeNode temp = node;
            while (temp != null)
            {
              
                temp.W += winner;
                temp.N++;
                temp = temp.parent;
            }
        }

        private int simulation3(TreeNode node)
        {
            int winner = 0;
            TreeNode temp = node.Copy();
            PlayerColor playerCol = getcurrentPlayerColor(node);
            PlayerColor otherColor = new PlayerColor();
            int copunt = 0;
            System.Random rnd = new System.Random();
            while (winner == 0 && copunt < 100)
            {
                if (playerCol == PlayerColor.Silver)
                {
                    otherColor = PlayerColor.Gold;
                }
                else
                {
                    otherColor = PlayerColor.Silver;
                }

                int moved = 1;
                int trad = 0;
                int[] resource_temp_curr = new int[4];
                int[] resource_temp_othr = new int[4];
                do
                {
                    if (playerCol == AIcolor)
                    {
                        resource_temp_curr = temp.localBoard.aiResources;
                        resource_temp_othr = temp.localBoard.playerResources;
                    }
                    else
                    {
                        resource_temp_curr = temp.localBoard.playerResources;
                        resource_temp_othr = temp.localBoard.aiResources;
                    }
                    if (rnd.Next(0, 2) == 0 && moved == 1)
                    {//branches
                        moved = 0;
                        List<int> result = CalculatePossibleBranches(temp.localBoard.boardState, resource_temp_curr, playerCol);
                        if (result.Count == 0)
                        {
                            ResourceTradingForBranches(resource_temp_curr, BeginnerAI.CollectCurrentPlayerResources(temp.localBoard.boardState, playerCol), temp.localBoard.boardState, playerCol,ref trad);
                            result = CalculatePossibleBranches(temp.localBoard.boardState, resource_temp_curr, playerCol);
                            if (result.Count == 0)
                            {
                                result = CalculatePossibleNodes(temp.localBoard.boardState, resource_temp_curr, playerCol);
                                if(result.Count == 0)
                                {
                                    ResourceTradingForNodes(resource_temp_curr, BeginnerAI.CollectCurrentPlayerResources(temp.localBoard.boardState, playerCol), temp.localBoard.boardState, playerCol, ref trad);
                                    result = CalculatePossibleNodes(temp.localBoard.boardState, resource_temp_curr, playerCol);
                                }
                                if(result.Count != 0)
                                {
                                    int index = rnd.Next(0, result.Count);
                                    temp.localBoard.boardState.nodeStates[result[index]].nodeColor = playerCol;
                                    moved = 1;
                                }
                               
                            }
                        }
                        if(moved == 0 && result.Count != 0)
                        {
                            int index = rnd.Next(0, result.Count);
                            temp.localBoard.boardState.branchStates[result[index]].ownerColor = playerCol;
                            temp.localBoard.boardState.branchStates[result[index]].branchColor = playerCol;
                            moved = 1;
                        }
                    }
                    else
                    {//nodes
                        moved = 0;
                        List<int> result = CalculatePossibleNodes(temp.localBoard.boardState, resource_temp_curr, playerCol);
                        if (result.Count == 0)
                        {
                            ResourceTradingForNodes(resource_temp_curr, BeginnerAI.CollectCurrentPlayerResources(temp.localBoard.boardState, playerCol), temp.localBoard.boardState, playerCol, ref trad);
                            result = CalculatePossibleNodes(temp.localBoard.boardState, resource_temp_curr, playerCol);
                            if (result.Count == 0)
                            {
                                result = CalculatePossibleBranches(temp.localBoard.boardState, resource_temp_curr, playerCol);
                                if(result.Count == 0)
                                {
                                    ResourceTradingForBranches(resource_temp_curr, BeginnerAI.CollectCurrentPlayerResources(temp.localBoard.boardState, playerCol), temp.localBoard.boardState, playerCol, ref trad);
                                    result = CalculatePossibleBranches(temp.localBoard.boardState, resource_temp_curr, playerCol);
                                }
                                if(result.Count != 0)
                                {
                                    int index = rnd.Next(0, result.Count);
                                    temp.localBoard.boardState.branchStates[result[index]].ownerColor = playerCol;
                                    temp.localBoard.boardState.branchStates[result[index]].branchColor = playerCol;
                                    moved = 1;

                                }
                            }
                        }
                        if (moved == 0 && result.Count != 0)
                        {
                            int index = rnd.Next(0, result.Count);
                            temp.localBoard.boardState.nodeStates[result[index]].nodeColor = playerCol;
                            moved = 1;
                        }
                    }
                } while (moved == 1);

                DetectMultiTileCaptures(temp.localBoard.boardState);
                int[] res = BeginnerAI.CollectCurrentPlayerResources(temp.localBoard.boardState, otherColor);
                for (int i = 0; i < 4; i++)
                {
                    resource_temp_othr[i] += res[i];
                }


                winner = isEnd(temp.localBoard.boardState, AIcolor);

                if (playerCol == PlayerColor.Silver)
                {
                    playerCol = PlayerColor.Gold;
                }
                else
                {
                    playerCol = PlayerColor.Silver;
                }
                copunt++;
            }
            return winner;
        }

        public async Task<BoardState> findNextMove(double timeLimit) // timeLimit = 5 means 5 seconds
        {
            int ttt = new int();
            int ccc = 0;
            BoardState best = new BoardState();
            DateTime beforDT = System.DateTime.Now;
            TimeSpan t = TimeSpan.FromSeconds(timeLimit);
            TreeNode root = new TreeNode(beginBoard);
            expand(root);
            bool timeOut = false;

            await Task.Run(() =>
            {
                while (timeOut == false)
                {
                    ccc = ccc;

                    int max = -1;
                    int loc = 0;
                    TreeNode promisingNode = traverse(root);

                    if (promisingNode.N == 0)
                    {
                        int winner = simulation3(promisingNode);
                        backpropgation(promisingNode, winner);
                    }
                    else
                    {
                        expand(promisingNode);
                        if (promisingNode.child.Count == 0)
                        {
                            Debug.Log("Error: promisingNode.child.Count = 0");
                        }
                        promisingNode = promisingNode.child[0];

                   
                        int winner = simulation3(promisingNode);
                 
                            backpropgation(promisingNode, winner);
                   
                    }
                    for (int i = 0; i < root.child.Count; i++)
                    {
                        if (root.child[i].N >= max)
                        {
                            if((root.child[i].N == max && root.child[i].W > root.child[loc].W) || root.child[i].N > max)
                            {
                                loc = i;
                                max = root.child[i].N;
                            }
                        }
                    }
                    best = root.child[loc].localBoard.boardState;
                    ttt = loc;

                    DateTime afterDT = System.DateTime.Now;
                    TimeSpan ts = afterDT.Subtract(beforDT);

                    if (ts >= t)
                    {
                        Debug.Log("times: "+ccc);
                        timeOut = true;

                    }
                    ccc++;
                }
                /*
                for (int i = 0; i < best.branchStates.Count(); i++)
                {
                    if (best.branchStates[i].branchColor != beginBoard.boardState.branchStates[i].branchColor)
                    {
                        Debug.Log("branch placed: " + i);
                    }
                }
                for (int i = 0; i < best.nodeStates.Count(); i++)
                {
                    if (best.nodeStates[i].nodeColor != beginBoard.boardState.nodeStates[i].nodeColor)
                    {
                        Debug.Log("node placed: " + i);
                    }
                }*/

                aiResourcesForUpdateBoard = root.child[ttt].localBoard.aiResources;
                /*
                for(int i = 0; i < root.child.Count-1; i++)
                {
                    int cc = 0;
                    int nn = 0;
                    for (int j = 0; j < best.branchStates.Count(); j++)
                    {
                        if(root.child[i].localBoard.boardState.branchStates[j].branchColor == root.child[i + 1].localBoard.boardState.branchStates[j].branchColor)
                        {
                            cc++;
                        }
                    }
                    for (int j = 0; j < best.nodeStates.Count(); j++)
                    {
                        if (root.child[i].localBoard.boardState.nodeStates[j].nodeColor == root.child[i + 1].localBoard.boardState.nodeStates[j].nodeColor)
                        {
                            nn++;
                        }
                    }
                    if(nn == best.nodeStates.Count() && cc == best.branchStates.Count() )
                    {
                        Debug.Log("root.child[" + i + "] is the same as " + "root.child[" + (i+1) + "]");
                    }
                }*/
                // Assign resources
                if (GameInformation.playerIsHost)
                    GameInformation.playerTwoResources = aiResourcesForUpdateBoard;
                else
                    GameInformation.playerOneResources = aiResourcesForUpdateBoard;
            });
            return best;
        }

        public BoardState MakeRandomOpeningMove(BoardState currentBoard)
        {

            beginBoard.boardState = currentBoard;
            BoardState result = new BoardState();

            if (GameInformation.openingSequence)
            {
                List<NodeState> unownedNodes = new List<NodeState>();
                foreach (NodeState i in currentBoard.nodeStates)
                {
                    if (i.nodeColor == PlayerColor.Blank)
                    {
                        unownedNodes.Add(i);
                    }
                }
                result = OpeningMove(unownedNodes);
            }
            return result;
        }

        private BoardState OpeningMove(List<NodeState> possibleMoves)
        {
            BoardState result = beginBoard.boardState;

            TimeSpan t = (DateTime.UtcNow - new DateTime(1970, 1, 1));
            var rand = new System.Random();
            int index = rand.Next(possibleMoves.Count);

            //*********************
            List<double> temp = evaluateBoardStatus(result);
            double max = -1;
            int loc = -1;
            for (int i = 0; i < temp.Count; i++)
            {
                if (temp[i] >= max)
                {
                    loc = i;
                    max = temp[i];

                }
            }
            //*********************
            result.nodeStates[loc].nodeColor = AIcolor;
            int[] connectedBranche = ReferenceScript.nodeConnectsToTheseBranches[loc];
            int[] connectedBranches = new int[4];
            for (int i = 0, j = 0; i < connectedBranche.Length; i++)
            {

                if (result.branchStates[connectedBranche[i]].ownerColor == PlayerColor.Blank)
                {
                    connectedBranches[j] = connectedBranche[i];
                    j++;
                }
            }
            rand = new System.Random(t.Seconds);
            index = rand.Next(0, connectedBranches.Length);
            result.branchStates[connectedBranches[index]].branchColor = AIcolor;
            result.branchStates[connectedBranches[index]].ownerColor = AIcolor;

            beginBoard.boardState = result;
            return result;
        }

        private List<double> evaluateBoardStatus(BoardState currentBoard)
        {
            List<double> res = new List<double>();
            foreach (NodeState temp in currentBoard.nodeStates)
            {

                double result = 0;
                int[] col = { 0, 0, 0, 0 };

                foreach (int tile in ReferenceScript.nodeConnectToTheseTiles[temp.location])
                {

                    SquareResourceAmount flag = SquareResourceAmount.Blank;

                    foreach (int connectedNode in ReferenceScript.tileConnectsToTheseNodes[tile])
                    {

                        if (currentBoard.nodeStates[connectedNode].nodeColor != PlayerColor.Blank)
                        {
                            flag++;
                        }
                    }
                    if (flag < currentBoard.squareStates[tile].resourceAmount)
                    {

                        if (currentBoard.squareStates[tile].resourceColor == SquareResourceColor.Red)
                        {
                            // Debug.Log("1");
                            col[0]++;
                        }
                        if (currentBoard.squareStates[tile].resourceColor == SquareResourceColor.Yellow)
                        {
                            col[1]++;
                            //   Debug.Log("2");
                        }
                        if (currentBoard.squareStates[tile].resourceColor == SquareResourceColor.Blue)
                        {
                            col[2]++;
                            //    Debug.Log("3");
                        }
                        if (currentBoard.squareStates[tile].resourceColor == SquareResourceColor.Green)
                        {
                            col[3]++;
                            // Debug.Log("4");
                        }
                    }

                    flag = 0;
                    if(currentBoard.squareStates[tile].resourceAmount == SquareResourceAmount.One)
                    {
                        result += 0.2;
                    }else if(currentBoard.squareStates[tile].resourceAmount == SquareResourceAmount.Two)
                    {
                        result += 0.3;
                    }
                    else if (currentBoard.squareStates[tile].resourceAmount == SquareResourceAmount.Three)
                    {
                        result += 0.4;
                    }

                }
                if (col[0] > 0 && col[2] > 0)
                {
                    if (col[0] > 1 && col[2] > 1)
                    {
                        result += (1.8 + 1.8);
                    }
                    else
                    {
                        result += (1.5 + 1.5);
                    }

                    if (col[1] > 0 && col[3] > 0)
                    {
                        result += (2 + 2);
                    }

                }
                else if (col[1] > 0 && col[3] > 0)
                {
                    if (col[1] > 1 && col[3] > 1)
                    {
                        result += (1.8 + 1.8);
                    }
                    else
                    {
                        result += (1.5 + 1.5);
                    }

                }
                for (int i = 0; i < 4; i++)
                {
                    if (col[i] > 0)
                    {
                        result += 1;
                    }
                }

                if (temp.nodeColor == PlayerColor.Blank)
                {
                    res.Add(result);
                }
                else
                {
                    res.Add(-1);
                }

                result = 0;
            }

            return res;
        }



        // assistant function *********************************************** 


        public PlayerColor getCapturedSquareOwner(BoardState currentBoard, int squareId)
        {
            PlayerColor captureColor;
            // Check the surrounding branches for an owner color.
            for (int branch = 0; branch < 4; ++branch)
            {
                captureColor = currentBoard.branchStates[Reference.branchesOnSquareConnections[squareId, branch]].branchColor;
                // If found, return it.
                if (captureColor != PlayerColor.Blank)
                {
                    return captureColor;
                }
            }
            // If all surrounding branches are blank, go to the square above and check it for an owner color. 
            return getCapturedSquareOwner(currentBoard, Reference.squareOnSquareConnections[squareId, 0]);
        }

        public void captureArea(BoardState currentBoard, int startSquare, List<int> possibleCaptures, List<int> captures)
        {
            List<int> blankBranches = getBlankBranches(currentBoard, startSquare);
            possibleCaptures.Remove(startSquare);
            captures.Add(startSquare);

            // For every blank branch on this captured square...
            foreach (int blankBranch in blankBranches)
            {
                // If the connected square hasn't yet been moved from possible to captured then move it.
                if (possibleCaptures.Contains(getConnectedSquare(blankBranch, startSquare)))
                {
                    captureArea(currentBoard, getConnectedSquare(blankBranch, startSquare), possibleCaptures, captures);
                }
            }
        }

        public bool IsValidNodeMoves(BoardState currentBoard, PlayerColor AIcolor)
        {

            List<int> aiOwnedBranches = new List<int>();

            foreach (BranchState branch in currentBoard.branchStates)
            {
                if (branch.branchColor == AIcolor || branch.ownerColor == AIcolor)
                {
                    aiOwnedBranches.Add(branch.location);
                }
            }

            List<int> possibleBranchMoves = new List<int>();

            foreach (int ownedBranch in aiOwnedBranches)
            {
                int[] connectingBranches = ReferenceScript.branchConnectsToTheseBranches[ownedBranch];

                foreach (int branch in connectingBranches)
                {
                    if (currentBoard.branchStates[branch].branchColor == PlayerColor.Blank && (currentBoard.branchStates[branch].ownerColor == PlayerColor.Blank || currentBoard.branchStates[branch].ownerColor == AIcolor))
                    {
                        possibleBranchMoves.Add(currentBoard.branchStates[branch].location);
                    }
                }
            }
            if (possibleBranchMoves.Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }

        }

        public void ResourceTradingForBranches(int[] aiResources, int[] initialResources, BoardState currentBoardState, PlayerColor color, ref int trad)
        {
            int[] debug = (int[])aiResources.Clone();
            for (int i = 0; i <2 ; i++)
            {
                if (initialResources[i] == 0 || aiResources[0] + aiResources[1] + aiResources[2] + aiResources[3] > 8)
                {
                    switch (i)
                    {
                        case 0:
                            if (aiResources[i] == 0 && trad == 0)
                            {
                                if (aiResources[1] + aiResources[2] + aiResources[3] >= 3)
                                {
                                    int max = aiResources.Max();
                                    int index = Array.IndexOf(aiResources, max);
                                    aiResources[index]--;
                                    aiResources[i]++;
                                    trad = 1;
                                }
                            }
                            break;
                        case 1:
                            if (aiResources[i] == 0 && trad == 0)
                            {
                                if (aiResources[0] + aiResources[2] + aiResources[3] >= 3)
                                {
                                    int max = aiResources.Max();
                                    int index = Array.IndexOf(aiResources, max);
                                    aiResources[index]--;
                                    aiResources[i]++;
                                    trad = 1;
                                }
                            }
                            break;
                   
                    }
                }
            }
        }

        public void ResourceTradingForNodes(int[] aiResources, int[] initialResources, BoardState currentBoardState, PlayerColor color, ref int trad)
        {
            int[] debug = (int[])aiResources.Clone();
            for (int i = 2; i < 4; i++)
            {
                if (initialResources[i] == 0 || aiResources[0] + aiResources[1] + aiResources[2] + aiResources[3] > 8)
                {
                    switch (i)
                    {
                        case 2:
                            if (aiResources[i] < 2 && trad == 0 && IsValidNodeMoves(currentBoardState, color) == true)
                            {
                                if (aiResources[0] + aiResources[1] + aiResources[3] >= 3)
                                {
                                    int max = aiResources.Max();
                                    int index = Array.IndexOf(aiResources, max);
                                    aiResources[index]--;
                                    aiResources[i]++;
                                    trad = 1;
                                }
                            }
                            break;
                        case 3:
                            if (aiResources[i] < 2 && trad == 0 && IsValidNodeMoves(currentBoardState, color) == true)
                            {
                                if (aiResources[0] + aiResources[1] + aiResources[2] >= 3)
                                {
                                    int max = aiResources.Max();
                                    int index = Array.IndexOf(aiResources, max);
                                    aiResources[index]--;
                                    aiResources[i]++;
                                    trad = 1;
                                }
                            }
                            break;
                    }
                }
            }
        }

        public List<int> CalculatePossibleBranches(BoardState currentBoard, int[] aiResources, PlayerColor CurrentPlayerColor)
        {
            List<int> aiOwnedBranches = new List<int>();

            foreach (BranchState branch in currentBoard.branchStates)
            {
                if (branch.branchColor == CurrentPlayerColor || branch.ownerColor == CurrentPlayerColor)
                {
                    aiOwnedBranches.Add(branch.location);
                }
            }

            List<int> possibleBranchMoves = new List<int>();

            foreach (int ownedBranch in aiOwnedBranches)
            {
                int[] connectingBranches = ReferenceScript.branchConnectsToTheseBranches[ownedBranch];

                foreach (int branch in connectingBranches)
                {
                    if (currentBoard.branchStates[branch].branchColor == PlayerColor.Blank && (currentBoard.branchStates[branch].ownerColor == PlayerColor.Blank || currentBoard.branchStates[branch].ownerColor == CurrentPlayerColor))
                    {
                        possibleBranchMoves.Add(currentBoard.branchStates[branch].location);
                    }
                }
            }
            if (possibleBranchMoves.Count > 0 && aiResources[0] >= 1 && aiResources[1] >= 1)
            {
                aiResources[0]--;
                aiResources[1]--;
            }
            else
            {
                possibleBranchMoves.Clear();
            }
            return possibleBranchMoves;
        }

        public List<int> CalculatePossibleNodes(BoardState currentBoard, int[] aiResources, PlayerColor CurrentPlayerColor)
        {
            List<int> aiOwnedBranches = new List<int>();

            foreach (BranchState branch in currentBoard.branchStates)
            {
                if (branch.branchColor == CurrentPlayerColor || branch.ownerColor == CurrentPlayerColor)
                {
                    aiOwnedBranches.Add(branch.location);
                }
            }

            List<int> possibleNodeMoves = new List<int>();

            foreach (int ownedBranch in aiOwnedBranches)
            {
                int[] connectingNodes = ReferenceScript.branchesConnectToTheseNodes[ownedBranch];

                foreach (int node in connectingNodes)
                {
                    if (currentBoard.nodeStates[node].nodeColor == PlayerColor.Blank)
                    {
                        possibleNodeMoves.Add(currentBoard.nodeStates[node].location);
                    }
                }
            }
            if (possibleNodeMoves.Count > 0 && aiResources[2] >= 2 && aiResources[3] >= 2)
            {
                aiResources[2] -= 2;
                aiResources[3] -= 2;
            }
            else
            {
                possibleNodeMoves.Clear();
            }
            return possibleNodeMoves;
        }

        public List<int> GetPlayersBranches(BoardState currentBoard, PlayerColor playerColor)
        {
            List<int> ownedBranches = new List<int>();

            foreach (BranchState branch in currentBoard.branchStates)
            {
                if (branch.branchColor == playerColor)
                {
                    ownedBranches.Add(branch.location);
                }
            }
            return ownedBranches;
        }

        public int CalculatePlayerLongestNetwork(BoardState currentBoard, PlayerColor playerColor)
        {
            int longestNetwork = 0;
            int currentNetwork = 0;
            List<int> runningNetworkBranches = new List<int>();

            List<int> playerBranches = GetPlayersBranches(currentBoard, playerColor);

            runningNetworkBranches.Add(playerBranches[0]);
            currentNetwork++;
            foreach (int ownedBranch in playerBranches)
            {
                if (!runningNetworkBranches.Contains(ownedBranch))
                {
                    longestNetwork = currentNetwork;
                    currentNetwork = 0;
                    runningNetworkBranches.Clear();
                }

                int[] touchingBranches = ReferenceScript.branchConnectsToTheseBranches[ownedBranch];
                foreach (int touchedBranch in touchingBranches)
                {
                    if (!runningNetworkBranches.Contains(touchedBranch) && playerBranches.Contains(touchedBranch))
                    {
                        runningNetworkBranches.Add(touchedBranch);
                        currentNetwork++;
                    }
                }
            }
            if (currentNetwork > longestNetwork)
                longestNetwork = currentNetwork;

            return longestNetwork;
        }

        public List<int> getBlankBranches(BoardState currentBoard, int squareId)
        {
            List<int> blankBranches = new List<int>();
            for (int branch = 0; branch < 4; ++branch)
            {
                int branchId = Reference.branchesOnSquareConnections[squareId, branch];
                if (currentBoard.branchStates[branchId].branchColor == PlayerColor.Blank)
                {
                    blankBranches.Add(branchId);
                }
            }
            return blankBranches;
        }

        public int getConnectedSquare(int branchId, int squareId)
        {
            int branchDirection = -1;
            for (int i = 0; branchDirection == -1 && i < 4; ++i)
            {
                if (Reference.branchesOnSquareConnections[squareId, i] == branchId)
                {
                    branchDirection = i;
                }
            }
            return Reference.squareOnSquareConnections[squareId, branchDirection];
        }

        public bool isConnectedSquareCaptured(BoardState currentBoard, int square, List<int> checkedSquares, List<int> captures, List<int> possibleCaptures)
        {
            List<int> blankBranches = getBlankBranches(currentBoard, square);
            checkedSquares.Add(square);

            foreach (int blankBranchId in blankBranches)
            {
                int connectedSquareId = getConnectedSquare(blankBranchId, square);
                if (!checkedSquares.Contains(connectedSquareId))
                {
                    if (!possibleCaptures.Contains(connectedSquareId) ||
                        !isConnectedSquareCaptured(currentBoard, connectedSquareId, checkedSquares, captures, possibleCaptures))
                    {
                        possibleCaptures.Remove(square);
                        return false;
                    }
                }
            }
            return true;
        }

        public bool isCaptured(BoardState currentBoard, int startingSquare, List<int> captures, List<int> possibleCaptures)
        {
            List<int> blankBranches = getBlankBranches(currentBoard, startingSquare);
            List<int> checkedSquares = new List<int>();
            checkedSquares.Add(startingSquare);

            foreach (int blankBranch in blankBranches)
            {
                int connectedSquareId = getConnectedSquare(blankBranch, startingSquare);
                if (!possibleCaptures.Contains(connectedSquareId) ||
                    !isConnectedSquareCaptured(currentBoard, connectedSquareId, checkedSquares, captures, possibleCaptures))
                {
                    possibleCaptures.Remove(startingSquare);
                    return false;
                }
            }
            return true;
        }

        public PlayerColor getOpponentColor(PlayerColor currentPlayer)
        {
            if (currentPlayer == PlayerColor.Blank)
            {
                return PlayerColor.Blank;
            }

            if (currentPlayer == PlayerColor.Silver)
            {
                return PlayerColor.Gold;
            }
            else
            {
                return PlayerColor.Silver;
            }
        }

        public void DetectMultiTileCaptures(BoardState currentBoard)
        {
            const int MAX_SQUARES = 13;
            List<int> captures = new List<int>();
            List<int> possibleCaptures = new List<int>();

            for (int currentSquare = 0; currentSquare < MAX_SQUARES; ++currentSquare)
            {
                bool couldBeCaptured = true;
                List<int> blankBranches = new List<int>();
                // The color of the first branch found on the square with a player's piece associated with it.

                PlayerColor currentCaptureColor = currentBoard.branchStates[Reference.branchesOnSquareConnections[currentSquare, 0]].branchColor;
                // Look for a player's color.
                for (int connectedBranch = 1; currentCaptureColor == PlayerColor.Blank && connectedBranch < 4; ++connectedBranch)
                {
                    currentCaptureColor = currentBoard.branchStates[Reference.branchesOnSquareConnections[currentSquare, connectedBranch]].branchColor;
                }
                // If no player has placed a branch along this square then it can be captured, but only if it's surrounded by captured squares,
                //  so we'll assign it's possible color later and ignore the following for loop. 
                if (currentCaptureColor == PlayerColor.Blank)
                {
                    couldBeCaptured = false;
                    possibleCaptures.Add(currentSquare);
                }

                // If there is at least one branch that has a player's piece on it then check if the other branches are either blank or that color.
                for (int connectedBranch = 0; connectedBranch < 4 && couldBeCaptured; ++connectedBranch)
                {
                    BranchState currentBranchState = currentBoard.branchStates[Reference.branchesOnSquareConnections[currentSquare, connectedBranch]];

                    // If the node has an unclaimed branch boardering the edge of the board then it cannot be captured.
                    if (currentBranchState.branchColor == PlayerColor.Blank && Reference.squareOnSquareConnections[currentSquare, connectedBranch] == -1)
                    {
                        couldBeCaptured = false;
                    }
                    // If a connecting branch belongs to the opponent then it cannot be captured.
                    else if (currentBranchState.branchColor == getOpponentColor(currentCaptureColor))
                    {
                        couldBeCaptured = false;
                    }
                    // If a connecting branch is blank and there's a tile on the otherside, we need to check that tile for capture.
                    else if (currentBranchState.branchColor == PlayerColor.Blank)
                    {
                        blankBranches.Add(currentBranchState.location);
                    }
                    // Otherwise the branch should belong to the currentCaptureColor and nothing needs to happen.
                }

                // If the node can possibly be captured, add it to possibleCaptures.
                if (couldBeCaptured)
                {
                    // If the none of the branches connected to the tile are blank, it's a single tile capture.
                    if (blankBranches.Count == 0)
                    {
                        captures.Add(currentSquare);
                    }
                    else
                    {
                        possibleCaptures.Add(currentSquare);
                    }
                }
            }

            // Check the list of possible captures for actual captures.
            while (possibleCaptures.Count > 0)
            {
                int square = possibleCaptures.First();
                if (isCaptured(currentBoard, square, captures, possibleCaptures))
                {
                    captureArea(currentBoard, square, possibleCaptures, captures);
                }
                else
                {
                    possibleCaptures.Remove(square);
                }
            }

            foreach (int squareId in captures)
            {
                PlayerColor captureColor = getCapturedSquareOwner(currentBoard, squareId);

                currentBoard.squareStates[squareId].ownerColor = captureColor;
                currentBoard.squareStates[squareId].resourceState = SquareStatus.Captured;
            }
        }

        public int GetNumberOfPlayerNodes(BoardState currentBoard, PlayerColor playerColor)
        {
            int ownedNodes = 0;

            foreach (NodeState node in currentBoard.nodeStates)
            {
                if (node.nodeColor == playerColor)
                {
                    ownedNodes++;
                }
            }
            return ownedNodes;
        }

        public int GetNumberOfPlayerCapturedTiles(BoardState currentBoard, PlayerColor playerColor)
        {
            int ownedTiles = 0;
            DetectMultiTileCaptures(currentBoard);

            foreach (SquareState square in currentBoard.squareStates)
            {
                if (square.resourceState == SquareStatus.Captured && square.ownerColor == playerColor)
                {
                    ownedTiles++;
                }
            }
            return ownedTiles;
        }

        public int isEnd(BoardState currentBoard, PlayerColor AIcolor)
        {
            int playerOneScore = GetNumberOfPlayerNodes(currentBoard, PlayerColor.Silver);
            int playerTwoScore = GetNumberOfPlayerNodes(currentBoard, PlayerColor.Gold);
            playerOneScore += GetNumberOfPlayerCapturedTiles(currentBoard, PlayerColor.Silver);
            playerTwoScore += GetNumberOfPlayerCapturedTiles(currentBoard, PlayerColor.Gold);

            int playerOneNetwork = CalculatePlayerLongestNetwork(currentBoard, PlayerColor.Silver);
            int playerTwoNetwork = CalculatePlayerLongestNetwork(currentBoard, PlayerColor.Gold);

            if (playerOneNetwork > playerTwoNetwork)
            {
                playerOneScore += 2;
            }
            else if (playerOneNetwork < playerTwoNetwork)
            {
                playerTwoScore += 2;
            }
            if(AIcolor == PlayerColor.Silver)
            {
                return (playerOneScore - playerTwoScore);
            }
            else
            {
                return (playerTwoScore-playerOneScore );
            }
        }

        public BoardState CopyBoard(BoardState myBoard)
        {
            BoardState newBoard = new BoardState();
            newBoard.squareStates = new SquareState[13];
            newBoard.nodeStates = new NodeState[24];
            newBoard.branchStates = new BranchState[36];
            for (int i = 0; i < myBoard.squareStates.Length; i++)
            {
                newBoard.squareStates[i] = myBoard.squareStates[i];
            }
            for (int i = 0; i < myBoard.nodeStates.Length; i++)
            {
                newBoard.nodeStates[i] = myBoard.nodeStates[i];
            }
            for (int i = 0; i < myBoard.branchStates.Length; i++)
            {
                newBoard.branchStates[i] = myBoard.branchStates[i];
            }
            return newBoard;
        }

        //********************************************
    }

    public class UCT
    {
        public static TreeNode findBestNode(TreeNode node, TreeNode root)
        {
            List<double> score = new List<double>();
            for (int i = 0; i < node.child.Count; i++)
            {
                if (node.child[i].N == 0)
                {
                    score.Add(99999);
                }
                else
                {
                    double aut = (node.child[i].W / node.child[i].N) + (Math.Sqrt(2) * Math.Sqrt(Math.Log(Math.E, root.N)) / node.child[i].N);
                    score.Add(aut);
                }
            }
            double max = -99999;
            int loc = -1;
            for (int i = 0; i < score.Count; i++)
            {
                if (max <= score[i])
                {
                    loc = i;
                    max = score[i];
                }
            }
            if (loc == -1)
            {
                loc = score.Count - 1;
            }
            //Debug.Log("Best node: " + loc);
            return node.child[loc];
        }
    }
}


public struct MyBoard
{
    public BoardState boardState;
    public int[] aiResources;
    public int[] playerResources;

    public BoardState CopyBoard(BoardState myBoard)
    {
        BoardState newBoard = new BoardState();
        newBoard.squareStates = new SquareState[13];
        newBoard.nodeStates = new NodeState[24];
        newBoard.branchStates = new BranchState[36];
        for (int i = 0; i < myBoard.squareStates.Length; i++)
        {
            newBoard.squareStates[i] = myBoard.squareStates[i];
        }
        for (int i = 0; i < myBoard.nodeStates.Length; i++)
        {
            newBoard.nodeStates[i] = myBoard.nodeStates[i];
        }
        for (int i = 0; i < myBoard.branchStates.Length; i++)
        {
            newBoard.branchStates[i] = myBoard.branchStates[i];
        }
        return newBoard;
    }

    public MyBoard Clone()
    {
        BoardState temp = CopyBoard(boardState);
        int[] tempp = (int[])aiResources.Clone();
        int[] temppp = (int[])playerResources.Clone();
        MyBoard ttt = new MyBoard();
        ttt.aiResources = tempp;
        ttt.playerResources = temppp;
        ttt.boardState = temp;
        return ttt;
    }
}
