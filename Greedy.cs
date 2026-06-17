using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IP_KiprasRudzinskas
{
    public static class Greedy
    {
        public static List<Route> Solve(List<Place> places, int buses, int startNr)
        {
            int startIdx = places.FindIndex(p => p.Nr == startNr);
            int n = places.Count;

            bool[] visited = new bool[n];
            visited[startIdx] = true;

            var routes = Enumerable.Range(0, buses)
                .Select(b => new Route { BusId = b, Path = new List<int> { startIdx } })
                .ToList();

            int[] curr = Enumerable.Repeat(startIdx, buses).ToArray();
            int left = n - 1;

            while (left > 0)
            {
                for (int b = 0; b < buses && left > 0; b++)
                {
                    int bestJ = -1;
                    double bestD = double.MaxValue;

                    for (int j = 0; j < n; j++)
                    {
                        if (visited[j] || j == curr[b]) continue;
                        double d = Place.Distance(places[curr[b]], places[j]);
                        if (d < bestD) { bestD = d; bestJ = j; }
                    }

                    if (bestJ != -1)
                    {
                        routes[b].Path.Add(bestJ);
                        routes[b].Cost += bestD;
                        visited[bestJ] = true;
                        curr[b] = bestJ;
                        left--;
                    }
                }
            }

            //Grizimas i pradzia
            for (int b = 0; b < buses; b++)
            {
                routes[b].Cost += Place.Distance(places[curr[b]], places[startIdx]);
                routes[b].Path.Add(startIdx);
            }

            return routes;
        }
    }
}
