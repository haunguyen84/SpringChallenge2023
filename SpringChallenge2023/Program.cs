using System;
using System.Linq;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

public class Cell
{
    public int Index;
    public List<int> Neighbours = new List<int>();
    public int Type;
    public int InitialResources;
    public int Resources;
    public int MyAnts;
    public int OppAnts;
    public int Strength;
    public Dictionary<int, LinkedList<Cell>> HarvestingPaths = new Dictionary<int, LinkedList<Cell>>();
    public Dictionary<int, LinkedList<Cell>> AttackingPaths = new Dictionary<int, LinkedList<Cell>>();
}

/**
 * Auto-generated code below aims at helping you parse
 * the standard input according to the problem statement.
 **/
class Player
{
    public static Dictionary<int, Cell> CellsDic = new Dictionary<int, Cell>();
    public static List<int> MyBaseIndexes = new List<int>();
    public static List<int> OppBaseIndexes = new List<int>();
    public static int[,] DistanceCache = new int[100, 100];
    public static int TotalInitialCrystals = 0;
    public static int TotalInitialEggs = 0;
    public static int TotalCrystals = 0;
    public static int TotalEggs = 0;
    public static List<Cell> HasEggCells = new List<Cell>();
    public static List<Cell> HasCrystalCells = new List<Cell>();
    public static Dictionary<int, Dictionary<int, int>> AttackCache = new Dictionary<int, Dictionary<int, int>>();
    public static Dictionary<int, List<Cell>> MyHarvestingCells = new Dictionary<int, List<Cell>>(); // key is baseIndex, value is a list of harvesting cells
    public static Dictionary<int, string> Commands = new Dictionary<int, string>();

