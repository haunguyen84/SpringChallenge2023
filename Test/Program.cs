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
    public List<int> NeighbourIndexes = new List<int>();
    public int Type;
    public int InitialResources;
    public int Resources;
    public int MyAnts;
    public int OppAnts;
    public int Strength;
    public Dictionary<int, LinkedList<Cell>> HarvestingPaths = new Dictionary<int, LinkedList<Cell>>(); // key is baseIndex
    public Dictionary<int, LinkedList<Cell>> AttackingPaths = new Dictionary<int, LinkedList<Cell>>(); // key is baseIndex
}

public enum CellType
{
    Empty = 0,
    Egg = 1,
    Crystal = 2
}

/**
 * Auto-generated code below aims at helping you parse
 * the standard input according to the problem statement.
 **/
class Player
{
    public static Dictionary<int, Cell> CellsDict = new Dictionary<int, Cell>();
    public static List<int> MyBaseIndexes = new List<int>();
    public static List<int> OppBaseIndexes = new List<int>();
    public static int[,] DistanceCache;
    public static int TotalInitialCrystals = 0;
    public static int TotalInitialEggs = 0;
    public static int TotalCrystals = 0;
    public static int TotalEggs = 0;
    public static List<Cell> HasEggCells = new List<Cell>();
    public static List<Cell> HasCrystalCells = new List<Cell>();
    //public static Dictionary<int, Dictionary<int, int>> AttackCache = new Dictionary<int, Dictionary<int, int>>();
    public static Dictionary<int, List<Cell>> MyHarvestingCells = new Dictionary<int, List<Cell>>(); // key is baseIndex, value is a list of harvesting cells
    public static Dictionary<int, List<Cell>> MyAttackingCells = new Dictionary<int, List<Cell>>(); // key is baseIndex, value is a list of attacking cells
    public static Dictionary<int, string> Commands = new Dictionary<int, string>();
    public static Dictionary<int, List<int>> PathsFromBase = new Dictionary<int, List<int>>(); // key is baseIndex, value is the current path from base 

    // Game settings
    public static double LowCrystalRatio = 0.7;
    public static double LowEggRatio = 0.5;
    public static double HaveManyAntsRatio = 1.2;

