namespace ComputerCodeBlue.Orbits
{
    public class RunCompetitor
    {
        public string Number { get; set; }
        public string ClassName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string CarRegistration { get; set; }
        public string DriverRegistration { get; set; }
        public decimal Points { get; set; }
        public int StartPosition { get; set; }
        public bool Hidden { get; set; }
        public string Transponder { get; set; }

        public RunCompetitor()
        {

        }

        public RunCompetitor(Competitor orbitsCompetitor, decimal points)
        {
            Number = orbitsCompetitor.Number;
            ClassName = orbitsCompetitor.Class;
            FirstName = orbitsCompetitor.FirstName;
            LastName = orbitsCompetitor.LastName;
            CarRegistration = orbitsCompetitor.CarRegistration;
            DriverRegistration = orbitsCompetitor.DriverRegistration;
            Points = points;
            StartPosition = 0;
            Hidden = false;
            Transponder = orbitsCompetitor.Transponder1;
        }

        public string ToString(bool includeTransponder)
        {
            return "<competitor no=\"" + Number + "\" class=\"" + ClassName + "\" firstname=\"" + FirstName + "\" lastname=\"" + LastName +
                "\" registration=\"" + CarRegistration + "\" driverregistration=\"" + DriverRegistration + "\" points=\"" + Points.ToString() +
                "\" startpos=\"" + StartPosition.ToString() + "\" hidden=\"" + (Hidden ? "yes" : "no") + "\"" +
                (includeTransponder ? " transponders=\"" + Transponder + "\"/>" : "/>");
        }

        public override string ToString()
        {
            return ToString(false);
        }

        public string ToHtmlString()
        {
            return "<div class=\"column is-size- 4\">" + Number + " " + FirstName + " " + LastName + "</div>";
        }

    }
}
