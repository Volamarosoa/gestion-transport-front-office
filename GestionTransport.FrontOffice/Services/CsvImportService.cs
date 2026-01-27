using System.Data;
using System.Text;

using GestionTransport.FrontOffice.Models.Utils;

namespace GestionTransport.FrontOffice.Services
{
    public class CsvImportService
    {
        // Lire le CSV et retourner un DataTable (comme un Excel en mémoire)
        public DataTable ReadCsvToDataTable(Stream fileStream, string delimiter = ";")
        {
            var dataTable = new DataTable();

            using (var reader = new StreamReader(fileStream, Encoding.UTF8))
            {
                // Lire les en-têtes (première ligne)
                var headerLine = reader.ReadLine();
                if (string.IsNullOrEmpty(headerLine))
                {
                    throw new Exception("Le fichier CSV est vide.");
                }

                var headers = headerLine.Split(delimiter);
                foreach (var header in headers)
                {
                    dataTable.Columns.Add(header.Trim());
                }

                // Lire toutes les lignes de données
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    if (string.IsNullOrWhiteSpace(line)) continue;

                    var values = line.Split(delimiter);
                    var row = dataTable.NewRow();

                    for (int i = 0; i < values.Length && i < headers.Length; i++)
                    {
                        row[i] = values[i].Trim();
                    }

                    dataTable.Rows.Add(row);
                }
            }

            return dataTable;
        }

        // Analyser la structure du CSV
        public CsvStructure AnalyzeCsv(Stream fileStream, string delimiter = ";")
        {
            fileStream.Position = 0; // Remettre au début du stream
            var dataTable = ReadCsvToDataTable(fileStream, delimiter);

            var structure = new CsvStructure
            {
                Columns = dataTable.Columns.Cast<DataColumn>().Select(c => c.ColumnName).ToList(),
                RowCount = dataTable.Rows.Count,
                SampleData = new List<Dictionary<string, string>>()
            };

            // Prendre les 5 premières lignes comme aperçu
            var sampleRows = dataTable.AsEnumerable().Take(5);
            foreach (var row in sampleRows)
            {
                var rowDict = new Dictionary<string, string>();
                foreach (DataColumn col in dataTable.Columns)
                {
                    rowDict[col.ColumnName] = row[col].ToString() ?? "";
                }
                structure.SampleData.Add(rowDict);
            }

            return structure;
        }

        // Générer un template d'exemple
        public byte[] GenerateExampleTemplate()
        {
            var csv = new StringBuilder();
            csv.AppendLine("NomEmploye;NomSite;Adresse;TypeTransport;Heure");
            csv.AppendLine("Jean Dupont;Siège Paris;15 Rue de la Paix, Paris;Aller;07:30");
            csv.AppendLine("Marie Martin;Usine Lyon;8 Avenue Foch, Lyon;Retour;18:00");
            csv.AppendLine("Pierre Bernard;Agence Marseille;22 Boulevard Longchamp;Aller;07:30");
            return Encoding.UTF8.GetBytes(csv.ToString());
        }
    }
}