    static void Main(string[] args)
    {
        string[] inputs;

        int numberOfCells = 103; 
        //numberOfCells = int.Parse(Console.ReadLine()); // amount of hexagonal cells in this map
        Print($"numberOfCells {numberOfCells}");

        DistanceCache = new int[numberOfCells, numberOfCells];
        
        List<string> dumpInputInitialStr = new List<string>()
        {
            "2 51 1 3 5 2 4 6",
            "0 0 -1 7 3 0 6 16",
            "0 0 0 5 15 -1 8 4",
            "2 54 7 9 11 5 0 1",
            "2 54 6 0 2 8 10 12",
            "0 0 3 11 13 15 2 0",
            "0 0 16 1 0 4 12 14",
            "0 0 -1 19 9 3 1 -1",
            "0 0 4 2 -1 -1 20 10",
            "0 0 19 21 23 11 3 7",
            "0 0 12 4 8 20 22 24",
            "0 0 9 23 25 13 5 3",
            "0 0 14 6 4 10 24 26",
            "2 51 11 25 27 29 15 5",
            "2 51 30 16 6 12 26 28",
            "0 0 5 13 29 31 -1 2",
            "0 0 32 -1 1 6 14 30",
            "0 0 33 35 -1 -1 32 56",
            "0 0 -1 31 55 34 36 -1",
            "0 0 37 39 21 9 7 -1",
            "0 0 10 8 -1 38 40 22",
            "0 0 39 41 43 23 9 19",
            "0 0 24 10 20 40 42 44",
            "0 0 21 43 45 25 11 9",
            "0 0 26 12 10 22 44 46",
            "0 0 23 45 47 27 13 11",
            "0 0 28 14 12 24 46 48",
            "2 12 25 47 49 51 29 13",
            "2 12 52 30 14 26 48 50",
            "1 12 13 27 51 53 31 15",
            "1 12 54 32 16 14 28 52",
            "2 57 15 29 53 55 18 -1",
            "2 57 56 17 -1 16 30 54",
            "1 21 57 59 35 17 56 -1",
            "1 21 18 55 -1 58 60 36",
            "0 0 59 61 37 -1 17 33",
            "0 0 -1 18 34 60 62 38",
            "0 0 61 63 39 19 -1 35",
            "0 0 20 -1 36 62 64 40",
            "0 0 63 65 41 21 19 37",
            "0 0 22 20 38 64 66 42",
            "0 0 65 -1 -1 43 21 39",
            "0 0 44 22 40 66 -1 -1",
            "2 59 41 -1 -1 45 23 21",
            "2 59 46 24 22 42 -1 -1",
            "0 0 43 -1 -1 47 25 23",
            "0 0 48 26 24 44 -1 -1",
            "0 0 45 -1 -1 49 27 25",
            "0 0 50 28 26 46 -1 -1",
            "2 19 47 -1 -1 67 51 27",
            "2 19 68 52 28 48 -1 -1",
            "0 0 27 49 67 69 53 29",
            "0 0 70 54 30 28 50 68",
            "0 0 29 51 69 71 55 31",
            "0 0 72 56 32 30 52 70",
            "0 0 31 53 71 -1 34 18",
            "0 0 -1 33 17 32 54 72",
            "0 0 73 75 59 33 -1 90",
            "0 0 34 -1 89 74 76 60",
            "0 0 75 77 61 35 33 57",
            "0 0 36 34 58 76 78 62",
            "0 0 77 79 63 37 35 59",
            "0 0 38 36 60 78 80 64",
            "1 18 79 81 65 39 37 61",
            "1 18 40 38 62 80 82 66",
            "2 52 81 -1 -1 41 39 63",
            "2 52 42 40 64 82 -1 -1",
            "0 0 49 -1 -1 83 69 51",
            "0 0 84 70 52 50 -1 -1",
            "0 0 51 67 83 85 71 53",
            "0 0 86 72 54 52 68 84",
            "0 0 53 69 85 87 -1 55",
            "0 0 88 -1 56 54 70 86",
            "2 56 -1 -1 75 57 90 102",
            "2 56 58 89 101 -1 -1 76",
            "0 0 -1 91 77 59 57 73",
            "0 0 60 58 74 -1 92 78",
            "0 0 91 93 79 61 59 75",
            "0 0 62 60 76 92 94 80",
            "0 0 93 95 81 63 61 77",
            "0 0 64 62 78 94 96 82",
            "0 0 95 -1 -1 65 63 79",
            "0 0 66 64 80 96 -1 -1",
            "1 15 67 -1 -1 -1 85 69",
            "1 15 -1 86 70 68 -1 -1",
            "0 0 69 83 -1 97 87 71",
            "0 0 98 88 72 70 84 -1",
            "0 0 71 85 97 99 89 -1",
            "0 0 100 90 -1 72 86 98",
            "2 56 -1 87 99 101 74 58",
            "2 56 102 73 57 -1 88 100",
            "2 6 -1 -1 93 77 75 -1",
            "2 6 78 76 -1 -1 -1 94",
            "0 0 -1 -1 95 79 77 91",
            "0 0 80 78 92 -1 -1 96",
            "0 0 -1 -1 -1 81 79 93",
            "0 0 82 80 94 -1 -1 -1",
            "0 0 85 -1 -1 -1 99 87",
            "0 0 -1 100 88 86 -1 -1",
            "2 51 87 97 -1 -1 101 89",
            "2 51 -1 102 90 88 98 -1",
            "0 0 89 99 -1 -1 -1 74",
            "0 0 -1 -1 73 90 100 -1",
        };

        for (int i = 0; i < numberOfCells; i++)
        {
            var inputInitialStr = dumpInputInitialStr[i]; 
            //var inputInitialStr = Console.ReadLine();
            
            //if (i >= 98) Print($"inputInitialStr \"{inputInitialStr}\",");
            
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
            if (neigh0 >= 0) cell.NeighbourIndexes.Add(neigh0);
            if (neigh1 >= 0) cell.NeighbourIndexes.Add(neigh1);
            if (neigh2 >= 0) cell.NeighbourIndexes.Add(neigh2);
            if (neigh3 >= 0) cell.NeighbourIndexes.Add(neigh3);
            if (neigh4 >= 0) cell.NeighbourIndexes.Add(neigh4);
            if (neigh5 >= 0) cell.NeighbourIndexes.Add(neigh5);
            CellsDict.Add(cell.Index, cell);

            // Calculate total initial eggs and crystals
            if (type == 1) // Egg
                TotalInitialEggs += initialResources;
            else if (type == 2) // Crystal
                TotalInitialCrystals += initialResources;
        }
        
        int numberOfBases = 2; 
        //numberOfBases = int.Parse(Console.ReadLine());

        // My base
        var myBaseInputStr = "5 42"; 
        //myBaseInputStr = Console.ReadLine();
        
        Print($"mybaseInputStr: {myBaseInputStr}");
        
        inputs = myBaseInputStr.Split(' ');
        for (int i = 0; i < numberOfBases; i++)
        {
            var idx = int.Parse(inputs[i]);
            MyBaseIndexes.Add(idx);
            MyHarvestingCells.Add(idx, new List<Cell>());
            MyAttackingCells.Add(idx, new List<Cell>());
            PathsFromBase[idx] = new List<int>() { idx};            
            
            Commands.Add(idx, "");
        }

        // Opponent base
        var oppBaseInputStr = "6 41"; 
        //oppBaseInputStr = Console.ReadLine();
        
        Print($"oppBaseInputStr: {oppBaseInputStr}");
        
        inputs = oppBaseInputStr.Split(' ');
        for (int i = 0; i < numberOfBases; i++)
        {
            var idx = int.Parse(inputs[i]);
            OppBaseIndexes.Add(idx);
            PathsFromBase[idx] = new List<int>() { idx };
            
            Commands.Add(idx, "");
        }

        List<string> dumpInputLoopStr = new List<string>()
        {
            "51 0 0",
            "0 0 0",
            "0 0 0",
            "54 0 0",
            "54 0 0",
            "0 10 0",
            "0 0 10",
            "0 0 0",
            "0 0 0",
            "0 0 0",
            "0 0 0",
            "0 0 0",
            "0 0 0",
            "51 0 0",
            "51 0 0",
            "0 0 0",
            "0 0 0",
            "0 0 0",
            "0 0 0",
            "0 0 0",
            "0 0 0",
            "0 0 0",
            "0 0 0",
            "0 0 0",
            "0 0 0",
            "0 0 0",
            "0 0 0",
            "12 0 0",
            "12 0 0",
            "12 0 0",
            "12 0 0",
            "57 0 0",
            "57 0 0",
            "21 0 0",
            "21 0 0",
            "0 0 0",
            "0 0 0",
            "0 0 0",
            "0 0 0",
            "0 0 0",
            "0 0 0",
            "0 0 10",
            "0 10 0",
            "59 0 0",
            "59 0 0",
            "0 0 0",
            "0 0 0",
            "0 0 0",
            "0 0 0",
            "19 0 0",
            "19 0 0",
            "0 0 0",
            "0 0 0",
            "0 0 0",
            "0 0 0",
            "0 0 0",
            "0 0 0",
            "0 0 0",
            "0 0 0",
            "0 0 0",
            "0 0 0",
            "0 0 0",
            "0 0 0",
            "18 0 0",
            "18 0 0",
            "52 0 0",
            "52 0 0",
            "0 0 0",
            "0 0 0",
            "0 0 0",
            "0 0 0",
            "0 0 0",
            "0 0 0",
            "56 0 0",
            "56 0 0",
            "0 0 0",
            "0 0 0",
            "0 0 0",
            "0 0 0",
            "0 0 0",
            "0 0 0",
            "0 0 0",
            "0 0 0",
            "15 0 0",
            "15 0 0",
            "0 0 0",
            "0 0 0",
            "0 0 0",
            "0 0 0",
            "56 0 0",
            "56 0 0",
            "6 0 0",
            "6 0 0",
            "0 0 0",
            "0 0 0",
            "0 0 0",
            "0 0 0",
            "0 0 0",
            "0 0 0",
            "51 0 0",
            "51 0 0",
            "0 0 0",
            "0 0 0"
        };

        // game loop
        while (true)
        {
            var inputScoreStr = "0 0";
            //inputScoreStr = Console.ReadLine();
            
            var inputScores = inputScoreStr.Split(' ');
            int myScore = int.Parse(inputScores[0]);
            int oppScore = int.Parse(inputScores[1]);
            
            // Reset
            TotalCrystals = 0;
            TotalEggs = 0;
            HasEggCells.Clear();
            HasCrystalCells.Clear();

            // Process realtime inputs
            for (int i = 0; i < numberOfCells; i++)
            {
                var inputLoopStr = dumpInputLoopStr[i]; 
                //var inputLoopStr = Console.ReadLine();
                
                if (i >= 0) Print($"inputLoopStr \"{inputLoopStr}\",");
                
                inputs = inputLoopStr.Split(' ');
                int resources = int.Parse(inputs[0]); // the current amount of eggs/crystals on this cell
                int myAnts = int.Parse(inputs[1]); // the amount of your ants on this cell
                int oppAnts = int.Parse(inputs[2]); // the amount of opponent ants on this cell

                var currentCell = CellsDict[i];
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
                
                // Reset existing path and build from cells having ants
                PathsFromBase[myBaseIndex].Clear();
                PathsFromBase[myBaseIndex].Add(myBaseIndex);
                
                var existingPathHavingAnts = BuildPathFromCellsHavingAnts(myBaseIndex);
                
                Print($"--- Existing path");
                PrintPath(PathsFromBase[myBaseIndex]);
                
                // Strategy 1.1: Try to harvest eggs as much as possible
                GoHarvestAllEggCells(myBaseIndex, existingPathHavingAnts);
                
                // Strategy 1.2: Try to harvest crystals when remaining eggs low or have too much ants
                if (IsLowEgg(LowEggRatio) || CheckIfHaveTooMuchAnts(myBaseIndex, HaveManyAntsRatio))
                {
                    GoHarvestAllCrystalCells(myBaseIndex, existingPathHavingAnts);
                }

                // Do nothing
                //Commands[myBaseIndex] += "WAIT"; 
                
                Print($"-----------------------------------------------------");
            }
            
            DoAllMyCommands();
        }
    }

