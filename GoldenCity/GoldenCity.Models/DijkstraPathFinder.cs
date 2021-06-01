using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace GoldenCity.Models
{
    public class DijkstraPathFinder
    {
        private static readonly Point DefaultPoint = new Point(int.MinValue, int.MinValue);
        
        public static IEnumerable<PathWithCost> GetPathsByDijkstra(GameSetting gameSetting, Point start,
            IEnumerable<Point> targets)
        {
            var visitedPoints = new HashSet<Point>();
            var fastTargets = new HashSet<Point>(targets);
            var track = new Dictionary<Point, DijkstraData>
            {
                [start] = new DijkstraData() {Price = 0, Previous = DefaultPoint}
            };
            while (fastTargets.Count != 0)
            {
                var toOpen = GetPointToOpen(track, visitedPoints);

                if (toOpen == DefaultPoint)
                    break;
                
                if (fastTargets.Contains(toOpen))
                {
                    fastTargets.Remove(toOpen);
                    yield return ConvertToPathWithCost(toOpen, track);
                }

                AddNextPointsToTrack(gameSetting, toOpen, track);
                visitedPoints.Add(toOpen);
            }
        }

        private static Point GetPointToOpen(Dictionary<Point, DijkstraData> track, HashSet<Point> visitedPoints)
        {
            var toOpen = DefaultPoint;
            var bestPrice = double.NegativeInfinity;
            foreach (var point in track.Keys
                .Where(point => !visitedPoints.Contains(point)))
            {
                if (track[point].Price > bestPrice)
                {
                    bestPrice = track[point].Price;
                    toOpen = point;
                }
            }

            return toOpen;
        }

        private static void AddNextPointsToTrack(GameSetting gameSetting, Point currentPoint, Dictionary<Point, DijkstraData> track)
        {
            foreach (var nextPoint in currentPoint.IncidentPoints())
            {
                if (!gameSetting.IsInsideMap(nextPoint))
                    continue;

                var nextPointPrice = 0;
                if (gameSetting.Map[nextPoint.Y, nextPoint.X] != null)
                    nextPointPrice = gameSetting.Map[nextPoint.Y, nextPoint.X].BudgetWeakness;
                
                var currentPrice = track[currentPoint].Price + nextPointPrice;
                if (!track.ContainsKey(nextPoint) || track[nextPoint].Price < currentPrice)
                {
                    track[nextPoint] = new DijkstraData {Previous = currentPoint, Price = currentPrice};
                }
            }
        }

        private static PathWithCost ConvertToPathWithCost(Point point, Dictionary<Point, DijkstraData> track)
        {
            var result = new PathWithCost(track[point].Price);
            while (point != DefaultPoint)
            {
                result.Path.Add(point);
                point = track[point].Previous;
            }

            result.Path.Reverse();
            return result;
        }
        
        private class DijkstraData
        {
            public Point Previous { get; set; }
            public int Price { get; set; }
        }
    }

    public static class PointExtensions
    {
        public static IEnumerable<Point> IncidentPoints(this Point point)
        {
            for (var i = -1; i <= 1; i += 2)
            {
                yield return new Point(point.X + i, point.Y);
                yield return new Point(point.X, point.Y + i);
            }
        }
    }
}