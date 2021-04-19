using System;
using CsvHelper.Configuration.Attributes;

namespace ComputerCodeBlue.Orbits
{
    public class Competitor
    {
        [Name("No")]
        public string Number { get; set; }
        [Name("Class")]
        public string Class { get; set; }
        [Name("FirstName")]
        public string FirstName { get; set; }
        [Name("LastName")]
        public string LastName { get; set; }
        [Name("CarRegistration")]
        public string CarRegistration { get; set; }
        [Name("DriverRegistration")]
        public string DriverRegistration { get; set; }
        [Name("Transponder1")]
        public string Transponder1 { get; set; }
        [Name("Transponder2")]
        public string Transponder2 { get; set; }
        [Name("Additional1")]
        public string NatState { get; set; }
        [Name("Additional2")]
        public string Sponsor { get; set; }
        [Name("Additional3")]
        public string Make { get; set; }
        [Name("Additional4")]
        public string Hometown { get; set; }
        [Name("Additional5")]
        public string Club { get; set; }
        [Name("Additional6")]
        public string ModelEngine { get; set; }
        [Name("Additional7")]
        public string Tires { get; set; }
        [Name("Additional8")]
        public string Email { get; set; }

        public static string GetIdNumber()
        {
            byte[] buffer = Guid.NewGuid().ToByteArray();
            var IdNumber = BitConverter.ToUInt32(buffer, 0) ^ BitConverter.ToUInt32(buffer, 4) ^ BitConverter.ToUInt32(buffer, 8) ^ BitConverter.ToUInt32(buffer, 12);
            return IdNumber.ToString("X").ToLower();
        }
    }
}
