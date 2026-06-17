using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IP_KiprasRudzinskas
{
    public static class ParallelSA
    {
        public static List<Route> Solve(List<Place> places, int buses, int startNr, int seconds = 60)
        {
            int workers = Environment.ProcessorCount;
            var results = new List<Route>[workers];

            Parallel.For(0, workers, i =>
            {
                var rng = new Random(i * 31 + Environment.TickCount);
                int startIdx = places.FindIndex(p => p.Nr == startNr);
                var current = GenerateRandom(places, buses, startIdx, rng);
                var best = Copy(current);

                double T = 1000.0;
                double Tf = 0.001;
                double alpha = 0.995;
                var deadline = DateTime.Now.AddSeconds(seconds);

                while (DateTime.Now < deadline)
                {
                    var next = SimulatedAnnealing.GetNeighborPublic(current, startIdx, rng, places);
                    double delta = next.Max(r => r.Cost) - current.Max(r => r.Cost);

                    if (delta < 0 || rng.NextDouble() < Math.Exp(-delta / T))
                        current = next;

                    if (current.Max(r => r.Cost) < best.Max(r => r.Cost))
                        best = Copy(current);

                    T *= alpha;
                    if (T < Tf) T = 1000.0;
                }

                results[i] = best;
            });

            return results.OrderBy(r => r.Max(x => x.Cost)).First();
        }

        private static List<Route> Copy(List<Route> routes) =>
            routes.Select(r => new Route { BusId = r.BusId, Path = new List<int>(r.Path), Cost = r.Cost }).ToList();

        private static List<Route> GenerateRandom(List<Place> places, int buses, int startIdx, Random rng)
        {
            var indices = Enumerable.Range(0, places.Count).Where(i => i != startIdx).OrderBy(_ => rng.Next()).ToList();
            var routes = Enumerable.Range(0, buses).Select(b => new Route { BusId = b, Path = new List<int> { startIdx } }).ToList();

            for (int i = 0; i < indices.Count; i++)
                routes[i % buses].Path.Add(indices[i]);

            foreach (var r in routes)
            {
                r.Path.Add(startIdx);
                r.Cost = SimulatedAnnealing.CalcCost(r.Path, places);
            }

            return routes;
        }
    }
}