    static void Main(string[] args)
    {
        string[] inputs;

        int numberOfCells = 35; 
        numberOfCells = int.Parse(Console.ReadLine()); // amount of hexagonal cells in this map
        Print($"numberOfCells {numberOfCells}");

        List<string> dumpInputInitialStr = new List<string>()
        {
            "0 0 1 3 5 2 4 6",
            "2 42 -1 7 3 0 6 -1",
            "2 42 0 5 -1 -1 8 4",
            "0 0 7 9 11 5 0 1",
            "0 0 6 0 2 8 10 12",
            "0 0 3 11 13 -1 2 0",
            "0 0 -1 1 0 4 12 14",
            "0 0 17 19 9 3 1 -1",
            "0 0 4 2 -1 18 20 10",
            "1 10 19 -1 -1 11 3 7",
            "1 10 12 4 8 20 -1 -1",
            "0 0 9 -1 -1 13 5 3",
            "0 0 14 6 4 10 -1 -1",
            "0 0 11 -1 -1 21 -1 5",
            "0 0 22 -1 6 12 -1 -1",
            "2 50 25 27 17 -1 24 34",
            "2 50 -1 23 33 26 28 18",
            "0 0 27 29 19 7 -1 15",
            "0 0 8 -1 16 28 30 20",
            "0 0 29 -1 -1 9 7 17",
            "0 0 10 8 18 30 -1 -1",
            "2 18 13 -1 -1 31 23 -1",
            "2 18 32 24 -1 14 -1 -1",
            "0 0 -1 21 31 33 16 -1",
            "0 0 34 15 -1 -1 22 32",
            "0 0 -1 -1 27 15 34 -1",
            "0 0 16 33 -1 -1 -1 28",
            "2 7 -1 -1 29 17 15 25",
            "2 7 18 16 26 -1 -1 30",
            "1 13 -1 -1 -1 19 17 27",
            "1 13 20 18 28 -1 -1 -1",
            "0 0 21 -1 -1 -1 33 23",
            "0 0 -1 34 24 22 -1 -1",
            "0 0 23 31 -1 -1 26 16",
            "0 0 -1 25 15 24 32 -1"
        };

        for (int i = 0; i < numberOfCells; i++)
        {
            var inputInitialStr = dumpInputInitialStr[i]; 
            inputInitialStr = Console.ReadLine();
            //Print($"inputInitialStr \"{inputInitialStr}\",");
            inputs = inputInitialStr.Split(' ');
            
            int type = int.Parse(inputs[0]); // 0 for empty, 1 for eggs, 2 for crystal
            int initialResources = int.Parse(inputs[1]); // the initial amount of eggs/crystals on this cell
            int neigh0 = int.Parse(inputs[2]); // the index of the neighbouring cell for each direction
            int neigh1 = int.Parse(inputs[3]);
            int neigh2 = int.Parse(inputs[4]);
            int neigh3 = int.Parse(inputs[5]);
            int neigh4 = int.Parse(inputs[6]);
            int neigh5 = int.Parse(inputs[7]);

            Cell cell = new Cell();
            cell.Index = i;
            cell.Type = type;
            cell.InitialResources = initialResources;
            if (neigh0 >= 0) cell.Neighbours.Add(neigh0);
            if (neigh1 >= 0) cell.Neighbours.Add(neigh1);
            if (neigh2 >= 0) cell.Neighbours.Add(neigh2);
            if (neigh3 >= 0) cell.Neighbours.Add(neigh3);
            if (neigh4 >= 0) cell.Neighbours.Add(neigh4);
            if (neigh5 >= 0) cell.Neighbours.Add(neigh5);
            CellsDic.Add(cell.Index, cell);

            // Calculate total initial eggs and crystals
            if (type == 1) // Egg
                TotalInitialEggs += initialResources;
            else if (type == 2) // Crystal
                TotalInitialCrystals += initialResources;
        }

        int numberOfBases = 2; 
        numberOfBases = int.Parse(Console.ReadLine());

        var myBaseInputStr = "7 31"; 
        myBaseInputStr = Console.ReadLine();
        Print($"mybaseInputStr: {myBaseInputStr}");
        
        inputs = myBaseInputStr.Split(' ');
        for (int i = 0; i < numberOfBases; i++)
        {
            var idx = int.Parse(inputs[i]);
            MyBaseIndexes.Add(idx);
            MyHarvestingCells.Add(idx, new List<Cell>());
            Commands.Add(idx, "");
        }

        var oppBaseInputStr = "8 32"; 
        oppBaseInputStr = Console.ReadLine();
        Print($"oppBaseInputStr: {oppBaseInputStr}");
        
        inputs = oppBaseInputStr.Split(' ');
        for (int i = 0; i < numberOfBases; i++)
        {
            var idx = int.Parse(inputs[i]);
            OppBaseIndexes.Add(idx);
            Commands.Add(idx, "");
        }

        List<string> dumpInputLoopStr = new List<string>()
        {
            "0 0 0",
            "42 0 0",
            "42 0 0",
            "0 0 0",
            "0 0 0",
            "0 0 0",
            "0 0 0",
            "0 14 0",
            "0 0 14",
            "10 0 0",
            "10 0 0",
            "0 0 0",
            "0 0 0",
            "0 0 0",
            "0 0 0",
            "50 0 0",
            "50 0 0",
            "0 0 0",
            "0 0 0",
            "0 0 0",
            "0 0 0",
            "18 0 0",
            "18 0 0",
            "0 0 0",
            "0 0 0",
            "0 0 0",
            "0 0 0",
            "7 0 0",
            "7 0 0",
            "13 0 0",
            "13 0 0",
            "0 14 0",
            "0 0 14",
            "0 0 0",
            "0 0 0"
        };

        // game loop
        while (true)
        {
            // Reset
            TotalCrystals = 0;
            TotalEggs = 0;
            HasEggCells.Clear();
            HasCrystalCells.Clear();

            // Process realtime inputs
            for (int i = 0; i < numberOfCells; i++)
            {
                var inputLoopStr = dumpInputLoopStr[i]; 
                inputLoopStr = Console.ReadLine();
                //Print($"inputLoopStr \"{inputLoopStr}\",");
                
                inputs = inputLoopStr.Split(' ');
                int resources = int.Parse(inputs[0]); // the current amount of eggs/crystals on this cell
                int myAnts = int.Parse(inputs[1]); // the amount of your ants on this cell
                int oppAnts = int.Parse(inputs[2]); // the amount of opponent ants on this cell

                var currentCell = CellsDic[i];
                currentCell.Resources = resources;
                currentCell.MyAnts = myAnts;
                currentCell.OppAnts = oppAnts;

                // Find and calculate all cells which has eggs/crystal 
                if (IsEgg(currentCell.Type) && resources > 0) // Egg
                {
                    TotalEggs += resources;
                    HasEggCells.Add(currentCell);
                }
                else if (IsCrystal(currentCell.Type) && resources > 0) // Crystal
                {
                    TotalCrystals += resources;
                    HasCrystalCells.Add(currentCell);
                }
            }
            
            // Sort resources cells DESC
            HasCrystalCells.Sort((cell1, cell2) => cell2.Resources.CompareTo(cell1.Resources));
            HasEggCells.Sort((cell1, cell2) => cell2.Resources.CompareTo(cell1.Resources));

            //Test();
            
            // GAME ON :)
            Print($"-----------------------------------------------------");
            int defaultWeight = 1;
            
            foreach (var myBaseIndex in MyBaseIndexes)
            {
                Print($"myBaseIndex {myBaseIndex}");
                
                // Reset command
                Commands[myBaseIndex] = "";
                
                // Check if harvesting a cell
                DoCheckIfHarvesting(myBaseIndex, false);                

                if (Commands[myBaseIndex].Length > 0)
                {
                    Print($"-----------------------------------------------------");
                    continue;
                }
                
                // Check if attacking a cell
                DoCheckIfHarvesting(myBaseIndex, true);

                if (Commands[myBaseIndex].Length > 0)
                {
                    Print($"-----------------------------------------------------");
                    continue;
                }
                
                // Find a new cell to harvest eggs                
                if (!IsLowCrystal())
                {
                    DoFindANewEggCellToHarvest(myBaseIndex);  
                }
                else
                {
                    DoFindMaxCrystalCellHarvestingByEnemyToAttack(myBaseIndex);
                }      
                
                Print($"-----------------------------------------------------");
            }
            
            DoAllMyCommands();
        }
    }

