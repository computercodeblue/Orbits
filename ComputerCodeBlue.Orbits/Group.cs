using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ComputerCodeBlue.Orbits
{
    public class Group
    {
        public string Name { get; set; }
        public List<RunCompetitor> Competitors { get; set; }
        public List<Run> Runs { get; set; }
        public List<PointsEntry> PointsList { get; set; }
        public Group(string name, DateTime raceDate, string raceDayFormat, int groupOrder, IEnumerable<Format> runFormats, IEnumerable<RunCompetitor> competitors, IEnumerable<PointsEntry> pointsList)
        {
            Name = name;
            Competitors = new List<RunCompetitor>();
            Runs = new List<Run>();
            PointsList = new List<PointsEntry>();

            foreach (var competitor in competitors)
                Competitors.Add(new RunCompetitor
                {
                    CarRegistration = competitor.CarRegistration,
                    ClassName = competitor.ClassName,
                    DriverRegistration = competitor.DriverRegistration,
                    FirstName = competitor.FirstName,
                    LastName = competitor.LastName,
                    Hidden = competitor.Hidden,
                    Number = competitor.Number,
                    Points = competitor.Points,
                    StartPosition = competitor.StartPosition,
                    Transponder = competitor.Transponder
                });

            PointsList = pointsList.ToList();

            List<Format> orderedFormats = runFormats.Where(rf => Name.Contains(rf.Class) && (rf.FormatCode == raceDayFormat || rf.FormatCode == "A")).OrderBy(rf => rf.Order).ToList();

            if (orderedFormats.Count < 1)
                orderedFormats = runFormats.Where(rf => rf.Class == "All" && (rf.FormatCode == raceDayFormat || rf.FormatCode == "A")).OrderBy(rf => rf.Order).ToList();

            foreach (var format in orderedFormats)
            {
                var run = new Run
                {
                    GroupName = Name,
                    Name = format.RunName,
                    RunType = format.RunType,
                    AutoFinishMethod = format.AutoFinishType.ToLower(),
                    DateTime = new DateTime(raceDate.Year, raceDate.Month, raceDate.Day, 4 + format.Order, (groupOrder - 1) * 2, 0, 0),
                    AutoFinishLaps = format.Laps,
                    AutoFinishTime = format.Time,
                    ShortName = string.Empty,
                    StartMethod = RunStartMethod.FirstPassing,
                    QualificationType = RunQualificationType.None,
                    QualificationValue = string.Empty,
                    Competitors = new List<RunCompetitor>(),
                    PointsList = PointsList,
                    IncludeCompetitors = false
                };

                if (format.Grid != GridType.None)
                {
                    foreach (var competitor in Competitors)
                        run.Competitors.Add(new RunCompetitor
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

                    switch (format.Grid.ToLower())
                    {
                        case GridType.Random:
                            run.IncludeCompetitors = true;
                            run.RandomizeCompetitors();
                            break;
                        case GridType.Invert:
                            run.IncludeCompetitors = true;
                            var oldRun = Runs.Where(r => r.Name.ToLower() == format.BaseGridOn.ToLower()).FirstOrDefault();

                            if (oldRun != null)
                            {
                                run.InvertCompetitors(oldRun.Competitors);
                            }
                            break;
                        case GridType.OrderBy:
                            if (format.BaseGridOn.ToLower() == "number")
                                run.SortCompetitorsByNumber();
                            else if (format.BaseGridOn.ToLower() == "points")
                                run.SortCompetitorsByPoints();
                            run.IncludeCompetitors = true;
                            break;
                        default:
                            run.IncludeCompetitors = false;
                            break;
                    }
                }

                Runs.Add(run);
            }

        }
        public override string ToString()
        {
            StringBuilder resultStringBuilder = new StringBuilder();

            resultStringBuilder.AppendLine("<group name=\"" + Name + "\">");

            resultStringBuilder.AppendLine("<competitors>");

            foreach (var competitor in Competitors)
            {
                resultStringBuilder.AppendLine(competitor.ToString());
            }

            resultStringBuilder.AppendLine("</competitors>");

            if (Runs.Count > 0)
            {
                resultStringBuilder.AppendLine("<runs>");

                foreach (var run in Runs)
                {
                    resultStringBuilder.Append(run.ToString(true));
                }

                resultStringBuilder.AppendLine("</runs>");
            }

            resultStringBuilder.AppendLine("</group>");

            return resultStringBuilder.ToString();
        }

        public string ToHtmlString()
        {
            StringBuilder result = new StringBuilder();

            foreach (var run in Runs)
            {
                result.Append(run.ToHtmlString());
            }

            return result.ToString();
        }
    }
}
