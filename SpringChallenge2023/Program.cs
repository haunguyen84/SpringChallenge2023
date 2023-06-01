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

    static void Main(string[] args)
    {
        string[] inputs;
        int totalInitialResources = 0;
        int totalResources = 0;

        int numberOfCells = int.Parse(Console.ReadLine()); // amount of hexagonal cells in this map

        for (int i = 0; i < numberOfCells; i++)
        {
            inputs = Console.ReadLine().Split(' ');
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

            totalInitialResources += initialResources;
        }

        int numberOfBases = int.Parse(Console.ReadLine());
        inputs = Console.ReadLine().Split(' ');
        for (int i = 0; i < numberOfBases; i++)
        {
            MyBaseIndexes.Add(int.Parse(inputs[i]));
        }
        inputs = Console.ReadLine().Split(' ');
        for (int i = 0; i < numberOfBases; i++)
        {
            OppBaseIndexes.Add(int.Parse(inputs[i]));
        }

        // game loop
        string command = "";
        int defaultWeight = 1;

        while (true)
        {
            totalResources = 0;
            List<int> hasResourcesCellIndexes = new List<int>();

            for (int i = 0; i < numberOfCells; i++)
            {
                inputs = Console.ReadLine().Split(' ');
                int resources = int.Parse(inputs[0]); // the current amount of eggs/crystals on this cell
                int myAnts = int.Parse(inputs[1]); // the amount of your ants on this cell
                int oppAnts = int.Parse(inputs[2]); // the amount of opponent ants on this cell

                var currentCell = CellsDic[i];
                currentCell.Resources = resources;
                currentCell.MyAnts = myAnts;
                currentCell.OppAnts = oppAnts;

                totalResources += resources;

                // Find all cells having resources
                if (resources > 0)
                {
                    hasResourcesCellIndexes.Add(i);
                }
            }

            // Create a line from our base to all cells having resources
            foreach (var cellIndex in hasResourcesCellIndexes)
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
            
            Print($"totalInitialResources: {totalInitialResources}");
            Print($"totalResources: {totalResources}");
            
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

    public static LinkedList<int> FindShortestPath(int a, int b)
    {
        return FindShortestPath(a, b, null);
    }

    public static LinkedList<int> FindShortestPath(int a, int b, int? playerIdx)
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
        LinkedList<int> path = new LinkedList<int>();
        int? current = b;
        while (current != null)
        {
            path.AddFirst(current.Value);
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
    
    public static List<Dictionary<int, int>> AttackCache;
    public static int InitialFood = 0;

    public static int GetAttackPower(int cellIdx, int playerIdx) {
        int cachedAttackPower = AttackCache[playerIdx][cellIdx];
        if (cachedAttackPower != null) {
            return cachedAttackPower;
        }

        List<int> anthills;
        if (IsFriendly(playerIdx))
            anthills = MyBaseIndexes;
        else
            anthills = OppBaseIndexes;

        List<LinkedList<Cell>> allPaths = new List<LinkedList<Cell>>();
        foreach (int anthill in anthills) {
            LinkedList<Cell> bestPath = GetBestPath(cellIdx, anthill, playerIdx, false);

            if (bestPath != null) {
                allPaths.Add(bestPath);
            }
        }

        int maxMin = 0;
        if (IsFriendly(playerIdx))
            allPaths.Max(list => list.Min(cell => cell.MyAnts));
        else
            allPaths.Max(list => list.Min(cell => cell.OppAnts));

        AttackCache[playerIdx].Add(cellIdx, maxMin);
        return maxMin;
    }

    public static int GetAnts(int cellIdx, int playerIdx)
    {
        if (IsFriendly(playerIdx))
            return CellsDic[cellIdx].MyAnts;
        
        return CellsDic[cellIdx].OppAnts;
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
        Print($"GetAnts(start, playerIdx) {maxPathValues[start]}");
        distanceFromStart[start] = 0;
        int startAnts = GetAnts(start, playerIdx);
        if (interruptedByFight) {
            int myForce = GetAttackPower(start, playerIdx);
            int otherForce = GetAttackPower(start, 1 - playerIdx);
            if (otherForce > myForce) {
                startAnts = 0;
            }
        }
        if (startAnts > 0) {
            queue.Enqueue(start, start);
        }

        Print($"queue.Count {queue.Count}");
        while (queue.Count > 0 && !visited[end]) {
            int currentIndex = queue.Dequeue();
            Print($"currentIndex {currentIndex}");
            visited[currentIndex] = true;

            // Update the max values of the neighbors
            Print($"CellsDic[currentIndex].Neighbours {CellsDic[currentIndex].Neighbours.Count}");
            foreach (int neighborIndex in CellsDic[currentIndex].Neighbours) {
                int neighborAnts = GetAnts(neighborIndex, playerIdx);
                Print($"neighborAnts {neighborAnts}");
                if (neighborAnts > 0) {
                    if (interruptedByFight) {
                        int myForce = GetAttackPower(neighborIndex, playerIdx);
                        int otherForce = GetAttackPower(neighborIndex, 1 - playerIdx);
                        if (otherForce > myForce) {
                            neighborAnts = 0;
                        }
                    }
                }

                if (!visited[neighborIndex] && neighborAnts > 0) {
                    int potentialMaxPathValue = Math.Min(maxPathValues[currentIndex], neighborAnts);
                    Print($"potentialMaxPathValue {potentialMaxPathValue}");
                    if (potentialMaxPathValue > maxPathValues[neighborIndex]) {
                        maxPathValues[neighborIndex] = potentialMaxPathValue;
                        distanceFromStart[neighborIndex] = distanceFromStart[currentIndex] + 1;
                        prev[neighborIndex] = currentIndex;
                        queue.Enqueue(neighborIndex, neighborIndex);
                        Print($"queue.Enqueue(neighborIndex, neighborIndex) {neighborIndex}");
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
        Print($"Compute the path from start to end {end}");
        while (index != -1) {
            path.AddFirst(CellsDic[index]);
            Print($"path.AddFirst {index}");
            index = prev[index];
        }
        return path;
    }
}