    #region GAME ON

    public static void DoFindMaxCrystalCellHarvestingByEnemyToAttack(int myBaseIndex)
    {
        Print($"Start DoFindMaxCrystalCellHarvestingByEnemyToAttack");
        var attackCell = FindMaxCrystalCellHarvestingByEnemyToAttack(myBaseIndex);
        var attackPath = attackCell.AttackingPaths[myBaseIndex];
        
        Print($"Found a cell to attack from {myBaseIndex}: {attackCell.Index}");

        if (attackCell != null && attackPath.Count > 0)
        {
            Commands[myBaseIndex] = BuildBeaconCommand(attackPath);
            MyHarvestingCells[myBaseIndex].Add(attackCell);
            Print($"attackPath {Commands[myBaseIndex]}");
        }
        
        Print($"End DoFindMaxCrystalCellHarvestingByEnemyToAttack");
    }

    public static int CountTotalAntsFromBase(int baseIndex)
    {
        var baseCell = CellsDic[baseIndex];
        var totalAnts = 0;
        
        List<int> visitedIndexes = new List<int>();
        visitedIndexes.Add(baseIndex);

        CountAnts(baseIndex, visitedIndexes, ref totalAnts);

        return totalAnts;
    }

    public static void CountAnts(int idx, List<int> visitedIndexes, ref int totalAnts)
    {
        var cell = CellsDic[idx];
        totalAnts += cell.MyAnts;
        visitedIndexes.Add(idx);

        var totalAntsInNeighbours = cell.Neighbours.Sum(idx =>
        {
            if (!visitedIndexes.Contains(idx))
            {
                return CellsDic[idx].MyAnts;
            }
            return 0;
        });

        if (totalAntsInNeighbours == 0)
        {
            return;
        }
        
        foreach (var neighbour in cell.Neighbours)
        {
            if (!visitedIndexes.Contains(neighbour))
            {                
                CountAnts(neighbour, visitedIndexes, ref totalAnts);
            }
        }
    }