    #region GAME ON

    #region Strategy 1
    
    // Strategy 1.1: Try to harvest eggs as much as possible
    #region Strategy 1.1

    public static List<Cell> SortByDistanceAndResourcesDesc(List<Cell> list, Cell fromCell, int baseIndex)
    {
        return list.OrderByDescending(cell =>
        {
            var shortestPath = FindShortestPathHavingMaxResources(fromCell.Index, cell.Index, baseIndex);
            var totalResources = shortestPath.Sum(cell => cell.Resources);

            return totalResources * GetDistance(shortestPath);
        }).ToList();
    }

    public static void MergePathToExistingPath(LinkedList<Cell> path, List<int> existingPath)
    {
        if (path != null)
        {
            foreach (var cell in path)
            {
                if (!existingPath.Contains(cell.Index))
                {
                    existingPath.Add(cell.Index);
                }
            }
        }
    }

    public static void MergePaths(LinkedList<Cell> sourcePath, LinkedList<Cell> mergePath)
    {
        foreach (var cell in mergePath)
        {
            sourcePath.AddLast(cell);
        }
    }

    public static string BuildBeaconCommandFromPath(int baseIndex)
    {
        var path = PathsFromBase[baseIndex];
        
        StringBuilder commandBuilder = new StringBuilder();
        
        int weight = 1;

        foreach (var index in path)
        {
            commandBuilder.Append($"BEACON {index} {weight};");
        }

        return commandBuilder.ToString();
    }
    
    public static LinkedList<Cell> FindShortestPathHavingMaxAnts(int fromIdx, int ToIdx, int baseIndex)
    {
        // BFS
        LinkedList<int> queue = new LinkedList<int>();
        IDictionary<int, int?> prev = new Dictionary<int, int?>();

        prev[fromIdx] = null;
        queue.AddLast(fromIdx);

        while (queue.Count > 0)
        {
            if (prev.ContainsKey(ToIdx))
            {
                break;
            }
            int head = queue.First.Value;
            queue.RemoveFirst();

            // Only check neighbours having ants
            IList<int> neighbours = GetNeighbours(head).Where(idx =>
            {
                var ants = GetAnts(idx, baseIndex);
                
                return ants > 0;
            }).ToList();            
           
            // Order by amount of ants, then beacon strength, then id of cell
            if (IsFriendly(baseIndex)) 
                neighbours = neighbours.OrderBy(idx => CellsDict[idx].MyAnts).ThenBy(idx => idx).ToList();
            else 
                neighbours = neighbours.OrderBy(idx => CellsDict[idx].OppAnts).ThenBy(idx => idx).ToList();            
            
            // Loop in neighbours
            foreach (int neighbour in neighbours)
            {
                Cell cell = CellsDict[neighbour];
                bool visited = prev.ContainsKey(neighbour);
                
                if (cell.Index >= 0 && !visited)
                {
                    prev[neighbour] = head;
                    queue.AddLast(neighbour);
                }
            }
        }

        if (!prev.ContainsKey(ToIdx))
        {
            return null; // impossibru
        }

        // Reconstruct path
        LinkedList<Cell> path = new LinkedList<Cell>();
        int? current = ToIdx;
        while (current != null)
        {
            path.AddFirst(CellsDict[current.Value]);
            current = prev[current.Value];
        }

        return path;
    }
    
    // Loop all egg cells sorted by Distance DESC and Resources DESC from base
    //  Find shortest path (having max resources) from each cell in PathsFromBase to the current egg cell
    //  Add found path to PathsFromBase (check if a cell already existed in the list)
    public static void GoHarvestAllEggCells(int baseIndex, List<int> existingPathHavingAnts)
    {
        Print($"--- Start GoHarvestAllEggCells");
        
        var sortedList = SortByDistanceAndResourcesDesc(HasEggCells, CellsDict[baseIndex], baseIndex);
        var existingPath = PathsFromBase[baseIndex].Distinct().ToList(); 

        foreach (var resourceCell in sortedList)
        {
            var minDistance = int.MaxValue;
            var maxResource = int.MinValue;
            LinkedList<Cell> bestPath = null;
            
            // Loop in existingPath
            foreach (var idx in existingPath)
            {
                var path = FindShortestPathHavingMaxResources(idx, resourceCell.Index, baseIndex);
                var distance = GetDistance(path);
                var totalResources = path.Sum(cell => cell.Resources);

                // Shorter path having more resources
                if (minDistance > distance
                    || (minDistance == distance && totalResources > maxResource))
                {
                    minDistance = distance;
                    maxResource = totalResources;
                    bestPath = path;
                }
            }
            
            // Loop in existingPathHavingAnts
            foreach (var antCellIdx in existingPathHavingAnts)
            {
                var path = FindShortestPathHavingMaxResources(antCellIdx, resourceCell.Index, baseIndex);
                var distance = GetDistance(path);
                var totalResources = path.Sum(cell => cell.Resources);

                // Shorter path having more resources
                if (minDistance > distance
                    || (minDistance == distance && totalResources > maxResource))
                {
                    minDistance = distance;
                    maxResource = totalResources;
                    
                    // Build best path from base --> current cell having ants --> resourceCell
                    var pathFromBaseToAntCell = FindShortestPathHavingMaxAnts(baseIndex, antCellIdx, baseIndex);
                    var pathFromAntCellToResourceCell =
                        FindShortestPathHavingMaxResources(antCellIdx, resourceCell.Index, baseIndex);
                    
                    Print("pathFromBaseToAntCell");
                    PrintPath(pathFromBaseToAntCell);
                    Print("pathFromAntCellToResourceCell");
                    PrintPath(pathFromAntCellToResourceCell);

                    bestPath = pathFromBaseToAntCell;
                    MergePaths(bestPath, pathFromAntCellToResourceCell);                    
                }
            }

            Print($"bestPath from {baseIndex} to {resourceCell.Index}");
            PrintPath(bestPath);
            
            MergePathToExistingPath(bestPath, existingPath);

            PathsFromBase[baseIndex] = existingPath;
        }

        var command = BuildBeaconCommandFromPath(baseIndex);
        Commands[baseIndex] += command;

        Print($"command: {command}");
        
        Print($"--- End GoHarvestAllEggCells");
    }
    
