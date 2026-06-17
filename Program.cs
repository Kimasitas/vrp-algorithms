namespace IP_KiprasRudzinskas
{
    internal class Program
    {
        static void Main(string[] args)
        {
            const string xlsxpath = "IP_places_data_2026.xlsx";
            const int startNr = 67;
            const int buses = 2;

            var places = DataLoader.Load(xlsxpath);
            Console.WriteLine($"Uzkrauta vietu: {places.Count}, pradzia nr: {startNr}, ({places.First(p => p.Nr == startNr).Name})");

            // -------- 1 Dalis: Greedy ---------
            Console.WriteLine("\n=== 1 Dalis: Greedy (lokaliai geriausias) ===");
            var sw = System.Diagnostics.Stopwatch.StartNew();
            var greedy = Greedy.Solve(places, buses, startNr);
            sw.Stop();

            foreach (var r in greedy)
            {
                string pathStr = string.Join(" ", r.Path.Select(i => places[i].Nr));
                Console.WriteLine($"Autobusas {r.BusId + 1}: {pathStr}");
                Console.WriteLine($"Atstumas: {r.Cost:F2}");
            }
            Console.WriteLine($"Maksimalus marsrutas: {greedy.Max(r => r.Cost):F2}");
            Console.WriteLine($"Greedy laikas: {sw.Elapsed.TotalMilliseconds:F1} ms");

            Visualizer.Plot(places, greedy, "Greedy Sprendinys", "greedy.png");


            // -------- 2 Dalis: Branch and Bound - ieskome kiek vietu telpa per 10s ---------
            Console.WriteLine("\n=== 2 Dalis: Branch & Bound (optimalus) ===");
            for (int n = 4; n <= places.Count; n++)
            {
                var subset = places
                    .Where(p => p.Nr == startNr)
                    .Concat(places
                        .Where(p => p.Nr != startNr)
                        .OrderBy(p => Math.Sqrt(Math.Pow(p.X - places.First(x => x.Nr == startNr).X, 2) +
                                                Math.Pow(p.Y - places.First(x => x.Nr == startNr).Y, 2)))
                        .Take(n - 1))
                    .ToList();

                var swBB = System.Diagnostics.Stopwatch.StartNew();
                var (bbRoutes, timedOut) = BranchAndBound.Solve(subset, buses, startNr, 10.0);
                swBB.Stop();

                if (timedOut)
                {
                    Console.WriteLine($"N={n}: TIMEOUT (>{10}s) - optimalu sprendini galima rasti iki N={n-1}");
                    if (bbRoutes != null)
                    {
                        foreach (var r in bbRoutes)
                        {
                            string path = string.Join(" ", r.Path.Select(i => subset[i].Nr));
                            Console.WriteLine($"Autobusas {r.BusId + 1}: {path} | Atstumas: {r.Cost:F2}");
                        }
                        Visualizer.Plot(subset, bbRoutes, $"B&B sprendinys (N={n}, timeout)", "bnb.png");
                    }
                    break;
                }

                Console.WriteLine($"N={n}: {swBB.Elapsed.TotalSeconds:F4}s  Maks.atstumas: {bbRoutes!.Max(r => r.Cost):F2}");
                foreach (var r in bbRoutes)
                {
                    string path = string.Join(" ", r.Path.Select(i => subset[i].Nr));
                    Console.WriteLine($"  Autobusiukas {r.BusId + 1}: {path} | Atstumas: {r.Cost:F2}");
                }
                Visualizer.Plot(subset, bbRoutes, $"B&B sprendinys (N={n})", $"bnb_n{n}.png");
            }

            // -------- 3 Dalis: Simulated Annealing ---------
            Console.WriteLine("\n=== 3 Dalis: Simulated Annealing (60s) ===");
            var swSA = System.Diagnostics.Stopwatch.StartNew();
            var saRoutes = SimulatedAnnealing.Solve(places, buses, startNr, 60);
            swSA.Stop();

            foreach (var r in saRoutes)
            {
                string path = string.Join(" ", r.Path.Select(i => places[i].Nr));
                Console.WriteLine($"Autobusas {r.BusId + 1}: {path} | Atstumas: {r.Cost:F2}");
            }
            Console.WriteLine($"Maksimalus marsrutas: {saRoutes.Max(r => r.Cost):F2}");
            Console.WriteLine($"SA laikas: {swSA.Elapsed.TotalSeconds:F1}s");
            Visualizer.Plot(places, saRoutes, "Simulated Annealing", "sa.png");

            // -------- 4 Dalis: Parallel Simulated Annealing ---------
            Console.WriteLine("\n=== 4 Dalis: Parallel Simulated Annealing (60s) ===");
            Console.WriteLine($"CPU branduoliai: {Environment.ProcessorCount}");
            var swPSA = System.Diagnostics.Stopwatch.StartNew();
            var psaRoutes = ParallelSA.Solve(places, buses, startNr, 60);
            swPSA.Stop();

            foreach (var r in psaRoutes)
            {
                string path = string.Join(" ", r.Path.Select(i => places[i].Nr));
                Console.WriteLine($"Autobusas {r.BusId + 1}: {path} | Atstumas: {r.Cost:F2}");
            }
            Console.WriteLine($"Maksimalus marsrutas: {psaRoutes.Max(r => r.Cost):F2}");
            Console.WriteLine($"Parallel SA laikas: {swPSA.Elapsed.TotalSeconds:F1}s");
            Console.WriteLine($"Speedup: {swSA.Elapsed.TotalSeconds / swPSA.Elapsed.TotalSeconds:F2}x");
            Visualizer.Plot(places, psaRoutes, "Parallel Simulated Annealing", "psa.png");
        }
    }
}