    public static Cell FindMaxCrystalCellHarvestingByEnemyToAttack(int myBaseIndex)
    {
        foreach (var crystalCell in HasCrystalCells)
        {
            Print($"GetAttackPower {OppBaseIndexes[0]}");
            var oppAttackPower1 = GetAttackPower(crystalCell.Index, OppBaseIndexes[0]);
            
            Print($"GetAttackPower {OppBaseIndexes[1]}");
            var oppAttackPower2 = GetAttackPower(crystalCell.Index, OppBaseIndexes[1]);
                        
            var oppAttackPower = Math.Max(oppAttackPower1, oppAttackPower2);

            var shortestPath = FindShortestPathHavingMaxResources(myBaseIndex, crystalCell.Index, myBaseIndex);
            var totalAnts = CountTotalAntsFromBase(myBaseIndex);
            var myAttackPower = totalAnts / (GetDistance(shortestPath) + 1);
            
            Print($"totalAnts {totalAnts} GetDistance(shortestPath) {GetDistance(shortestPath)}");
            Print($"myAttackPower {myAttackPower} oppAttackPower {oppAttackPower}");

            // Check if can attack
            if (myAttackPower > oppAttackPower)
            {
                if (!crystalCell.AttackingPaths.ContainsKey(myBaseIndex))
                {
                    crystalCell.AttackingPaths.Add(myBaseIndex, shortestPath);
                }
                else
                {
                    crystalCell.AttackingPaths[myBaseIndex] = shortestPath;
                }
                
                return crystalCell;
            }
        }

        return null;
    }

    public static void DoFindANewEggCellToHarvest(int myBaseIndex)
    {
        var myHarvestingCells = MyHarvestingCells[myBaseIndex];
        var closestCellHasMaxEggIndex = FindClosestCellHasMaxEgg(myBaseIndex, myBaseIndex);
        var closestCellHasMaxEgg = CellsDic[closestCellHasMaxEggIndex];
        Print($"FindClosestCellHasMaxEgg from {myBaseIndex} is {closestCellHasMaxEggIndex}");

        if (closestCellHasMaxEggIndex > -1)
        {
            var path = FindShortestPathHavingMaxResources(myBaseIndex, closestCellHasMaxEggIndex, myBaseIndex);

            var myAttackPower = GetAttackPower(path, myBaseIndex);
            var oppAttackPower = GetAttackPower(closestCellHasMaxEggIndex, OppBaseIndexes[0]);
            var currentResource = CellsDic[closestCellHasMaxEggIndex].Resources;
                    
            Print($"myAttackPower {myAttackPower}");
            Print($"oppAttackPower {oppAttackPower}");
            Print($"currentResource {currentResource}");

            if (myAttackPower >= oppAttackPower && currentResource > 0)
            {
                Print($"Found new HarvestingPath for {myBaseIndex}: {BuildBeaconCommand(path)}");
                        
                closestCellHasMaxEgg.HarvestingPaths.Add(myBaseIndex, path);

                if (!myHarvestingCells.Contains(closestCellHasMaxEgg))
                {
                    myHarvestingCells.Add(closestCellHasMaxEgg);
                }
                        
                Commands[myBaseIndex] += BuildBeaconCommand(path);
            }                    
        }  
    }

    public static void CheckIfHarvesting(Cell myHarvestingCell, int myBaseIndex, List<Cell> canNotHarvestCells, bool isAttack)
    {
        Dictionary<int, LinkedList<Cell>> paths = myHarvestingCell.HarvestingPaths;
        if (isAttack)
        {
            paths = myHarvestingCell.AttackingPaths;
        }
        
        if (paths.ContainsKey(myBaseIndex))
        {
            Print($"Checking harvesting cell: {myHarvestingCell.Index}");
                        
            var myAttackPower = GetAttackPower(paths[myBaseIndex], myBaseIndex);
            var oppAttackPower = GetAttackPower(myHarvestingCell.Index, OppBaseIndexes[0]);
            var currentResource = myHarvestingCell.Resources;

            Print($"myAttackPower {myAttackPower}");
            Print($"oppAttackPower {oppAttackPower}");
            Print($"currentResource {currentResource}");

            if (myAttackPower >= oppAttackPower && currentResource > 0)
            {
                Print($"Harvesting cell: {myHarvestingCell.Index}");
                Commands[myBaseIndex] += BuildBeaconCommand(paths[myBaseIndex]);
                Print($"command {myBaseIndex}: {Commands[myBaseIndex]}");
            }
            else
            {
                paths.Remove(myBaseIndex);
                canNotHarvestCells.Add(myHarvestingCell);
            }
        }
    }

    public static bool IsLowCrystal()
    {
        Print($"TotalCrystals: {TotalCrystals} TotalInitialCrystals: {TotalInitialCrystals}");
        return TotalCrystals <= (TotalInitialCrystals * 0.75);
    }

