using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScottPlot;

namespace IP_KiprasRudzinskas
{
    public static class Visualizer
    {
        public static void Plot(List<Place> places, List<Route> routes, string title, string outputPath)
        {
            var plt = new Plot(800, 600);

            string[] colors = { "#e74c3c", "#2ecc71", "#3498db", "#f39c12" };

            foreach (var route in routes)
            {
                double[] xs = route.Path.Select(i => places[i].X).ToArray();
                double[] ys = route.Path.Select(i => places[i].Y).ToArray();

                var color = System.Drawing.ColorTranslator.FromHtml(colors[route.BusId % colors.Length]);
                plt.AddScatterLines(xs, ys, color, lineWidth: 1.5f, label: $"Autobusiukas {route.BusId + 1}");
            }

            // Vietos taškai
            double[] allX = places.Select(p => p.X).ToArray();
            double[] allY = places.Select(p => p.Y).ToArray();
            plt.AddScatterPoints(allX, allY, System.Drawing.Color.Black, markerSize: 4);

            // Numeriai
            foreach (var p in places)
                plt.AddText(p.Nr.ToString(), p.X, p.Y, size: 8, color: System.Drawing.Color.Black);

            plt.Title(title);
            plt.XLabel("X");
            plt.YLabel("Y");
            plt.Legend();
            plt.SaveFig(outputPath);

            Console.WriteLine($"Grafikas išsaugotas: {outputPath}");
        }
    }
}
