using System.Linq;

namespace GoldenCity.Models
{
    public class Bandits
    {
        // private HashSet<Building> buildingsToRaidHashes;
        // public readonly List<Point> Path;
        private readonly GameSetting gameSetting;

        public Bandits(GameSetting gameSetting)
        {
            BuildingsToRaid = new Building[3 - gameSetting.SheriffsCount];
            // buildingsToRaidHashes = new HashSet<Building>();
            // Path = new List<Point> {Point.Empty};
            this.gameSetting = gameSetting;
        }
        //make path to draw it
        public Building[] BuildingsToRaid { get; private set; }

        public void FindBuildingsToRaid() //Считается ли это за ДП и нетривиальный алгоритм?
        {
            var currentMinBudgetWeakness = -1;
            foreach (var building in gameSetting.Map)
            {
                if (building == null || building.BudgetWeakness < currentMinBudgetWeakness || building.WorkerId < 0)
                    continue;
                AddBuildingToRaid(building);
                currentMinBudgetWeakness = BuildingsToRaid.First(b => b != null).BudgetWeakness;
            }
            // buildingsToRaidHashes = buildingsToRaid.ToHashSet();
            // ComposeBanditsPath();
        }

        public void Raid()
        {
           var budgetToRob = BuildingsToRaid.Where(b => b != null).Sum(b => b.BudgetWeakness) 
               * gameSetting.Money / 100;
           gameSetting.ChangeMoney(-budgetToRob);
           foreach (var building in BuildingsToRaid.Where(b => b != null))
           {
               // if (building.WorkerId >= 0)
                   gameSetting.DeleteCitizen(building.WorkerId);
           }
        }

        private void AddBuildingToRaid(Building building)
        {
            BuildingsToRaid[0] = building;
            
            for (var i = 1; i < BuildingsToRaid.Length; i++)
            {
                if (BuildingsToRaid[i] == null || BuildingsToRaid[i - 1].BudgetWeakness >= BuildingsToRaid[i].BudgetWeakness)
                {
                    var tmp = BuildingsToRaid[i];
                    BuildingsToRaid[i] = BuildingsToRaid[i - 1];
                    BuildingsToRaid[i - 1] = tmp;
                }
            }
        }

        // private void ComposeBanditsPath()
        // {
        //     var startPoint = Point.Empty;
        //     while (buildingsToRaidHashes.Count > 0)
        //     {
        //         var shortPath = FindPaths(startPoint).OrderBy(p => p.Length).First();
        //         startPoint = shortPath.Value;
        //         Path.AddRange(shortPath.Skip(1).Reverse().ToList());
        //         buildingsToRaidHashes.Remove(gameSetting.Map[shortPath.Value.Y, shortPath.Value.X]);
        //     }
        // }
        //
        // private IEnumerable<SinglyLinkedList<Point>> FindPaths(Point start) //поиск в ширину
        // {
        //     var visited = new HashSet<Point> {start};
        //     var queue = new Queue<SinglyLinkedList<Point>>();
        //     queue.Enqueue(new SinglyLinkedList<Point>(start));
        //     while (queue.Count != 0)
        //     {
        //         var currentPoint = queue.Dequeue();
        //         if (buildingsToRaidHashes.Contains(gameSetting.Map[currentPoint.Value.Y, currentPoint.Value.X]))
        //             yield return currentPoint;
        //         foreach (var nextPoint in FindNeighbourPoints(currentPoint.Value))
        //         {
        //             if (visited.Contains(nextPoint) && !IsPointInMapBounds(nextPoint))
        //                 continue;
        //             visited.Add(nextPoint);
        //             queue.Enqueue(new SinglyLinkedList<Point>(nextPoint, currentPoint));
        //         }
        //     }
        // }
        //
        // private static IEnumerable<Point> FindNeighbourPoints(Point point)
        // {
        //     for (var i = -1; i <= 1; i += 2)
        //     {
        //         yield return new Point(point.X + i, point.Y);
        //         yield return new Point(point.X, point.Y + i);
        //     }
        // }
        //
        // private bool IsPointInMapBounds(Point point)
        // {
        //     return point.X >= 0 && point.X < gameSetting.Map.Rank
        //                         && point.Y >= 0 && point.Y < gameSetting.Map.Rank;
        // }
        
        
        // public class SinglyLinkedList<T> : IEnumerable<T>
        // {
        //     public readonly T Value;
        //     public readonly SinglyLinkedList<T> Previous;
        //     public readonly int Length;
        //
        //     public SinglyLinkedList(T value, SinglyLinkedList<T> previous = null)
        //     {
        //         Value = value;
        //         Previous = previous;
        //         Length = previous?.Length + 1 ?? 1;
        //     }
        //
        //     public IEnumerator<T> GetEnumerator()
        //     {
        //         yield return Value;
        //         var pathItem = Previous;
        //         while (pathItem != null)
        //         {
        //             yield return pathItem.Value;
        //             pathItem = pathItem.Previous;
        //         }
        //     }
        //
        //     IEnumerator IEnumerable.GetEnumerator()
        //     {
        //         return GetEnumerator();
        //     }
        // }
    }
}