    public static void DoCheckIfHarvesting(int myBaseIndex, bool isAttack)
    {
        var myHarvestingCells = MyHarvestingCells[myBaseIndex];
        var canNotHarvestCells = new List<Cell>();
        
        Print($"DoCheckIfHarvesting myBaseIndex{myBaseIndex} isAttack {isAttack}");

        foreach (var myHarvestingCell in myHarvestingCells)
        {
            Print($"myHarvestingCell {myHarvestingCell}");
            
            // Check if should stop harvesting eggs
            if (!isAttack)
            {
                if (IsLowCrystal())
                {
                    canNotHarvestCells.Add(myHarvestingCell);
                    continue;
                }
            }
            
            CheckIfHarvesting(myHarvestingCell, myBaseIndex, canNotHarvestCells, isAttack);
        }

        foreach (var canNotHarvestingCell in canNotHarvestCells)
        {
            Print($"canNotHarvestingCell {canNotHarvestingCell.Index}");
            myHarvestingCells.Remove(canNotHarvestingCell);
        }
    }

    public static void DoAllMyCommands()
    {
        string command = "";
        
        foreach (var myBaseIndex in MyBaseIndexes)
        {
            command += Commands[myBaseIndex];
        }
        
        Console.WriteLine(command);
    }

    public static int GetAttackPower(LinkedList<Cell> path, int playIdx)
    {
        var attackPower = path.Min(cell => IsFriendly(playIdx) ? cell.MyAnts : cell.OppAnts);
        return attackPower;
    }

    public static string GoToClosestCellHasMaxEgg(int fromIdx, int toIdx, int playerIdx)
    {
        string command = "";

        if (toIdx > -1)
        {
            var path = FindShortestPath(fromIdx, toIdx, playerIdx);

            if (path != null)
            {
                command += BuildBeaconCommand(path);
            }
        }

        return command;
    }

    public static int FindClosestCellHasMaxEgg(int fromIdx, int playerIdx)
    {
        // Sort Eggs DESC
        HasEggCells.Sort((cell1, cell2) => cell2.Resources.CompareTo(cell1.Resources));
        
        var maxEgg = int.MinValue;
        var minDistance = int.MaxValue;
        var desiredIndex = -1;
        
        foreach (var eggCell in HasEggCells)
        {
            if (maxEgg <= eggCell.Resources)
            {
                var path = FindShortestPath(fromIdx, eggCell.Index, playerIdx);
                var distance = GetDistance(path);

                // Check if enough ants to go there
                if (CheckIfEnoughAntsToGo(path, playerIdx) && minDistance > distance)
                {
                    desiredIndex = eggCell.Index;
                    minDistance = distance;
                }

                maxEgg = eggCell.Resources;
            }
            else
            {
                break;
            }
        }

        return desiredIndex;
    }

    #endregion

    public static string BuildBeaconCommand(LinkedList<Cell> path)
    {
        StringBuilder commandBuilder = new StringBuilder();

        foreach (var cell in path)
        {
            commandBuilder.Append($"BEACON {cell.Index} 1;");
        }

        return commandBuilder.ToString();
    }

    public static bool CheckIfEnoughAntsToGo(LinkedList<Cell> path, int playerIdx)
    {
        int distance = 0;
        
        foreach (var cell in path)
        {
            if (GetAnts(cell.Index, playerIdx) == 0)
            {
                distance++;
            }
        }
        
        //Print($"CheckIfEnoughAntsToGo {GetAnts(playerIdx, playerIdx)} {distance}");
        return distance <= GetAnts(playerIdx, playerIdx);
    }

    public static void Test()
    {
        string command = "";
        int defaultWeight = 1;
        
        // Create a line from our base to all cells having resources
        foreach (var cellIndex in HasEggCells)
        {
            command += $"LINE {MyBaseIndexes[0]} {cellIndex} {defaultWeight};";
        }

        // Write an action using Console.WriteLine()
        // To debug: Console.Error.WriteLine("Debug messages...");
        foreach (var index in MyBaseIndexes)
        {
            Print($"myBaseIndex: {index}");
        }
            
        foreach (var index in OppBaseIndexes)
        {
            Print($"oppBaseIndex: {index}");
        }
            
        Print($"TotalInitialCrystals: {TotalInitialCrystals}");
        Print($"TotalCrystals: {TotalCrystals}");
        Print($"TotalInitialEggs: {TotalInitialEggs}");
        Print($"TotalEggs: {TotalEggs}");        
            
        // Test FindShortestPath
        var shortestPath = FindShortestPath(30, 27, MyBaseIndexes[0]);
        Print($"FindShortestPath: ");
        PrintPath(shortestPath);

        var distance = GetDistance(5, 50);
        Print($"GetDistance {distance}");
            
        // Test GetBestPath
        Print($"Start GetBestPath");
        var bestPath = GetBestPath(5, 50, MyBaseIndexes[0], false);
        Print($"Found GetBestPath: ");
        PrintPath(bestPath);
        Print($"End GetBestPath: ");

        // WAIT | LINE <sourceIdx> <targetIdx> <strength> | BEACON <cellIdx> <strength> | MESSAGE <text>
        Console.WriteLine(command);
    }

