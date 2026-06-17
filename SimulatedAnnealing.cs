using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IP_KiprasRudzinskas
{
    public static class SimulatedAnnealing
    {
        public static List<Route> Solve(List<Place> places, int buses, int startNr, int seconds = 60)
        {
            var rng = new Random();
            int startIdx = places.FindIndex(p => p.Nr == startNr);

            var current = GenerateRandom(places, buses, startIdx, rng);
            var best = current.Select(r => new Route { BusId = r.BusId, Path = new List<int>(r.Path), Cost = r.Cost }).ToList();

            double T = 1000.0;
            double Tf = 0.001;
            double alpha = 0.995;
            var deadline = DateTime.Now.AddSeconds(seconds);

            while (DateTime.Now < deadline)
            {
                var next = GetNeighborPublic(current, startIdx, rng, places);
                double currMax = current.Max(r => r.Cost);
                double nextMax = next.Max(r => r.Cost);
                double delta = nextMax - currMax;

                if (delta < 0 || rng.NextDouble() < Math.Exp(-delta / T))
                    current = next;

                if (current.Max(r => r.Cost) < best.Max(r => r.Cost))
                    best = current.Select(r => new Route { BusId = r.BusId, Path = new List<int>(r.Path), Cost = r.Cost }).ToList();

                T *= alpha;
                if (T < Tf) T = 1000.0;
            }

            return best;
        }

        private static List<Route> GenerateRandom(List<Place> places, int buses, int startIdx, Random rng)
        {
            var indices = Enumerable.Range(0, places.Count).Where(i => i != startIdx).OrderBy(_ => rng.Next()).ToList();
            var routes = Enumerable.Range(0, buses).Select(b => new Route { BusId = b, Path = new List<int> { startIdx } }).ToList();

            for (int i = 0; i < indices.Count; i++)
                routes[i % buses].Path.Add(indices[i]);

            foreach (var r in routes)
            {
                r.Path.Add(startIdx);
                r.Cost = CalcCost(r.Path, places);
            }

            return routes;
        }

        public static List<Route> GetNeighborPublic(List<Route> routes, int startIdx, Random rng, List<Place> places)
        {
            var next = routes.Select(r => new Route { BusId = r.BusId, Path = new List<int>(r.Path), Cost = r.Cost }).ToList();

            int a = rng.Next(next.Count);
            int b = rng.Next(next.Count);

            // Swap vietas tarp dviejų autobusiukų
            if (next[a].Path.Count > 2 && next[b].Path.Count > 2)
            {
                int ia = rng.Next(1, next[a].Path.Count - 1);
                int ib = rng.Next(1, next[b].Path.Count - 1);
                (next[a].Path[ia], next[b].Path[ib]) = (next[b].Path[ib], next[a].Path[ia]);
            }

            // 2-opt viename maršrute
            int c = rng.Next(next.Count);
            if (next[c].Path.Count > 3)
            {
                int i = rng.Next(1, next[c].Path.Count - 2);
                int j = rng.Next(i + 1, next[c].Path.Count - 1);
                next[c].Path.Reverse(i, j - i + 1);
            }

            foreach (var r in next)
                r.Cost = CalcCost(r.Path, places);

            return next;
        }

        public static double CalcCost(List<int> path, List<Place> places)
        {
            double cost = 0;
            for (int i = 0; i < path.Count - 1; i++)
                cost += Place.Distance(places[path[i]], places[path[i + 1]]);
            return cost;
        }
    }
}
