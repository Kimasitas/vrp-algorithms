namespace IP_KiprasRudzinskas
{
    public static class BranchAndBound
    {
        private static double _bestCost;
        private static List<Route>? _bestRoutes;
        private static bool _timedOut;
        private static DateTime _startTime;
        private static double _maxSeconds;
        private static int _startIdx;
        private static int _buses;
        private static List<Place> _places = null!;

        public static (List<Route>? routes, bool timedOut) Solve(
            List<Place> places, int buses, int startNr, double maxSeconds = 10.0)
        {
            _places = places;
            _buses = buses;
            _startIdx = places.FindIndex(p => p.Nr == startNr);
            _maxSeconds = maxSeconds;
            _startTime = DateTime.Now;
            _timedOut = false;

            //Greedy sprendinys kaip pradine virsutine riba
            var greedy = Greedy.Solve(places, buses, startNr);
            _bestCost = greedy.Max(r => r.Cost);
            _bestRoutes = greedy;

            int n = places.Count;
            bool[] visited = new bool[n];
            visited[_startIdx] = true;

            var routeState = Enumerable.Range(0, buses)
                .Select(_ => new List<int>()).ToList();
            double[] costs = new double[buses];

            Search(routeState, visited, costs, n);

            return (_bestRoutes, _timedOut);
        }

        private static void Search(
            List<List<int>> routeState, bool[] visited, double[] costs, int n)
        {
            if (_timedOut) return;
            if ((DateTime.Now - _startTime).TotalMilliseconds > _maxSeconds)
            {
                _timedOut = true;
                return;
            }

            //Apkarpymas jei kaina virsija geriausia - griztame
            if (costs.Any(c => c >= _bestCost)) return;

            //Ar vietos priskirtos
            if (visited.All(v => v))
            {
                var paths = new List<int>[_buses];
                var rCosts = new double[_buses];
                double maxC = 0;

                for (int b = 0; b < _buses; b++)
                {
                    var path = new List<int>(routeState[b]);
                    double c = costs[b];
                    int last = path.Count > 0 ? path[^1] : _startIdx;
                    c += Place.Distance(_places[last], _places[_startIdx]);
                    path.Add(_startIdx);
                    paths[b] = path;
                    rCosts[b] = c;
                    if (c > maxC) maxC = c;
                }

                if (maxC < _bestCost)
                {
                    _bestCost = maxC;
                    _bestRoutes = Enumerable.Range(0, _buses)
                        .Select(b => new Route
                        {
                            BusId = b,
                            Path = paths[b],
                            Cost = rCosts[b]
                        }).ToList();
                }
                return;
            }

            for (int i = 0; i < n; i++)
            {
                if (visited[i] || i == _startIdx) continue;

                for (int b = 0; b < _buses; b++)
                {
                    int last = routeState[b].Count > 0 ? routeState[b][^1] : _startIdx;
                    double step = Place.Distance(_places[last], _places[i]);

                    if (costs[b] + step >= _bestCost) continue; //apkarpymas

                    routeState[b].Add(i);
                    visited[i] = true;
                    costs[b] += step;

                    Search(routeState, visited, costs, n);

                    costs[b] -= step;
                    visited[i] = false;
                    routeState[b].RemoveAt(routeState[b].Count - 1);

                    if (_timedOut) return;
                }
            }
        }
    }
}