    #endregion
    
    // Strategy 1.2: Try to harvest crystals when remaining eggs low or have too much ants

    #region Strategy 1.2

    public static List<int> BuildPathFromCellsHavingAnts(int baseIndex)
    {
        Print($"--- Start BuildPathFromCellsHavingAnts");
        
        var path = new List<int>();
        var baseCell = CellsDict[baseIndex];
        var visitedIndexes = new List<int>();
        var baseAnts = IsFriendly(baseIndex) ? baseCell.MyAnts : baseCell.OppAnts;
        
        Print($"baseAnts: {baseAnts}");
        
        if (baseAnts > 0)
        {            
            InternalBuildPathFromCellsHavingAnts(baseCell, visitedIndexes, path, baseIndex);
        }        
        
        PrintPath(path);
        Print($"--- End BuildPathFromCellsHavingAnts");

        return path;
    }

    public static void InternalBuildPathFromCellsHavingAnts(Cell cell, List<int> visitedIndexes, List<int> path, int baseIndex)
    {
        visitedIndexes.Add(cell.Index);
        path.Add(cell.Index);
        
        foreach (var neighbourIndex in cell.NeighbourIndexes)
        {
            var neighbour = CellsDict[neighbourIndex];

            if (!visitedIndexes.Contains(neighbourIndex))
            {                
                var ants = IsFriendly(baseIndex) ? neighbour.MyAnts : neighbour.OppAnts;

                if (ants > 0)
                {                
                    InternalBuildPathFromCellsHavingAnts(neighbour, visitedIndexes, path, baseIndex);
                }                
            }
        }
    }

    // Loop all crystal cells sorted by Distance DESC and Resources DESC from base
    //  Find shortest path (having max resources) from each cell in PathsFromBase to the current crystal cell
    //  Add found path to PathsFromBase (check if a cell already existed in the list)
    public static void GoHarvestAllCrystalCells(int baseIndex, List<int> existingPathHavingAnts)
    {
        var sortedList = SortByDistanceAndResourcesDesc(HasCrystalCells, CellsDict[baseIndex], baseIndex);
        var existingPath = PathsFromBase[baseIndex].Distinct().ToList();
        
        foreach (var resourceCell in sortedList)
        {
            var minDistance = int.MaxValue;
            var maxResource = int.MinValue;
            LinkedList<Cell> bestPath = null;
            
            foreach (var idx in existingPath)
            {
                var path = FindShortestPathHavingMaxResources(idx, resourceCell.Index, baseIndex);
                var distance = GetDistance(path);
                var totalResources = path.Sum(cell => cell.Resources);

                // Shorter path having more resources
                if (minDistance > distance
                    || (minDistance == distance && totalResources > maxResource))
                {
                    minDistance = distance;
                    maxResource = totalResources;
                    bestPath = path;
                }
            }
            
            // Loop in existingPathHavingAnts
            foreach (var antCellIdx in existingPathHavingAnts)
            {
                var path = FindShortestPathHavingMaxResources(antCellIdx, resourceCell.Index, baseIndex);
                var distance = GetDistance(path);
                var totalResources = path.Sum(cell => cell.Resources);

                // Shorter path having more resources
                if (minDistance > distance
                    || (minDistance == distance && totalResources > maxResource))
                {
                    minDistance = distance;
                    maxResource = totalResources;
                    
                    // Build best path from base --> current cell having ants --> resourceCell
                    var pathFromBaseToAntCell = FindShortestPathHavingMaxAnts(baseIndex, antCellIdx, baseIndex);
                    var pathFromAntCellToResourceCell =
                        FindShortestPathHavingMaxResources(antCellIdx, resourceCell.Index, baseIndex);
                    
                    Print("pathFromBaseToAntCell");
                    PrintPath(pathFromBaseToAntCell);
                    Print("pathFromAntCellToResourceCell");
                    PrintPath(pathFromAntCellToResourceCell);

                    bestPath = pathFromBaseToAntCell;
                    MergePaths(bestPath, pathFromAntCellToResourceCell);                    
                }
            }

            Print($"bestPath from {baseIndex} to {resourceCell.Index}");
            PrintPath(bestPath);

            MergePathToExistingPath(bestPath, existingPath);

            PathsFromBase[baseIndex] = existingPath;
        }

        var command = BuildBeaconCommandFromPath(baseIndex);
        Commands[baseIndex] += command;
    }

    #endregion
    
    #endregion
    

    public static void DoHarvestAllResourceCells(int myBaseIndex)
    {
        foreach (var hasCrystalCell in HasCrystalCells)
        {
            Commands[myBaseIndex] += $"LINE {myBaseIndex} {hasCrystalCell.Index} 1;";
        }
        
        foreach (var hasEggCell in HasEggCells)
        {
            Commands[myBaseIndex] += $"LINE {myBaseIndex} {hasEggCell.Index} 1;";
        }
    }

    public static void DoFindMaxCrystalCellsHarvestingByEnemyToAttack(int myBaseIndex)
    {
        var totalAnts = CountTotalAntsFromBase(myBaseIndex);
        var limitTarget = (int)Math.Min(totalAnts / 10, HasCrystalCells.Count);
        
        for (int i = 0; i < limitTarget; i++)
        {            
            Print($"***** Attack round {i}");
            DoFindMaxCrystalCellHarvestingByEnemyToAttack(myBaseIndex);
        }
    }

