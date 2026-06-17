using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using ExcelDataReader;

namespace IP_KiprasRudzinskas
{
    public static class DataLoader
    {
        public static List<Place> Load(string path)
        {
            System.Text.Encoding.RegisterProvider(
                System.Text.CodePagesEncodingProvider.Instance);

            var places = new List<Place>();
            using var stream = File.Open(path, FileMode.Open, FileAccess.Read);
            using var reader = ExcelReaderFactory.CreateReader(stream);
            var ds = reader.AsDataSet();
            var table = ds.Tables[0];

            for (int r = 4; r < table.Rows.Count; r++)
            {
                var row = table.Rows[r];
                if (row[1] == DBNull.Value) continue;
                places.Add(new Place
                {
                    Nr = Convert.ToInt32(row[1]),
                    Name = row[2]?.ToString() ?? "",
                    X = Convert.ToDouble(row[4]),
                    Y = Convert.ToDouble(row[5])
                });
            }
            return places;
        }
    }
}
