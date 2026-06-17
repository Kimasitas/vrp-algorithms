namespace IP_KiprasRudzinskas
{
    public class Place
    {
        public int Nr { get; set; }
        public string Name { get; set; }
        public double X { get; set; }
        public double Y { get; set; }

        public static double Distance(Place a, Place b)
            => Math.Sqrt(Math.Pow(a.X - b.X, 2) + Math.Pow(a.Y - b.Y, 2));
    }
}