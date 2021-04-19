using System;
using CsvHelper.Configuration.Attributes;

namespace ComputerCodeBlue.Orbits
{
    public class Format
    {
        [Name("Format Code")]
        public string FormatCode { get; set; }

        [Name("Class")]
        public string Class { get; set; }

        [Name("Order")]
        public int Order { get; set; }

        [Name("Run Name")]
        public string RunName { get; set; }

        [Name("Run Type")]
        public string RunType { get; set; }

        [Name("Auto Finish Type")]
        public string AutoFinishType { get; set; }

        [Name("Laps")]
        public int Laps { get; set; }

        [Name("Time")]
        public TimeSpan Time { get; set; }

        [Name("Grid")]
        public string Grid { get; set; }

        [Name("Base Grid On")]
        public string BaseGridOn { get; set; }
    }
}
