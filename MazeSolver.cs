using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.IO;


namespace MazeSolver
{
    public enum Directions
    {
        N=1,
        S=2,
        W=4,
        E=8

    }
    class MazeSolver
    {
         int intMazeWidth = 0;
         int intMazeHeight = 0;
         int intXStart = 0;
         int intYStart = 0;
         int intXEnd = 0;
         int intYEnd = 0;
         int[,] intMaze = null;
         int intNumberOfPositionsVisited = 0;
         Boolean bStepthrough = false;

private Dictionary<Directions, int> DirectionX = new Dictionary<Directions, int>
        {
            {Directions.N,0 },
            {Directions.S,0},
            {Directions.W,-1},
            {Directions.E,1}
        };
        private Dictionary<Directions, int> DirectionY = new Dictionary<Directions, int>
        {
            {Directions.N,-1},
            {Directions.S,1},
            {Directions.W,0},
            {Directions.E,0}
        };
        static void Main()
        {        
             try
            {
                MazeSolver instance = new MazeSolver();
                instance._Main();
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        void _Main()
        {try
            {
             string  strStepThrough = string.Empty;
            //*** Prompt user to input file path of text document.
            Console.WriteLine("\r\n Enter full file path of text document.");
            string strFilePath = Console.ReadLine();
            string[] strInput = File.ReadAllLines(strFilePath);
            Console.WriteLine("Would you like to step through each iteration. Yes/No");
            strStepThrough=Console.ReadLine();
                if (strStepThrough == "Yes") { bStepthrough = true;}
            if (strInput != null)
            {
                Initialise(strInput);
                List<string> lstSolution = solve();
                Output(lstSolution);
                _Main();
            }
            }
            catch(IOException)
            {
                Console.WriteLine("Cannot find file path, please try again.");
                _Main();
            }
        }
/// <summary>
/// User inputs file path. This is then loaded into globals variables.
/// </summary>
        void Initialise(string[] strInput)
        {
            try
            {

                //*** Set maze dimensions, start and end point.
                intMazeWidth = int.Parse(strInput[0].Split(' ')[0]);
                intMazeHeight = int.Parse(strInput[0].Split(' ')[1]);
                intXStart = int.Parse(strInput[1].Split(' ')[0]);
                intYStart = int.Parse(strInput[1].Split(' ')[1]);
                intXEnd = int.Parse(strInput[2].Split(' ')[0]);
                intYEnd = int.Parse(strInput[2].Split(' ')[1]);
                intNumberOfPositionsVisited = 0;
                //*** Import maze
                intMaze = new int[intMazeHeight, intMazeWidth];
                for (int y = 0; y <= intMazeHeight - 1; y++)
                {
                    string[] strMazeRow = strInput[y + 3].Split(' ');
                    for (int x = 0; x <= intMazeWidth - 1; x++)
                    {
                        intMaze[y, x] = int.Parse(strMazeRow[x]);
                    }
                }
            }
            catch(Exception ex)
            { Console.Write(ex.Message);
            }
        }
        List<string> solve()
        {
            try
            {
                int[] intStartPos = new int[2];
                intStartPos[0] = intYStart;
                intStartPos[1] = intXStart;
                int[] intEndPos = new int[2];
                intEndPos[0] = intYEnd;
                intEndPos[1] = intXEnd;
                Boolean bExit = false;
                //*** A dictionary of the nodes that have been evaluated with their position as the key.
                Dictionary<string, EvaluatedNode> dctEvaluated = new Dictionary<string, EvaluatedNode> { };
                //*** A dictionary of the nodes that are in the queue with their optimistic minimum path      
                Dictionary<string, EvaluatedNode> dctQueue = new Dictionary<string, EvaluatedNode>
                {{PositionToKey(intStartPos), new EvaluatedNode(intStartPos,intStartPos,0,GetShortestPathLength(intStartPos))}};
                List<int[]> lstNeighbours = new List<int[]> { };
                while (dctQueue.Count > 0 && !bExit)
                {
                    //*** Select node in the queue with the shortest optimistic path to the end.
                    KeyValuePair<string, EvaluatedNode> kvpCurrentNode = dctQueue.OrderBy(KeyValuePair => KeyValuePair.Value.TotaloptimisticScore).FirstOrDefault();
                    foreach (int[] intNeig in lstNeighbours)
                    {
                        EvaluatedNode ndeNeighbour = new EvaluatedNode();
                        int[] intNeighOptimisticDistance = new int[2];
                        dctQueue.TryGetValue(PositionToKey(intNeig), out ndeNeighbour);
                        if(ndeNeighbour.TotaloptimisticScore==kvpCurrentNode.Value.TotaloptimisticScore)
                        {
                            kvpCurrentNode = new KeyValuePair<string, EvaluatedNode>(PositionToKey(intNeig),ndeNeighbour);
                            break;
                        }
                    }

                    lstNeighbours = GetAllowedNeighbours(kvpCurrentNode.Value.Position, ref dctQueue, ref dctEvaluated);
                    dctQueue.Remove(kvpCurrentNode.Key);                   
                    foreach(int[] intNeighbour in lstNeighbours)
                    {
                        //optimistic path length is the current cost + shortest path to end
                        int intTotalOptimisticScore = kvpCurrentNode.Value.Cost + GetShortestPathLength(intNeighbour);
                        //Cost is previous cost +1
                        int intCost = kvpCurrentNode.Value.Cost + 1;
                        EvaluatedNode ndeNew = new EvaluatedNode(intNeighbour,kvpCurrentNode.Value.Position,intCost, intTotalOptimisticScore);
                        //***Add node to queue
                        dctQueue.Add(PositionToKey(intNeighbour), ndeNew);
                        if (PositionToKey(intNeighbour) == PositionToKey(intEndPos))
                        { bExit = true;
                            dctEvaluated.Add(PositionToKey(intNeighbour), new EvaluatedNode(intNeighbour, kvpCurrentNode.Value.Position, kvpCurrentNode.Value.Cost + 1, kvpCurrentNode.Value.Cost + 1));
                          break;
                        } 
                    }
                    dctEvaluated.Add(kvpCurrentNode.Key, kvpCurrentNode.Value);
                    if (bStepthrough)
                    {
                        Output(ReversePath(ref dctEvaluated, kvpCurrentNode.Value.Position, intStartPos));
                        Console.WriteLine("Press enter to see next step, or type 'Cancel' to proceed directly to solution.");
                        if (Console.ReadLine() != string.Empty) { bStepthrough = false;  };
                    }                    
                }
                if (dctQueue.Count==0)
                {
                    return null;
                }

                this.intNumberOfPositionsVisited = dctQueue.Count + dctEvaluated.Count;
                return ReversePath(ref dctEvaluated, intEndPos,intStartPos);
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
                _Main();
                return null;
            }
        }
        List<string> ReversePath(ref Dictionary<string, EvaluatedNode> dctEvaluated,int[] intEndPos, int[] intStartPos)
        {
            //*** Reverse path to find which positions are used for the solution
            EvaluatedNode node = new EvaluatedNode();
            dctEvaluated.TryGetValue(PositionToKey(intEndPos), out node);
            List<string> lstPath = new List<string> { };
            while (PositionToKey(node.Position) != PositionToKey(intStartPos))
            {
                lstPath.Add(PositionToKey(node.Position));
                dctEvaluated.TryGetValue(PositionToKey(node.CameFrom), out node);
                //  dctEvaluated.Remove(PositionToKey(node.Position));
            }            
            return lstPath;
        }
        void Output(List<string> lstPath)
        {
            if(lstPath==null)
            {
                Console.WriteLine("Maze is not solvable");
                return;
            }
            string strOutput =string.Empty;
            int[] intPath=new int[2];
            for (int y = 0; y <= intMazeHeight - 1; y++)
            {
                for (int x = 0; x <= intMazeWidth - 1; x++)
                {
                    intPath[0] = y;
                    intPath[1] = x;
                    if (intMaze[y, x] == 1)
                    { strOutput += "#"; }
                    else
                    {
                        if (y == intYStart && x == intXStart)
                        {strOutput += "S";
                            continue;
                        }
                        if (y == intYEnd && x == intXEnd)
                        { strOutput += "E";
                            continue;
                        }
                        if (lstPath.Contains(PositionToKey(intPath)))
                        { strOutput += "X"; }
                        else
                        { strOutput += " "; }
                        
                    }
                }
                strOutput += "\r\n";
            }
            Console.Write(strOutput);
            Console.WriteLine("No. of steps =" + lstPath.Count);
            Console.WriteLine("No. of positions visited =" + intNumberOfPositionsVisited);
            }
                //moves N,S,W,E and compares the values to the dictionary object to see if the node has been visited.
        List<int[]> GetAllowedNeighbours(int[] intCurrent, ref Dictionary<string, EvaluatedNode> dctQueue, ref Dictionary<string, EvaluatedNode> dctEvaluated)
        {
            try
            {
                List<int[]> lstOutput = new List<int[]> { };
                int[] intNewPositionEast;
                //East
                intNewPositionEast = MoveX((int[])intCurrent.Clone(),bEast: true);
                if (!dctEvaluated.ContainsKey(PositionToKey(intNewPositionEast)) && !dctQueue.ContainsKey(PositionToKey(intNewPositionEast)) && intMaze[intNewPositionEast[0], intNewPositionEast[1]] == 0)
                {
                    lstOutput.Add(intNewPositionEast);
                }
                //West
                int[] intNewPositionWest;
                intNewPositionWest = MoveX((int[])intCurrent.Clone(), bEast: false);
                if (!dctEvaluated.ContainsKey(PositionToKey(intNewPositionWest)) && !dctQueue.ContainsKey(PositionToKey(intNewPositionWest)) && intMaze[intNewPositionWest[0], intNewPositionWest[1]] == 0)
                {
                    lstOutput.Add(intNewPositionWest);
                }
                //North
                int[] intNewPositionNorth;
                intNewPositionNorth = MoveY((int[])intCurrent.Clone(), bNorth: true);
                if (!dctEvaluated.ContainsKey(PositionToKey(intNewPositionNorth)) && !dctQueue.ContainsKey(PositionToKey(intNewPositionNorth)) && intMaze[intNewPositionNorth[0], intNewPositionNorth[1]] == 0)
                {
                    lstOutput.Add(intNewPositionNorth);
               }
                //South
                int[] intNewPositionSouth;
                intNewPositionSouth = MoveY((int[])intCurrent.Clone(), bNorth: false);
                if (!dctEvaluated.ContainsKey(PositionToKey(intNewPositionSouth)) && !dctQueue.ContainsKey(PositionToKey(intNewPositionSouth)) && intMaze[intNewPositionSouth[0], intNewPositionSouth[1]] == 0)
                {
                    lstOutput.Add(intNewPositionSouth);
                }
                return lstOutput;
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
                return null;
            }
        } 
        /// <summary>
        /// Finds the most optmistic shortest path
        /// </summary>
        int GetShortestPathLength(int intXCoord,int intYCoord)
        {
            int intXDistance = Math.Abs( intXEnd - intXCoord);
            int intYDistance = Math.Abs(intYEnd - intYCoord);
            return intXDistance + intYDistance;
        }
        int GetShortestPathLength(int[] intPosition)
        {
            return GetShortestPathLength(intPosition[0], intPosition[1]);
        }

        int[] MoveX(int[] intPrevPos,Boolean bEast )
        {
            int[] intNewPosition = new int[2];
            intNewPosition = intPrevPos;
            int intIncrement = 0;
            if (bEast)
            {
                DirectionX.TryGetValue(Directions.E, out intIncrement);
            }
            else
            {
                DirectionX.TryGetValue(Directions.W, out intIncrement);
            }
            intNewPosition[1] += intIncrement;
            return intNewPosition;
        }

        int[] MoveY(int[] intPrevPos, Boolean bNorth)
        {
            int[] intNewPosition = new int[2];
            intNewPosition = intPrevPos;
            int intIncrement = 0;
            if (bNorth)
            {
                DirectionY.TryGetValue(Directions.N, out intIncrement);
            }
            else
            {
                DirectionY.TryGetValue(Directions.S, out intIncrement);
            }
            intNewPosition[0] += intIncrement;
            return intNewPosition;
        }
        /// <summary>
        /// type int[] is a reference type, so we convert the unique position int[] to a string to use this as the unique key for the dictionaries.
        /// </summary>
        string PositionToKey(int[] intPosition)
        {
            return intPosition[0].ToString() + ' ' + intPosition[1].ToString();
        }
    }
}