    public static void DoFindMaxCrystalCellHarvestingByEnemyToAttack(int myBaseIndex)
    {
        Print($"Start DoFindMaxCrystalCellHarvestingByEnemyToAttack");
        
        var attackCell = FindMaxCrystalCellHarvestingByEnemyToAttack(myBaseIndex);
        if (attackCell == null) return;
        
        var attackPath = attackCell.AttackingPaths[myBaseIndex];
        
        Print($"Found a cell to attack from {myBaseIndex}: {attackCell.Index}");

        if (attackCell != null && attackPath.Count > 0)
        {
            Commands[myBaseIndex] = BuildBeaconCommand(attackPath);
            MyAttackingCells[myBaseIndex].Add(attackCell);
            
            DoExtendHarvestingPathToNeighborsHasResource(attackCell, myBaseIndex);
            
            Print($"attackPath {Commands[myBaseIndex]}");
        }
        
        Print($"End DoFindMaxCrystalCellHarvestingByEnemyToAttack");
    }

    public static int CountTotalAntsFromBase(int baseIndex)
    {
        var baseCell = CellsDict[baseIndex];
        var totalAnts = 0;
        
        List<int> visitedIndexes = new List<int>();
        visitedIndexes.Add(baseIndex);

        CountAnts(baseIndex, visitedIndexes, ref totalAnts, baseIndex);

        return totalAnts;
    }

    public static void CountAnts(int idx, List<int> visitedIndexes, ref int totalAnts, int baseIndex)
    {
        var cell = CellsDict[idx];
        var originTotalAnts = totalAnts;
        
        if (IsFriendly(baseIndex))
            totalAnts += cell.MyAnts;
        else
            totalAnts += cell.OppAnts;
        
        visitedIndexes.Add(idx);  

        // Stop if there is no ant
        if (originTotalAnts == totalAnts)
        {
            return;
        }
              
        // Stop if all neighbours has no ant
        var totalAntsInNeighbours = cell.NeighbourIndexes.Sum(idx =>
        {
            if (!visitedIndexes.Contains(idx))
            {
                if (IsFriendly(baseIndex))
                    return CellsDict[idx].MyAnts;
                else
                    return CellsDict[idx].OppAnts;
            }
            return 0;
        });

        if (totalAntsInNeighbours == 0)
        {
            return;
        }
        
        // Count ants in Neighbours
        foreach (var neighbour in cell.NeighbourIndexes)
        {
            if (!visitedIndexes.Contains(neighbour))
            {                
                CountAnts(neighbour, visitedIndexes, ref totalAnts, baseIndex);
            }
        }
    }

    public static double GetMyMaxAttackPower(Cell targetCell, int myBaseIndex, LinkedList<Cell> path = null)
    {
        LinkedList<Cell> shortestPath;

        if (path == null)
        {
            shortestPath = FindShortestPathHavingMaxResources(myBaseIndex, targetCell.Index, myBaseIndex);
        }
        else
        {
            shortestPath = path;
        }
        
        var totalAnts = CountTotalAntsFromBase(myBaseIndex);
        var myAttackPower = (double) totalAnts / (GetDistance(shortestPath) + 1);
        
        Print($"--- targetCell {targetCell.Index} - totalAnts {totalAnts} - GetDistance(shortestPath) {GetDistance(shortestPath)}");        

        return myAttackPower;
    }

    public static int GetMinDistanceFromExistingPath(int toIdx, int myBaseIndex)
    {
        var minDistance = int.MaxValue;
        
        foreach (var cell in CellsDict.Values)
        {
            var ants = IsFriendly(myBaseIndex) ? cell.MyAnts : cell.OppAnts;
            if (ants > 0)
            {
                var distance = GetDistance(cell.Index, toIdx);

                if (minDistance > distance)
                {
                    minDistance = distance;
                }
            }
        }

        return minDistance;
    }

    public static List<Cell> SortResourceCellsByMyMaxAttackPowerDesc(List<Cell> cells, int myBaseIndex)
    {
        var result = cells.Where(cell =>
            {
                var harvestingPathsCount = !cell.HarvestingPaths.ContainsKey(myBaseIndex) ? 0 : cell.HarvestingPaths[myBaseIndex].Count;
                var attackingPathsCount = !cell.AttackingPaths.ContainsKey(myBaseIndex) ? 0 : cell.AttackingPaths[myBaseIndex].Count;
                
                return harvestingPathsCount == 0 && attackingPathsCount == 0;
            })
            .OrderByDescending(cell =>
            {
                var oppAttackPower = GetAttackPower(cell.Index, OppBaseIndexes[0]);            
                var minDistance = GetMinDistanceFromExistingPath(cell.Index, myBaseIndex);            

                return cell.Resources * Math.Abs(oppAttackPower) / Math.Max(1, minDistance);
            })
            .ThenByDescending(cell =>
            {
                return GetMyMaxAttackPower(cell, myBaseIndex);
            });

        return result.ToList();
    }

    public static Cell FindMaxCrystalCellHarvestingByEnemyToAttack(int myBaseIndex)
    {
        // Ignore cells NOT being harvested by enemy
        var cellsBeingHarvestedByEnemy = HasCrystalCells; //.Where(cell => cell.OppAnts > 0).ToList();
        
        // Sort HasCrystalCells by myMaxAttackPower
        SortResourceCellsByMyMaxAttackPowerDesc(cellsBeingHarvestedByEnemy, myBaseIndex);

        Print($"cellsBeingHarvestedByEnemy {cellsBeingHarvestedByEnemy.Count}");
        
        foreach (var crystalCell in cellsBeingHarvestedByEnemy)
        {
            Print($"---Start checking crystalCell {crystalCell.Index}---");
            
            //  Ignore if being attacked
            if (MyAttackingCells[myBaseIndex].Contains(crystalCell))
            {
                continue;
            }            
            
            // Checking attack power            
            var shortestPath = FindShortestPathHavingMaxResources(myBaseIndex, crystalCell.Index, myBaseIndex);
            var oppAttackPower = GetAttackPower(crystalCell.Index, OppBaseIndexes[0]);
            var myMaxAttackPower = GetMyMaxAttackPower(crystalCell, myBaseIndex, shortestPath);
            var minDistance = GetMinDistanceFromExistingPath(crystalCell.Index, myBaseIndex);
            
            Print($"myMaxAttackPower {myMaxAttackPower} oppAttackPower {oppAttackPower}");

            // Check if can attack
            if (myMaxAttackPower > oppAttackPower + minDistance)
            {
                if (!crystalCell.AttackingPaths.ContainsKey(myBaseIndex))
                {
                    crystalCell.AttackingPaths.Add(myBaseIndex, shortestPath);
                }
                else
                {
                    crystalCell.AttackingPaths[myBaseIndex] = shortestPath;
                }
                
                Print($"---End checking crystalCell {crystalCell.Index}---");
                return crystalCell;
            }
        }
        
        return null;
    }

