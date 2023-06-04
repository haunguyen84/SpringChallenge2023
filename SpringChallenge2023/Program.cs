using System;
using System.Linq;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;

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
    public static LinkedList<Cell>[,] ShortestPathHavingMaxResourcesCache;
    public static LinkedList<Cell>[,] ShortestPathHavingMaxAntsCache;
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
    public static Dictionary<int, List<int>> ExistingPathHavingAnts = new Dictionary<int, List<int>>(); // key is baseIndex, value is the path having ants from base
    public static Dictionary<int, int> TotalAnts = new Dictionary<int, int>(); // key is baseIndex, value is the total amount of ants

    // Game settings
    public static double LowCrystalRatio = 0.2;
    public static double LowEggRatio = 0.2;
    public static double HaveManyAntsRatio = 1.5;

    static void Main(string[] args)
    {
        string[] inputs;

        int numberOfCells = 103; 
        numberOfCells = int.Parse(Console.ReadLine()); // amount of hexagonal cells in this map
        Print($"numberOfCells {numberOfCells}");

        DistanceCache = new int[numberOfCells, numberOfCells];
        //PathCache = new LinkedList<Cell>[numberOfCells, numberOfCells];
        
        List<string> dumpInputInitialStr = new List<string>()
        {
            "2 54 1 3 5 2 4 6",
            "2 56 7 9 3 0 6 16",
            "2 56 0 5 15 8 10 4",
            "0 0 9 11 13 5 0 1",
            "0 0 6 0 2 10 12 14",
            "0 0 3 13 -1 15 2 0",
            "0 0 16 1 0 4 14 -1",
            "0 0 17 19 9 1 16 32",
            "0 0 2 15 31 18 20 10",
            "0 0 19 -1 11 3 1 7",
            "0 0 4 2 8 20 -1 12",
            "0 0 -1 21 23 13 3 9",
            "0 0 14 4 10 -1 22 24",
            "0 0 11 23 25 -1 5 3",
            "0 0 -1 6 4 12 24 26",
            "1 15 5 -1 29 31 8 2",
            "1 15 32 7 1 6 -1 30",
            "0 0 33 35 19 7 32 54",
            "0 0 8 31 53 34 36 20",
            "0 0 35 37 -1 9 7 17",
            "0 0 10 8 18 36 38 -1",
            "0 0 39 41 43 23 11 -1",
            "0 0 24 12 -1 40 42 44",
            "2 16 21 43 45 25 13 11",
            "2 16 26 14 12 22 44 46",
            "0 0 23 45 47 27 -1 13",
            "0 0 28 -1 14 24 46 48",
            "0 0 25 47 49 -1 29 -1",
            "0 0 -1 30 -1 26 48 50",
            "2 42 -1 27 -1 51 31 15",
            "2 42 52 32 16 -1 28 -1",
            "1 15 15 29 51 53 18 8",
            "1 15 54 17 7 16 30 52",
            "0 0 55 57 35 17 54 -1",
            "0 0 18 53 -1 56 58 36",
            "0 0 57 59 37 19 17 33",
            "0 0 20 18 34 58 60 38",
            "0 0 59 61 39 -1 19 35",
            "0 0 -1 20 36 60 62 40",
            "0 0 61 -1 41 21 -1 37",
            "0 0 22 -1 38 62 -1 42",
            "0 0 -1 -1 -1 43 21 39",
            "0 0 44 22 40 -1 -1 -1",
            "0 0 41 -1 -1 45 23 21",
            "0 0 46 24 22 42 -1 -1",
            "0 0 43 -1 -1 47 25 23",
            "0 0 48 26 24 44 -1 -1",
            "0 0 45 -1 -1 49 27 25",
            "0 0 50 28 26 46 -1 -1",
            "2 59 47 -1 -1 63 -1 27",
            "2 59 64 -1 28 48 -1 -1",
            "0 0 29 -1 65 67 53 31",
            "0 0 68 54 32 30 -1 66",
            "0 0 31 51 67 -1 34 18",
            "0 0 -1 33 17 32 52 68",
            "1 16 69 71 57 33 -1 86",
            "1 16 34 -1 85 70 72 58",
            "0 0 71 73 59 35 33 55",
            "0 0 36 34 56 72 74 60",
            "1 25 73 75 61 37 35 57",
            "1 25 38 36 58 74 76 62",
            "0 0 75 77 -1 39 37 59",
            "0 0 40 38 60 76 78 -1",
            "0 0 49 -1 -1 79 65 -1",
            "0 0 80 66 -1 50 -1 -1",
            "1 10 -1 63 79 81 67 51",
            "1 10 82 68 52 -1 64 80",
            "0 0 51 65 81 83 -1 53",
            "0 0 84 -1 54 52 66 82",
            "1 37 87 89 71 55 86 102",
            "1 37 56 85 101 88 90 72",
            "0 0 89 -1 73 57 55 69",
            "0 0 58 56 70 90 -1 74",
            "0 0 -1 91 75 59 57 71",
            "0 0 60 58 72 -1 92 76",
            "0 0 91 93 77 61 59 73",
            "0 0 62 60 74 92 94 78",
            "1 18 93 -1 -1 -1 61 75",
            "1 18 -1 62 76 94 -1 -1",
            "1 18 63 -1 -1 95 81 65",
            "1 18 96 82 66 64 -1 -1",
            "0 0 65 79 95 97 83 67",
            "0 0 98 84 68 66 80 96",
            "0 0 67 81 97 99 85 -1",
            "0 0 100 86 -1 68 82 98",
            "0 0 -1 83 99 101 70 56",
            "0 0 102 69 55 -1 84 100",
            "0 0 -1 -1 89 69 102 -1",
            "0 0 70 101 -1 -1 -1 90",
            "0 0 -1 -1 -1 71 69 87",
            "0 0 72 70 88 -1 -1 -1",
            "2 5 -1 -1 93 75 73 -1",
            "2 5 76 74 -1 -1 -1 94",
            "0 0 -1 -1 -1 77 75 91",
            "0 0 78 76 92 -1 -1 -1",
            "1 20 79 -1 -1 -1 97 81",
            "1 20 -1 98 82 80 -1 -1",
            "2 49 81 95 -1 -1 99 83",
            "2 49 -1 100 84 82 96 -1",
            "0 0 83 97 -1 -1 101 85",
            "0 0 -1 102 86 84 98 -1",
            "2 40 85 99 -1 -1 88 70",
            "2 40 -1 87 69 86 100 -1",
        };

        for (int i = 0; i < numberOfCells; i++)
        {
            //var inputInitialStr = dumpInputInitialStr[i]; 
            var inputInitialStr = Console.ReadLine();
            
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
        
        int numberOfBases = 1; 
        numberOfBases = int.Parse(Console.ReadLine());

        // My base
        var myBaseInputStr = "34"; 
        myBaseInputStr = Console.ReadLine();
        
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
        var oppBaseInputStr = "33"; 
        oppBaseInputStr = Console.ReadLine();
        
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
            "54 0 0",
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
            "0 0 0",
            "0 0 0",
            "16 0 0",
            "16 0 0",
            "0 0 0",
            "0 0 0",
            "0 0 0",
            "0 0 0",
            "42 0 0",
            "42 0 0",
            "15 0 0",
            "15 0 0",
            "0 0 10",
            "0 10 0",
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
            "0 0 0",
            "0 0 0",
            "59 0 0",
            "59 0 0",
            "0 0 0",
            "0 0 0",
            "0 0 0",
            "0 0 0",
            "16 0 0",
            "16 0 0",
            "0 0 0",
            "0 0 0",
            "25 0 0",
            "25 0 0",
            "0 0 0",
            "0 0 0",
            "0 0 0",
            "0 0 0",
            "10 0 0",
            "10 0 0",
            "0 0 0",
            "0 0 0",
            "37 0 0",
            "37 0 0",
            "0 0 0",
            "0 0 0",
            "0 0 0",
            "0 0 0",
            "0 0 0",
            "0 0 0",
            "18 0 0",
            "18 0 0",
            "18 0 0",
            "18 0 0",
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
            "5 0 0",
            "5 0 0",
            "0 0 0",
            "0 0 0",
            "20 0 0",
            "20 0 0",
            "49 0 0",
            "49 0 0",
            "0 0 0",
            "0 0 0",
            "40 0 0",
            "40 0 0",
        };

        // game loop
        while (true)
        {
            var inputScoreStr = "0 0";
            inputScoreStr = Console.ReadLine();
            
            var inputScores = inputScoreStr.Split(' ');
            int myScore = int.Parse(inputScores[0]);
            int oppScore = int.Parse(inputScores[1]);
            
            // Reset
            TotalCrystals = 0;
            TotalEggs = 0;
            HasEggCells.Clear();
            HasCrystalCells.Clear();
            // Reset PathCache
            ShortestPathHavingMaxResourcesCache = new LinkedList<Cell>[numberOfCells, numberOfCells];
            ShortestPathHavingMaxAntsCache = new LinkedList<Cell>[numberOfCells, numberOfCells];

            // Process realtime inputs
            for (int i = 0; i < numberOfCells; i++)
            {
                //var inputLoopStr = dumpInputLoopStr[i]; 
                var inputLoopStr = Console.ReadLine();
                
                //if (i >= 0) Print($"inputLoopStr \"{inputLoopStr}\",");
                
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

            // Build ExistingPathHavingAnts for all my bases
            foreach (var myBaseIndex in MyBaseIndexes)
            {
                ExistingPathHavingAnts[myBaseIndex] = BuildPathFromCellsHavingAnts(myBaseIndex);
            }
            
            // Start to play
            foreach (var myBaseIndex in MyBaseIndexes)
            {
                Print($"myBaseIndex {myBaseIndex}");
                
                // Reset command
                Commands[myBaseIndex] = "";

                // Reset existing path and build from cells having ants
                PathsFromBase[myBaseIndex].Clear();
                PathsFromBase[myBaseIndex].Add(myBaseIndex);
                
                // Calculate total ants
                TotalAnts[myBaseIndex] = CountTotalAntsFromBase(myBaseIndex);
                
                Print($"-----------------------------------------------------");
            }
            
            Print($"---- DoHarvestAllEggCells ----");
            // Strategy 1.1: Try to harvest eggs as much as possible
            DoHarvestAllResourceCells(MyBaseIndexes[0], CellType.Egg);
            
            Print($"-----------------------------------------------------");
            
            Print($"---- DoHarvestAllCrystalCells ----");
            // Strategy 1.2: Try to harvest crystals when remaining eggs low or have too much ants
            if (IsLowEgg(LowEggRatio) || 
                CheckIfHaveTooMuchAnts(MyBaseIndexes[0], HaveManyAntsRatio) ||
                IsLowCrystal(LowCrystalRatio))
            {
                DoHarvestAllResourceCells(MyBaseIndexes[0], CellType.Crystal);
            }
            
            Print($"-----------------------------------------------------");                        
            
            DoAllMyCommands();
        }
    }

    #region GAME ON

    #region Strategy 1
    
    // Strategy 1.1: Try to harvest eggs as much as possible
    #region Strategy 1.1

    public static List<Cell> SortByDistanceAndResourcesDesc(List<Cell> list, Cell fromCell, int baseIndex, CellType type)
    {
        return list.OrderByDescending(cell =>
            {
                var shortestPath = GetShortestPathHavingMaxResources(fromCell.Index, cell.Index, baseIndex);
                var totalResources = shortestPath.Sum(cell =>
                {
                    var weight = 1;
                    
                    if (cell.Type == (int)type)
                    {
                        weight = 10;
                    }
                    
                    return cell.Resources * weight;
                });

                var distanceWeight = 2;
                var distance = Math.Max(1, GetDistance(shortestPath)) * distanceWeight;

                return totalResources / distance;
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
            if (!sourcePath.Contains(cell)) 
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
    
    public static LinkedList<Cell> GetShortestPathHavingMaxAnts(int fromIdx, int toIdx, int baseIndex)
    {
        LinkedList<Cell> path = null;
        
        if (fromIdx == toIdx) {
            path = new LinkedList<Cell>();
            path.AddLast(CellsDict[fromIdx]);

            return path;
        }

        var cachedPath = ShortestPathHavingMaxAntsCache[fromIdx, toIdx];
        if (cachedPath != null) {
            return cachedPath;
        }
        
        path = FindShortestPathHavingMaxAnts(fromIdx, toIdx, baseIndex);
        ShortestPathHavingMaxAntsCache[fromIdx, toIdx] = path;
        ShortestPathHavingMaxAntsCache[toIdx, fromIdx] = path;
        
        return path;
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

    public static LinkedList<Cell> FindBestPath(Cell resourceCell, int baseIndex, List<int> visitedFromIndexes)
    {
        var maxHarvestPower = int.MinValue;
        var maxResource = int.MinValue;
        LinkedList<Cell> bestPath = null;

        var existingPath = PathsFromBase[baseIndex];
        var existingPathHavingAnts = ExistingPathHavingAnts[baseIndex];
        
        // Print($"existingPath of {baseIndex}");
        // PrintPath(existingPath);
        // Print("visitedFromIndexes");
        // PrintPath(visitedFromIndexes);

        var distanceExistingPath = GetDistance(existingPath);
        
        // Loop in existingPath
        foreach (var idx in existingPath)
        {
            if (visitedFromIndexes.Contains(idx)) continue;
            visitedFromIndexes.Add(idx);
                
            var path = GetShortestPathHavingMaxResources(idx, resourceCell.Index, baseIndex);
            var distance = GetDistance(path);
            var totalResources = path.Sum(cell => cell.Resources);
            var harvestPower = TotalAnts[baseIndex] / Math.Max((distanceExistingPath + distance), 1);

            // Shorter path having more resources
            if (maxHarvestPower < harvestPower
                || (maxHarvestPower == harvestPower && totalResources > maxResource))
            {
                maxHarvestPower = harvestPower;
                maxResource = totalResources;
                bestPath = path;
            }
        }
        
        // Loop in existingPathHavingAnts
        foreach (var antCellIdx in existingPathHavingAnts)
        {
            if (visitedFromIndexes.Contains(antCellIdx)) continue;
            visitedFromIndexes.Add(antCellIdx);
            
            var path = GetShortestPathHavingMaxResources(antCellIdx, resourceCell.Index, baseIndex);
            var distance = GetDistance(path);
            var totalResources = path.Sum(cell => cell.Resources);
            var harvestPower = TotalAnts[baseIndex] / Math.Max((distanceExistingPath + distance), 1);

            // Shorter path having more resources
            if (maxHarvestPower < harvestPower
                || (maxHarvestPower == harvestPower && totalResources > maxResource))
            {
                maxHarvestPower = harvestPower;
                maxResource = totalResources;
                
                // Build best path from base --> current cell having ants --> resourceCell
                var pathFromBaseToAntCell = GetShortestPathHavingMaxAnts(baseIndex, antCellIdx, baseIndex);
                var pathFromAntCellToResourceCell = GetShortestPathHavingMaxResources(antCellIdx, resourceCell.Index, baseIndex);
                
                // Print("pathFromBaseToAntCell");
                // PrintPath(pathFromBaseToAntCell);
                // Print("pathFromAntCellToResourceCell");
                // PrintPath(pathFromAntCellToResourceCell);

                bestPath = pathFromBaseToAntCell;
                MergePaths(bestPath, pathFromAntCellToResourceCell);                    
            }
        }
        
        // Check if enough ants to go
        var totalAnts = TotalAnts[baseIndex];
        var totalDistance = GetDistance(PathsFromBase[baseIndex]) + GetDistance(bestPath);
            
        Print($"Check if enough ants to go from {baseIndex}: ants {totalAnts} - totalDistance {totalDistance}");
            
        if (totalAnts < totalDistance)
        {
            return null;
        }

        return bestPath;
    }
    
    // Loop all egg cells sorted by Distance DESC and Resources DESC from base
    //  Find shortest path (having max resources) from each cell in PathsFromBase to the current egg cell
    //  Add found path to PathsFromBase (check if a cell already existed in the list)
    public static void DoHarvestAllResourceCells(int baseIndex, CellType type)
    {
        Print($"--- Start DoHarvestAllResourceCells {type.ToString()}");

        var visitedResourceIndexes = new List<int>();        

        // Loop in egg cells
        List<Cell> resourceCells = new List<Cell>();
        
        if (IsEgg(type))
        {
            resourceCells = HasEggCells;
        }
        else if (IsCrystal(type))
        {
            resourceCells = HasCrystalCells;
        }
        
        var sortedList = SortByDistanceAndResourcesDesc(resourceCells, CellsDict[baseIndex], baseIndex, type);
        
        foreach (var resourceCell in sortedList)
        {
            Print($"+++ Harvesting {resourceCell.Index}");
            
            if (visitedResourceIndexes.Contains(resourceCell.Index)) continue;
            visitedResourceIndexes.Add(resourceCell.Index);
            
            var visitedFromIndexes = new List<int>();

            var minDistance = int.MaxValue;
            var maxResource = int.MinValue;
            var bestPath = new LinkedList<Cell>();
            var bestBaseIndex = baseIndex;
            
            foreach (var myBaseIndex in MyBaseIndexes)
            {
                var path = FindBestPath(resourceCell, myBaseIndex, visitedFromIndexes);
                
                if (path == null) continue;                
                
                var distance = GetDistance(path);
                var resource = path.Sum(cell => cell.Resources);
                
                // Print($"bestPath from {myBaseIndex} to {resourceCell.Index}");
                // PrintPath(path);

                if (minDistance > distance || (minDistance == distance && maxResource < resource))
                {
                    bestPath = path;
                    minDistance = distance;
                    maxResource = resource;
                    bestBaseIndex = myBaseIndex;
                }                
            }  
            
            Print($"--- The final bestPath is from {bestBaseIndex} to {resourceCell.Index}");
            PrintPath(bestPath);
            Print("--------------------------------------------------------------");
            
            MergePathToExistingPath(bestPath, PathsFromBase[bestBaseIndex]);
        }

        foreach (var myBaseIndex in MyBaseIndexes)
        {
            var command = BuildBeaconCommandFromPath(myBaseIndex);
            Commands[myBaseIndex] += command;

            Print($"command of {myBaseIndex}: {command}");
        }        
        
        Print($"--- End DoHarvestAllResourceCells {type.ToString()}");
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

    #endregion
    
    #endregion

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

        InternalCountAnts(baseIndex, visitedIndexes, ref totalAnts, baseIndex);

        return totalAnts;
    }

    public static void InternalCountAnts(int idx, List<int> visitedIndexes, ref int totalAnts, int baseIndex)
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
                InternalCountAnts(neighbour, visitedIndexes, ref totalAnts, baseIndex);
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

    public static List<int> SortByCellTypeAndResources(List<int> neighbours, int baseIndex)
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
            //.ThenBy(idx => GetDistance(myBaseIndex, idx)) // Sort by distance
            //.ThenByDescending(idx => GetMyMaxAttackPower(CellsDict[idx], myBaseIndex))
            .ThenByDescending(idx => // Sort by ants
            {
                if (IsFriendly(baseIndex))
                {
                    return CellsDict[idx].MyAnts;
                }

                return CellsDict[idx].OppAnts;
            })
            .ThenBy(idx => idx).ToList(); // Sort by id

        return result;
    }

    public static LinkedList<Cell> GetShortestPathHavingMaxResources(int fromIdx, int toIdx, int baseIndex)
    {
        LinkedList<Cell> path = null;
        
        if (fromIdx == toIdx) {
            path = new LinkedList<Cell>();
            path.AddLast(CellsDict[fromIdx]);

            return path;
        }

        var cachedPath = ShortestPathHavingMaxResourcesCache[fromIdx, toIdx];
        if (cachedPath != null) {
            return cachedPath;
        }
        
        path = FindShortestPathHavingMaxResources(fromIdx, toIdx, baseIndex);
        ShortestPathHavingMaxResourcesCache[fromIdx, toIdx] = path;
        ShortestPathHavingMaxResourcesCache[toIdx, fromIdx] = path;
        
        return path;
    }

    public static LinkedList<Cell> FindShortestPathHavingMaxResources(int fromIdx, int toIdx, int baseIndex)
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
                neighbours = SortByCellTypeAndResources(neighbours, baseIndex);
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