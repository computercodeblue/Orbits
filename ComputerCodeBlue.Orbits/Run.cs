using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ComputerCodeBlue.Orbits
{
    public class Run
    {
        public string GroupName { get; set; }
        public string Name { get; set; }
        public string ShortName { get; set; }
        public DateTime DateTime { get; set; }
        public string RunType { get; set; }
        public string StartMethod { get; set; }
        public string AutoFinishMethod { get; set; }
        public int AutoFinishLaps { get; set; }
        public TimeSpan AutoFinishTime { get; set; }
        public string QualificationType { get; set; }
        public string QualificationValue { get; set; }
        public bool IncludeCompetitors { get; set; }
        private RunGridType OrbitsRunGridType { get; set; }
        public List<RunCompetitor> Competitors { get; set; }
        public List<PointsEntry> PointsList { get; set; }

        public void SortCompetitorsByNumber()
        {
            var competitorsByNumber = Competitors.OrderBy(c => c.Number).ToList();

            for (int i = 1; i <= competitorsByNumber.Count; i++)
            {
                competitorsByNumber[i-1].StartPosition = i;
            }

            OrbitsRunGridType = RunGridType.Number;

            Competitors = competitorsByNumber;
        }

        public void SortCompetitorsByPoints()
        {
            var competitorsByPoints = Competitors.OrderByDescending(c => c.Points).ToList();

            for (int i = 1; i <= competitorsByPoints.Count; i++)
            {
                competitorsByPoints[i-1].StartPosition = i;
            }

            OrbitsRunGridType = RunGridType.Points;

            Competitors = competitorsByPoints;
        }

        public void RandomizeCompetitors()
        {
            var rnd = new Random();

            var randomCompetitors = Competitors.Select(x => new { value = x, order = rnd.Next() }).OrderBy(x => x.order).Select(x => x.value).ToList();

            for (int i = 1; i <= randomCompetitors.Count; i++)
            {
                randomCompetitors[i-1].StartPosition = i;
            }

            OrbitsRunGridType = RunGridType.Random;

            Competitors = randomCompetitors;
        }

        public void InvertCompetitors(IEnumerable<RunCompetitor> competitors)
        {
            var unsortedCompetitors = new List<RunCompetitor>();

            foreach (var competitor in competitors)
            {
                unsortedCompetitors.Add(new RunCompetitor
                {
                    CarRegistration = competitor.CarRegistration,
                    ClassName = competitor.ClassName,
                    DriverRegistration = competitor.DriverRegistration,
                    FirstName = competitor.FirstName,
                    Hidden = competitor.Hidden,
                    LastName = competitor.LastName,
                    Number = competitor.Number,
                    Points = competitor.Points,
                    StartPosition = competitor.StartPosition,
                    Transponder = competitor.Transponder,
                });
            }

            var sortedCompetitors = unsortedCompetitors.OrderByDescending(c => c.StartPosition).ToList();
            var result = new List<RunCompetitor>(sortedCompetitors.Count);

            int currentPosition = 1;

            foreach (var c in sortedCompetitors)
            {
                c.StartPosition = currentPosition;
                result.Add(c);
                currentPosition++;
            }

            OrbitsRunGridType = RunGridType.Invert;

            Competitors = result;
        }

        public string ToString(bool includeCompetitors)
        {
            string raceTimeString = DateTime.ToString("HH:mm:ss");
            string raceDateString = DateTime.ToString("yyyy-MM-dd");

            StringBuilder resultStringBuilder = new StringBuilder();

            resultStringBuilder.Append("<run name=\"" + Name + "\" shortname=\"" + ShortName + "\" date=\"" + raceDateString + "\" time=\"" + raceTimeString +
                "\" type=\"" + RunType + "\" startmethod=\"" + StartMethod + "\" countfirst=\"none\" autofinishmethod=\"" + AutoFinishMethod + "\" ");

            if (AutoFinishMethod == RunAutoFinishMethod.Laps)
                resultStringBuilder.Append("autofinishlaps=\"" + AutoFinishLaps.ToString() + "\" ");

            if (AutoFinishMethod == RunAutoFinishMethod.Time)
                resultStringBuilder.Append("autofinishtime=\"" + AutoFinishTime.ToString("h:mm:ss.fff") + "\" "); 
            
            resultStringBuilder.AppendLine("qualificationtype=\"" + (string.IsNullOrWhiteSpace(QualificationType) ? "none" : QualificationType) + 
                "\" qualificationvalue=\"" + QualificationValue + "\">");

            resultStringBuilder.AppendLine("<competitors>");

            if (includeCompetitors)
            {
                foreach(var competitor in Competitors)
                {
                    resultStringBuilder.AppendLine(competitor.ToString(true));
                }
            }

            resultStringBuilder.AppendLine("</competitors>");

            resultStringBuilder.AppendLine("</run>");

            return resultStringBuilder.ToString();
        }

        public override string ToString()
        {
            return ToString(IncludeCompetitors);
        }

        public string ToHtmlString()
        {
            var classes = Competitors.Select(c => c.ClassName).Distinct().ToList();
            StringBuilder result = new StringBuilder();

            if (!IncludeCompetitors || Competitors.Count < 1 || Name.ToLower().Contains("practice")) return string.Empty;

            result.AppendLine("<h1>" + GroupName + "</h1>");
            result.AppendLine("<div class=\"flex-container\">");
            result.AppendLine("<div class=\"left-column\">");
            result.AppendLine("<h2>" + Name + "</h2>");
            result.AppendLine("<div class=\"starting-grid-container\">");

            foreach (string className in classes)
            {
                if (classes.Count > 1) result.AppendLine("<h3>" + className + "</h3>");
                result.AppendLine("<div class=\"starting-grid-container\">");
                int currentPosition = 1;
                for (int i = 0; i < Competitors.Count; i++)
                {
                    if (Competitors[i].ClassName == className)
                    {
                        result.AppendLine("<div class=\"driver\">");
                        result.AppendLine("<div class=\"driver-position\">" + currentPosition.ToString() + "</div>");
                        result.AppendLine("<div class=\"driver-name\">" + Competitors[i].Number + " " + Competitors[i].FirstName + " " + Competitors[i].LastName + "</div>");
                        if (OrbitsRunGridType == RunGridType.Points)
                        {
                            result.AppendLine("<div class=\"driver-points\">" + Competitors[i].Points.ToString() + " pts.</div>");
                        }
                        result.AppendLine("</div>");
                        currentPosition++;
                    }

                }
                if (currentPosition % 2 != 1)
                {
                    result.AppendLine("<div class=\"driver\">&nbsp;</div>");
                }
                result.AppendLine("</div>");
            }

            result.AppendLine("</div>");
            result.AppendLine("<div class=\"right-column\">");
            result.AppendLine("<h2>Points</h2>");

            foreach (string className in classes)
            {
                if (classes.Count > 1)
                {
                    result.AppendLine("<h3>" + className + "</h3>");
                }
                result.AppendLine("<div class=\"driver-points-container\">");
                var seasonPoints = PointsList.Where(pl => pl.Class == className).OrderByDescending(pl => pl.Points);
                foreach (PointsEntry entry in seasonPoints)
                {
                    result.AppendLine("<div class=\"driver-points-name\">" + entry.Number.ToString() + " " + entry.Name + "</div>");
                    result.AppendLine("<div class=\"driver-points-points\">" + entry.Points.ToString() + "</div>");
                }
                result.AppendLine("</div>");
            }

            result.AppendLine("</div>");
            result.AppendLine("</div>");

            return result.ToString();
        }
    }
}