    public static bool CheckIfHaveTooMuchAnts(int myBaseIndex, double haveManyAntsRatio)
    {
        var totalAnts = CountTotalAntsFromBase(myBaseIndex);
        foreach (var oppBaseIndex in OppBaseIndexes)
        {
            var totalOppAnts = CountTotalAntsFromBase(oppBaseIndex);
            
            Print($"CountTotalAntsFromBase myBaseIndex: {myBaseIndex}: {totalAnts} - oppBaseIndex {oppBaseIndex}: {totalOppAnts}");

            if (totalAnts >= totalOppAnts * HaveManyAntsRatio)
            {
                return true;
            }
        }

        return false;
    }
    
    public static void DoFindClosestCellHasMaxCrystalToHarvest(int myBaseIndex, CellType cellType)
    {
        var myHarvestingCells = MyHarvestingCells[myBaseIndex];
        var closestCellHasMaxResourceIndex = FindClosestCellHasMaxResources(myBaseIndex, myBaseIndex, cellType);

        if (!CellsDict.ContainsKey(closestCellHasMaxResourceIndex)) return;
        
        var closestCellHasMaxResource = CellsDict[closestCellHasMaxResourceIndex];
        
        Print($"Found ClosestCellHasMaxCrystal to harvest from {myBaseIndex} is {closestCellHasMaxResourceIndex}");

        if (closestCellHasMaxResourceIndex > -1)
        {
            var path = FindShortestPathHavingMaxResources(myBaseIndex, closestCellHasMaxResourceIndex, myBaseIndex);

            var myAttackPower = GetAttackPower(path, myBaseIndex);
            var oppAttackPower = GetAttackPower(closestCellHasMaxResourceIndex, OppBaseIndexes[0]);
            var currentResource = CellsDict[closestCellHasMaxResourceIndex].Resources;
                    
            Print($"myAttackPower {myAttackPower} oppAttackPower {oppAttackPower}");
            Print($"closestCellHasMaxResourceIndex {closestCellHasMaxResourceIndex} currentResource {currentResource}");

            if (myAttackPower >= oppAttackPower && currentResource > 0)
            {
                Print($"Found new HarvestingPath for {myBaseIndex}: {BuildBeaconCommand(path)}");
                        
                closestCellHasMaxResource.HarvestingPaths.Add(myBaseIndex, path);

                if (!myHarvestingCells.Contains(closestCellHasMaxResource))
                {
                    myHarvestingCells.Add(closestCellHasMaxResource);
                }
                        
                Commands[myBaseIndex] += BuildBeaconCommand(path);
            }                    
        }  
    }

    public static Cell CheckIfNeighborHasResource(Cell cell, int myBaseIndex)
    {
        if (cell is null)
        {
            return null;
        }
        
        var neighbours = SortByCellTypeAndResources(cell.NeighbourIndexes, myBaseIndex);
        
        foreach (var neighbourIdx in neighbours)
        {
            var neighbourCell = CellsDict[neighbourIdx];
            
            if (neighbourCell.Resources > 0)
            {
                return CheckIfNeighborHasResource(neighbourCell, myBaseIndex);
            }
        }

        return null;
    }
    
    public static LinkedList<Cell> GetAllNeighborsHasResource(Cell cell, int baseIndex)
    {
        LinkedList<Cell> neighborsHasResource = new LinkedList<Cell>();
        List<int> visitedIndexes = new List<int>();
        
        visitedIndexes.Add(cell.Index);        

        GetAllNeighborsHasResource(cell, neighborsHasResource, visitedIndexes, baseIndex);    

        return neighborsHasResource;
    }

    public static void GetAllNeighborsHasResource(Cell cell, LinkedList<Cell> neighborsHasResource, List<int> visitedIndexes, int baseIndex)
    {        
        var neighbours = SortByCellTypeAndResources(cell.NeighbourIndexes.Where(idx => CellsDict[idx].Resources > 0).ToList(), baseIndex);
        
        foreach (var neighbourIdx in neighbours)
        {
            var neighbourCell = CellsDict[neighbourIdx];
            
            if (visitedIndexes.Contains(neighbourCell.Index)) continue;
            
            visitedIndexes.Add(neighbourCell.Index);

            if (neighbourCell.Resources > 0)
            {
                neighborsHasResource.AddLast(neighbourCell);
                GetAllNeighborsHasResource(neighbourCell, neighborsHasResource, visitedIndexes, baseIndex);
            }            
        }
    }

    public static void DoFindClosestCellsHasMaxEggToHarvest(int myBaseIndex)
    {
        var totalAnts = CountTotalAntsFromBase(myBaseIndex);
        var limitTarget = (int)Math.Min(totalAnts / 4, HasEggCells.Count);
        
        for (int i = 0; i < limitTarget; i++)
        {            
            DoFindClosestCellHasMaxEggToHarvest(myBaseIndex);
        }
    }

    public static void DoFindClosestCellHasMaxEggToHarvest(int myBaseIndex)
    {
        // Ignore when low crystal left 
        if (IsLowCrystal(LowCrystalRatio))
        {
            return;
        }
        
        // Ignore when already have too many ants and low egg left :) 
        if (CheckIfHaveTooMuchAnts(myBaseIndex, HaveManyAntsRatio) && IsLowEgg(LowEggRatio))
        {
            return;
        }

        var myHarvestingCells = MyHarvestingCells[myBaseIndex];
        var closestCellHasMaxEggIndex = FindClosestCellHasMaxResources(myBaseIndex, myBaseIndex, CellType.Egg);
        
        if (closestCellHasMaxEggIndex < 0) return;
        
        var closestCellHasMaxEgg = CellsDict[closestCellHasMaxEggIndex];
        Print($"FindClosestCellHasMaxEgg from {myBaseIndex} is {closestCellHasMaxEggIndex}");

        if (closestCellHasMaxEggIndex > -1)
        {
            var path = FindShortestPathHavingMaxResources(myBaseIndex, closestCellHasMaxEggIndex, myBaseIndex);

            var myAttackPower = GetAttackPower(path, myBaseIndex);
            var oppAttackPower = GetAttackPower(closestCellHasMaxEggIndex, OppBaseIndexes[0]);
            var currentResource = CellsDict[closestCellHasMaxEggIndex].Resources;
                    
            Print($"myAttackPower {myAttackPower} oppAttackPower {oppAttackPower}");
            Print($"closestCellHasMaxEggIndex {closestCellHasMaxEggIndex} currentResource {currentResource}");

            if (myAttackPower >= oppAttackPower && currentResource > 0)
            {
                Print($"Found new HarvestingPath for {myBaseIndex}: {BuildBeaconCommand(path)}");
                        
                closestCellHasMaxEgg.HarvestingPaths.Add(myBaseIndex, path);

                if (!myHarvestingCells.Contains(closestCellHasMaxEgg))
                {
                    myHarvestingCells.Add(closestCellHasMaxEgg);
                }
                
                Commands[myBaseIndex] += BuildBeaconCommand(path);
                
                DoExtendHarvestingPathToNeighborsHasResource(closestCellHasMaxEgg, myBaseIndex);  
            }                    
        }  
    }