    public static bool IsEgg(int type)
    {
        return type == 1;
    }
    
    public static bool IsCrystal(int type)
    {
        return type == 2;
    }

    public static void Print(string message)
    {
        Console.Error.WriteLine(message);
    }

    public static void PrintPath(LinkedList<int> path)
    {
        if (path == null) return;
        
        var pathStr = "Path: ";
        foreach (var idx in path)
        {
            pathStr += $"{idx} -> ";
        }
        
        Print(pathStr);
    }
    
    public static void PrintPath(LinkedList<Cell> path)
    {
        if (path == null) return;
        
        var pathStr = "Path: ";
        foreach (var cell in path)
        {
            pathStr += $"{cell.Index} -> ";
        }
        
        Print(pathStr);
    }

    public static List<int> GetNeighbours(int index)
    {
        return CellsDic[index].Neighbours;
    }

    public static LinkedList<Cell> FindShortestPath(int a, int b)
    {
        return FindShortestPath(a, b, null);
    }

    public static List<int> SortByResourcesAndAnts(List<int> neighbours, int playerIdx)
    {
        var result = new List<int>();
        
        // Order by type, egg resources, crystal resources, ants and then id of cell
        result = neighbours.OrderBy(idx =>
            {
                // Sort by type: egg > crystal > empty
                var type = CellsDic[idx].Type;
                if (IsEgg(type))
                {
                    return 0;
                }
                else if (IsCrystal(type))
                {
                    return 1;
                }

                return 2;
            })
            .ThenBy(idx => CellsDic[idx].Resources) // Sort by resources
            .ThenBy(idx => // Sort by ants
            {
                if (IsFriendly(playerIdx))
                {
                    return CellsDic[idx].MyAnts;
                }

                return CellsDic[idx].OppAnts;
            })
            .ThenBy(idx => idx).ToList(); // Sort by id

        return result;
    }

    public static LinkedList<Cell> FindShortestPathHavingMaxResources(int a, int b, int? playerIdx)
    {
        // BFS
        LinkedList<int> queue = new LinkedList<int>();
        IDictionary<int, int?> prev = new Dictionary<int, int?>();

        prev[a] = null;
        queue.AddLast(a);

        while (queue.Count > 0)
        {
            if (prev.ContainsKey(b))
            {
                break;
            }
            int head = queue.First.Value;
            queue.RemoveFirst();

            List<int> neighbours = GetNeighbours(head);
           
            if (playerIdx != null)
            {
                // Order by amount of egg resources, crystal resources, ants, then beacon strength, then id of cell
                neighbours = SortByResourcesAndAnts(neighbours, playerIdx.Value);
            }
            else
            {
                // Order by id of cell
                neighbours = neighbours.OrderBy(idx => idx).ToList();
            }
            foreach (int neighbour in neighbours)
            {
                Cell cell = CellsDic[neighbour];
                bool visited = prev.ContainsKey(neighbour);
                if (cell.Index >= 0 && !visited)
                {
                    prev[neighbour] = head;
                    queue.AddLast(neighbour);
                }
            }
        }

        if (!prev.ContainsKey(b))
        {
            return null; // impossibru
        }

        // Reconstruct path
        LinkedList<Cell> path = new LinkedList<Cell>();
        int? current = b;
        while (current != null)
        {
            path.AddFirst(CellsDic[current.Value]);
            current = prev[current.Value];
        }

        return path;
    }

