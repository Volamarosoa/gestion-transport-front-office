namespace GestionTransport.FrontOffice.Models.Utils
{
    public class CsvStructure
    {
        public List<string> Columns { get; set; } = new();
        public int RowCount { get; set; }
        public List<Dictionary<string, string>> SampleData { get; set; } = new();
    }
}