    public static void DoExtendHarvestingPathToNeighborsHasResource(Cell cell, int myBaseIndex)
    {
        // Extend the path if there is neighbour has resources
        var neighboursHasResource = GetAllNeighborsHasResource(cell, myBaseIndex);

        Commands[myBaseIndex] += BuildBeaconCommand(neighboursHasResource); 
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
            Print($"--- Checking harvesting cell: {myHarvestingCell.Index}");
                        
            var myAttackPower = GetMyMaxAttackPower(myHarvestingCell, myBaseIndex);
            var oppAttackPower = GetAttackPower(myHarvestingCell.Index, OppBaseIndexes[0]);
            var currentResource = myHarvestingCell.Resources;

            var minDistance = GetMinDistanceFromExistingPath(myHarvestingCell.Index, myBaseIndex);

            Print($"myAttackPower {myAttackPower} oppAttackPower {oppAttackPower}");
            Print($"myHarvestingCell {myHarvestingCell.Index} currentResource {currentResource}");

            if (myAttackPower >= oppAttackPower + minDistance && currentResource > 0)
            {
                Print($"Harvesting cell: {myHarvestingCell.Index}");
                Commands[myBaseIndex] += BuildBeaconCommand(paths[myBaseIndex]);
                
                DoExtendHarvestingPathToNeighborsHasResource(myHarvestingCell, myBaseIndex);
                
                //Print($"command {myBaseIndex}: {Commands[myBaseIndex]}");
            }
            else
            {
                paths.Remove(myBaseIndex);
                canNotHarvestCells.Add(myHarvestingCell);
            }
        }
    }

    public static bool IsLowCrystal(double ratio)
    {
        //Print($"Check IsLowCrystal: {TotalCrystals <= (TotalInitialCrystals * ratio)}");
        return TotalCrystals <= (TotalInitialCrystals * ratio);
    }
    
    public static bool IsLowEgg(double ratio)
    {
        //Print($"Check IsLowEgg: {TotalCrystals <= (TotalInitialCrystals * ratio)}");
        return TotalEggs <= (TotalInitialEggs * ratio);
    }

    public static void DoCheckIfHarvesting(int myBaseIndex, bool isAttack)
    {
        List<Cell> myHarvertingCells;
        if (isAttack)
        {
            myHarvertingCells = MyAttackingCells[myBaseIndex];
        }
        else
        {
            myHarvertingCells = MyHarvestingCells[myBaseIndex];
        }
        
        var canNotHarvestCells = new List<Cell>();
        
        Print($"DoCheckIfHarvesting myBaseIndex{myBaseIndex} isAttack {isAttack}");

        foreach (var myHarvestingCell in myHarvertingCells)
        {
            Print($"myHarvestingCell {myHarvestingCell.Index}");
            
            // Check if should stop harvesting eggs
            if (!isAttack && IsEgg(myHarvestingCell.Type))
            {
                var isTooMuchAnts = CheckIfHaveTooMuchAnts(myBaseIndex, HaveManyAntsRatio);

                if (IsLowCrystal(LowCrystalRatio) 
                    || (CheckIfHaveTooMuchAnts(myBaseIndex, HaveManyAntsRatio) && IsLowEgg(LowEggRatio)))
                {
                    Print($"Stop harvesting {myHarvestingCell.Index} because IsLowCrystal: {IsLowCrystal(LowCrystalRatio)} - TooMuchAnts {isTooMuchAnts} - IsLowEgg {IsLowEgg(LowEggRatio)}");
                    
                    canNotHarvestCells.Add(myHarvestingCell);
                    continue;
                }
            }
            
            CheckIfHarvesting(myHarvestingCell, myBaseIndex, canNotHarvestCells, isAttack);
        }

        foreach (var canNotHarvestCell in canNotHarvestCells)
        {
            Print($"canNotHarvestCell {canNotHarvestCell.Index}");
            myHarvertingCells.Remove(canNotHarvestCell);
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

    public static List<Cell> SortByMaxHarvestProductivity(List<Cell> cells, int myBaseIndex)
    {
        return cells.Where(cell =>
            {
                var harvestingPathsCount = !cell.HarvestingPaths.ContainsKey(myBaseIndex) ? 0 : cell.HarvestingPaths[myBaseIndex].Count;
                var attackingPathsCount = !cell.AttackingPaths.ContainsKey(myBaseIndex) ? 0 : cell.AttackingPaths[myBaseIndex].Count;
                
                return harvestingPathsCount == 0 && attackingPathsCount == 0;
            })
            .OrderByDescending(cell =>
        {
            var myMaxAttackPower = GetMyMaxAttackPower(cell, myBaseIndex);
            
            Print($"myMaxAttackPower {cell.Index}: {myMaxAttackPower}");
            
            return myMaxAttackPower;
        })
            .ThenByDescending(cell => cell.Resources).ToList();
    }

    public static int FindClosestCellHasMaxResources(int fromIdx, int myBaseIndex, CellType type)
    {
        List<Cell> resourceCells;

        if (IsEgg(type))
        {
            resourceCells = SortByMaxHarvestProductivity(HasEggCells, myBaseIndex);
        }
        else
        {
            resourceCells = SortByMaxHarvestProductivity(HasCrystalCells, myBaseIndex);
        }

        var desiredIndex = -1;
        
        foreach (var resourceCell in resourceCells)
        {
            var path = FindShortestPath(fromIdx, resourceCell.Index, myBaseIndex);
            //var distance = GetDistance(path);

            // Check if enough ants to go there
            if (CheckIfEnoughAntsToGo(path, myBaseIndex))
            {
                desiredIndex = resourceCell.Index;

                break;
            }
        }

        return desiredIndex;
    }

    #endregion

    public static string BuildBeaconCommand(LinkedList<Cell> path)
    {
        StringBuilder commandBuilder = new StringBuilder();

        var weight = 1;
        var i = 0;

        foreach (var cell in path)
        {
            //if (i == path.Count - 1) weight = 2;
            
            commandBuilder.Append($"BEACON {cell.Index} {weight};");
            
            i++;
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
    
    public static bool IsEgg(CellType type)
    {
        return (int)type == 1;
    }

    public static bool IsEgg(int type)
    {
        return type == 1;
    }
    
    public static bool IsCrystal(CellType type)
    {
        return (int)type == 2;
    }
    
    public static bool IsCrystal(int type)
    {
        return type == 2;
    }

    public static void Print(string message)
    {
        Console.Error.WriteLine(message);
    }

    public static void PrintPath(ICollection<int> path)
    {
        if (path == null) return;
        
        var pathStr = "Path: ";
        foreach (var idx in path)
        {
            pathStr += $"{idx} -> ";
        }
        
        Print(pathStr);
    }
    
    public static void PrintPath(ICollection<Cell> path)
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
        return CellsDict[index].NeighbourIndexes;
    }

    public static List<int> SortByCellTypeAndResources(List<int> neighbours, int myBaseIndex)
    {
        var result = new List<int>();
        
        // Order by type, egg resources, crystal resources, ants and then id of cell
        result = neighbours.OrderBy(idx =>
            {
                // Sort by type: egg > crystal > empty
                var type = CellsDict[idx].Type;
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
            .ThenByDescending(idx => CellsDict[idx].Resources) // Sort by resources desc
            .ThenBy(idx => GetDistance(myBaseIndex, idx)) // Sort by distance
            //.ThenByDescending(idx => GetMyMaxAttackPower(CellsDict[idx], myBaseIndex))
            .ThenByDescending(idx => // Sort by ants
            {
                if (IsFriendly(myBaseIndex))
                {
                    return CellsDict[idx].MyAnts;
                }

                return CellsDict[idx].OppAnts;
            })
            .ThenBy(idx => idx).ToList(); // Sort by id

        return result;
    }

    public static LinkedList<Cell> FindShortestPathHavingMaxResources(int fromIdx, int toIdx, int? baseIndex)
    {
        // BFS
        LinkedList<int> queue = new LinkedList<int>();
        IDictionary<int, int?> prev = new Dictionary<int, int?>();

        prev[fromIdx] = null;
        queue.AddLast(fromIdx);

        while (queue.Count > 0)
        {
            if (prev.ContainsKey(toIdx))
            {
                break;
            }
            int head = queue.First.Value;
            queue.RemoveFirst();

            List<int> neighbours = GetNeighbours(head);
           
            if (baseIndex != null)
            {
                // Order by amount of egg resources, crystal resources, ants, then beacon strength, then id of cell
                neighbours = SortByCellTypeAndResources(neighbours, baseIndex.Value);
            }
            else
            {
                // Order by id of cell
                neighbours = neighbours.OrderBy(idx => idx).ToList();
            }
            foreach (int neighbour in neighbours)
            {
                Cell cell = CellsDict[neighbour];
                bool visited = prev.ContainsKey(neighbour);
                if (cell.Index >= 0 && !visited)
                {
                    prev[neighbour] = head;
                    queue.AddLast(neighbour);
                }
            }
        }

        if (!prev.ContainsKey(toIdx))
        {
            return null; // impossibru
        }

        // Reconstruct path
        LinkedList<Cell> path = new LinkedList<Cell>();
        int? current = toIdx;
        while (current != null)
        {
            path.AddFirst(CellsDict[current.Value]);
            current = prev[current.Value];
        }

        return path;
    }

    public static LinkedList<Cell> FindShortestPath(int a, int b, int? baseIndex)
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
            if (baseIndex != null)
            {
                // Order by amount of friendly ants, then beacon strength, then id of cell
                if (MyBaseIndexes.Contains(baseIndex.Value)) 
                    neighbours = neighbours.OrderBy(idx => CellsDict[idx].MyAnts).ThenBy(idx => idx).ToList();
                else 
                    neighbours = neighbours.OrderBy(idx => CellsDict[idx].OppAnts).ThenBy(idx => idx).ToList();
            }
            else
            {
                // Order by id of cell
                neighbours = neighbours.OrderBy(idx => idx).ToList();
            }
            foreach (int neighbour in neighbours)
            {
                Cell cell = CellsDict[neighbour];
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
            path.AddFirst(CellsDict[current.Value]);
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

        return GetDistance(path);
    }

    public static int GetDistance(ICollection path)
    {
        if (path == null) return -1;
        return path.Count - 1;
    }
    
    public static int GetAnts(int cellIdx, int baseIndex)
    {
        if (IsFriendly(baseIndex))
            return CellsDict[cellIdx].MyAnts;
        
        return CellsDict[cellIdx].OppAnts;
    }
    
    public static int GetAttackPower(int cellIdx, int playerIdx)
    {
        Print($"Start GetAttackPower cell {cellIdx} player {playerIdx}");
        
        //int cachedAttackPower = -1;        
        // if (AttackCache.ContainsKey(playerIdx) && AttackCache[playerIdx].ContainsKey(cellIdx))
        // {
        //     cachedAttackPower = AttackCache[playerIdx][cellIdx];
        //     Print($"GetAttackPower from AttackCache {cachedAttackPower}");
        //     return cachedAttackPower;
        // }

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

        // if (!AttackCache.ContainsKey(playerIdx))
        // {
        //     AttackCache.Add(playerIdx, new Dictionary<int, int>());
        // }
        // AttackCache[playerIdx].Add(cellIdx, maxMin);
        
        Print($"End GetAttackPower cell {cellIdx} player {playerIdx}");
        
        return maxMin;
    }    

    /**
     * @return The path that maximizes the given player score between start and end, while minimizing the distance from start to end.
     */
    public static LinkedList<Cell> GetBestPath(int start, int end, int playerIdx, bool interruptedByFight) {
        // Dijkstra's algorithm based on the tuple (maxValue, minDist)

        // TODO: optim: pre-compute all distances from each cell to the end
        int[] maxPathValues = new int[CellsDict.Count];
        int[] prev = new int[CellsDict.Count];
        int[] distanceFromStart = new int[CellsDict.Count];
        bool[] visited = new bool[CellsDict.Count];
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
            
            foreach (int neighborIndex in CellsDict[currentIndex].NeighbourIndexes) 
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
            path.AddFirst(CellsDict[index]);
            //Print($"path.AddFirst {index}");
            index = prev[index];
        }
        return path;
    }
}