    public static LinkedList<Cell> FindShortestPath(int a, int b, int? playerIdx)
    {
        // BFS
        LinkedList<int> queue = new LinkedList<int>();
        IDictionary<int, int?> prev = new Dictionary<int, int?>();

        prev[a] = null;
        queue.AddLast(a);

        while (queue.Count > 0)
        {
            if (prev.ContainsKey(b))
            {
                break;
            }
            int head = queue.First.Value;
            queue.RemoveFirst();

            IList<int> neighbours = GetNeighbours(head);
            if (playerIdx != null)
            {
                // Order by amount of friendly ants, then beacon strength, then id of cell
                if (MyBaseIndexes.Contains(playerIdx.Value)) 
                    neighbours = neighbours.OrderBy(idx => CellsDic[idx].MyAnts).ThenBy(idx => idx).ToList();
                else 
                    neighbours = neighbours.OrderBy(idx => CellsDic[idx].OppAnts).ThenBy(idx => idx).ToList();
            }
            else
            {
                // Order by id of cell
                neighbours = neighbours.OrderBy(idx => idx).ToList();
            }
            foreach (int neighbour in neighbours)
            {
                Cell cell = CellsDic[neighbour];
                bool visited = prev.ContainsKey(neighbour);
                if (cell.Index >= 0 && !visited)
                {
                    prev[neighbour] = head;
                    queue.AddLast(neighbour);
                }
            }
        }

        if (!prev.ContainsKey(b))
        {
            return null; // impossibru
        }

        // Reconstruct path
        LinkedList<Cell> path = new LinkedList<Cell>();
        int? current = b;
        while (current != null)
        {
            path.AddFirst(CellsDic[current.Value]);
            current = prev[current.Value];
        }

        return path;
    }

    public static bool IsFriendly(int playerIdx)
    {
        return MyBaseIndexes.Contains(playerIdx);
    }
    
    /**
     * @return -1 if no path exist between A and B, otherwise the length of the shortest path
     */
    public static int GetDistance(int a, int b) 
    {
        if (a == b) {
            return 0;
        }

        int cached = DistanceCache[a, b];
        if (cached > 0) {
            return cached;
        }
        
        int distance = InternalGetDistance(a, b, null);
        DistanceCache[a, b] = distance;
        DistanceCache[b, a] = distance;
        return distance;
    }
    
    public static int InternalGetDistance(int a, int b, int? playerIdx) {
        var path = FindShortestPath(a, b, playerIdx);

        return path.Count - 1;
    }

    public static int GetDistance(ICollection path)
    {
        if (path == null) return -1;
        return path.Count - 1;
    }
    
    public static int GetAnts(int cellIdx, int playerIdx)
    {
        if (IsFriendly(playerIdx))
            return CellsDic[cellIdx].MyAnts;
        
        return CellsDic[cellIdx].OppAnts;
    }
    
    public static int GetAttackPower(int cellIdx, int playerIdx)
    {
        int cachedAttackPower = -1;

        Print($"Start GetAttackPower cell {cellIdx} player {playerIdx}");
        if (AttackCache.ContainsKey(playerIdx) && AttackCache[playerIdx].ContainsKey(cellIdx))
        {
            cachedAttackPower = AttackCache[playerIdx][cellIdx];
            Print($"GetAttackPower from AttackCache {cachedAttackPower}");
            return cachedAttackPower;
        }

        List<int> anthills;
        if (IsFriendly(playerIdx))
            anthills = MyBaseIndexes;
        else
            anthills = OppBaseIndexes;

        List<LinkedList<Cell>> allPaths = new List<LinkedList<Cell>>();
        foreach (int anthill in anthills) 
        {
            LinkedList<Cell> bestPath = GetBestPath(cellIdx, anthill, playerIdx, false);
            
            PrintPath(bestPath);

            if (bestPath != null) {
                allPaths.Add(bestPath);
            }
        }

        int maxMin = -1;
        if (allPaths.Count > 0)
        {
            if (IsFriendly(playerIdx))
                maxMin = allPaths.Max(list => list.Min(cell => cell.MyAnts));
            else
                maxMin = allPaths.Max(list => list.Min(cell => cell.OppAnts));
        }

        if (!AttackCache.ContainsKey(playerIdx))
        {
            AttackCache.Add(playerIdx, new Dictionary<int, int>());
        }
        AttackCache[playerIdx].Add(cellIdx, maxMin);
        
        Print($"End GetAttackPower cell {cellIdx} player {playerIdx}");
        
        return maxMin;
    }    

