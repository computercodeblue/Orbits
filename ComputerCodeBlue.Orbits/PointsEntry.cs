using CsvHelper.Configuration.Attributes;

namespace ComputerCodeBlue.Orbits
{
    public class PointsEntry
    {
        [Name("Name")]
        public string Name { get; set; }

        [Name("Number")]
        public string Number { get; set; }

        [Name("Points")]
        public int Points { get; set; }

        [Name("Class")]
        public string Class { get; set; }
    }
}