    /**
     * @return The path that maximizes the given player score between start and end, while minimizing the distance from start to end.
     */
    public static LinkedList<Cell> GetBestPath(int start, int end, int playerIdx, bool interruptedByFight) {
        // Dijkstra's algorithm based on the tuple (maxValue, minDist)

        // TODO: optim: pre-compute all distances from each cell to the end
        int[] maxPathValues = new int[CellsDic.Count];
        int[] prev = new int[CellsDic.Count];
        int[] distanceFromStart = new int[CellsDic.Count];
        bool[] visited = new bool[CellsDic.Count];
        Array.Fill(maxPathValues, int.MinValue);
        Array.Fill(prev, -1);
        Array.Fill(visited, false);

        Comparison<int> valueComparison = (i1, i2) => maxPathValues[i2].CompareTo(maxPathValues[i1]);
        Comparison<int> distanceComparison = (i1, i2) =>
            (distanceFromStart[i1] + GetDistance(i1, end)).CompareTo(distanceFromStart[i2] + GetDistance(i2, end));

        // Prioritize Max Value then Min Distance
        Comparison<int> valueDistanceComparison = (i1, i2) =>
        {
            var result = valueComparison(i1, i2);
            if (result == 0)
            {
                return distanceComparison(i1, i2);
            }

            return result;
        };

        PriorityQueue<int, int> queue = new PriorityQueue<int, int>(Comparer<int>.Create(valueDistanceComparison));
        
        maxPathValues[start] = GetAnts(start, playerIdx);
        distanceFromStart[start] = 0;
        int startAnts = GetAnts(start, playerIdx);
        
        //Print($"GetAnts(start, playerIdx) {maxPathValues[start]}");
        
        if (interruptedByFight) {
            int myForce = GetAttackPower(start, playerIdx);
            int otherForce = GetAttackPower(start, OppBaseIndexes[0]);
            if (otherForce > myForce) {
                startAnts = 0;
            }
        }
        if (startAnts > 0) {
            queue.Enqueue(start, start);
        }

        //Print($"queue.Count {queue.Count}");
        while (queue.Count > 0 && !visited[end]) {
            int currentIndex = queue.Dequeue();
            //Print($"currentIndex {currentIndex}");
            visited[currentIndex] = true;

            // Update the max values of the neighbors
            //Print($"CellsDic[currentIndex].Neighbours {CellsDic[currentIndex].Neighbours.Count}");
            
            foreach (int neighborIndex in CellsDic[currentIndex].Neighbours) 
            {
                int neighborAnts = GetAnts(neighborIndex, playerIdx);
                //Print($"neighborAnts {neighborAnts}");
                
                if (neighborAnts > 0) 
                {
                    if (interruptedByFight) 
                    {
                        int myForce = GetAttackPower(neighborIndex, playerIdx);
                        int otherForce = GetAttackPower(neighborIndex, 1 - playerIdx);
                        
                        if (otherForce > myForce) 
                        {
                            neighborAnts = 0;
                        }
                    }
                }

                if (!visited[neighborIndex] && neighborAnts > 0) 
                {
                    int potentialMaxPathValue = Math.Min(maxPathValues[currentIndex], neighborAnts);
                    //Print($"potentialMaxPathValue {potentialMaxPathValue}");
                    
                    if (potentialMaxPathValue > maxPathValues[neighborIndex]) 
                    {
                        maxPathValues[neighborIndex] = potentialMaxPathValue;
                        distanceFromStart[neighborIndex] = distanceFromStart[currentIndex] + 1;
                        prev[neighborIndex] = currentIndex;
                        
                        queue.Enqueue(neighborIndex, neighborIndex);
                        //Print($"queue.Enqueue(neighborIndex, neighborIndex) {neighborIndex}");
                    }
                }
            }
        }

        if (!visited[end]) {
            // No path from start to end
            return null;
        }

        // Compute the path from start to end
        LinkedList<Cell> path = new LinkedList<Cell>();
        int index = end;
        //Print($"Compute the path from start to end {end}");
        while (index != -1) {
            path.AddFirst(CellsDic[index]);
            //Print($"path.AddFirst {index}");
            index = prev[index];
        }
        return path;
    }
}