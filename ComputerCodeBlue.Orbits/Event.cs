using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ComputerCodeBlue.Orbits
{
    public class Event
    {
        public string Name { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public List<Group> Groups { get; set; }
        public List<Competitor> Competitors { get; set; }
        public List<PointsEntry> Points { get; set; }
        public int KartCount
        {
            get
            {
                return Groups.SelectMany(g => g.Competitors).Count();
            }
        }

        public Event(string name, DateTime startDate, DateTime endDate, string raceDayFormat, List<List<Class>> raceOrderList, 
            List<Competitor> competitorList, List<PointsEntry> points, List<ClubTransponder> transponderList, List<Format> runFormats)
        {
            Name = name;
            StartDate = startDate;
            EndDate = endDate;
            Competitors = competitorList;
            Groups = new List<Group>();
            Points = points;

            int groupCount = 1;

            foreach (List<Class> group in raceOrderList)
            {
                string className = string.Empty;
                List<ClassAssignment> currentGroupCompetitors = new List<ClassAssignment>();

                if (group.Count > 0)
                {
                    foreach (Class rc in group)
                    {
                        if (string.IsNullOrWhiteSpace(className))
                            className += rc.Name + " " + rc.Weight;
                        else
                            className += "/" + rc.Name + " " + rc.Weight;
                    }

                    List<RunCompetitor> classCompetitorList = new List<RunCompetitor>();

                    foreach (Class rc in group)
                    {
                        int assignmentsCount = rc.Assignments.Count;

                        foreach (ClassAssignment a in rc.Assignments)
                        {
                            string firstName = System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(a.FirstName.ToLower());
                            string lastName = System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(a.LastName.ToLower());

                            List<Competitor> compList = Competitors.Where(c => (c.Number == a.RaceNumber) && (c.FirstName == firstName) && (c.LastName == lastName) && (c.Class == rc.Name)).ToList();
                            int compCount = compList.Count();
                            RunCompetitor cc;

                            var pointsCompetitor = Points.Where(p => p.Class == rc.Name && p.Name.ToLower().Contains(firstName.ToLower()) && p.Name.ToLower().Contains(lastName.ToLower())).FirstOrDefault();

                            decimal competitorPoints = 0.00M;

                            if (pointsCompetitor != null)
                                competitorPoints = pointsCompetitor.Points;

                            if (compCount < 1)
                            {
                                string newDriverId = string.Empty;
                                string newCarId = string.Empty;

                                do
                                {
                                    newCarId = Competitor.GetIdNumber();
                                } while (Competitors.Where(c => c.CarRegistration == newCarId).Count() != 0);

                                do
                                {
                                    newDriverId = Competitor.GetIdNumber();
                                } while (Competitors.Where(c => c.DriverRegistration == newDriverId).Count() != 0);

                                cc = new RunCompetitor
                                {
                                    Number = a.RaceNumber,
                                    ClassName = rc.Name,
                                    FirstName = firstName,
                                    LastName = lastName,
                                    CarRegistration = newCarId,
                                    DriverRegistration = newDriverId,
                                    Transponder = a.Transponder,
                                    Points = competitorPoints
                                };
                            }
                            else if (compCount == 1)
                            {
                                var orbitsCompetitor = compList.FirstOrDefault();

                                cc = new RunCompetitor
                                {
                                    Number = orbitsCompetitor.Number,
                                    DriverRegistration = orbitsCompetitor.DriverRegistration,
                                    CarRegistration = orbitsCompetitor.CarRegistration,
                                    ClassName = rc.Name,
                                    FirstName = orbitsCompetitor.FirstName,
                                    LastName = orbitsCompetitor.LastName,
                                    Hidden = false,
                                    Points = competitorPoints,
                                    StartPosition = 0,
                                    Transponder = a.Transponder
                                };
                            }
                            else // Remove duplicates from competitor file
                            {
                                var orbitsCompetitor = compList.FirstOrDefault();

                                cc = new RunCompetitor
                                {
                                    Number = orbitsCompetitor.Number,
                                    DriverRegistration = orbitsCompetitor.DriverRegistration,
                                    CarRegistration = orbitsCompetitor.CarRegistration,
                                    ClassName = rc.Name,
                                    FirstName = orbitsCompetitor.FirstName,
                                    LastName = orbitsCompetitor.LastName,
                                    Hidden = false,
                                    Points = competitorPoints,
                                    StartPosition = 0,
                                    Transponder = a.Transponder
                                };

                                for (int i = 1; i < compCount; i++)
                                {
                                    Competitors.Remove(compList[i]);
                                }

                            }

                            string transponderNumber = a.Transponder;

                            if (string.IsNullOrWhiteSpace(a.Transponder))
                            {
                                var rental = transponderList.Where(t => (t.Number == cc.Transponder)).ToList();
                                if (rental.Count == 0)
                                    transponderNumber = cc.Transponder; // Don't import the rental transponders
                            }
                            else
                            {
                                if (a.Transponder.Length == 3)
                                {
                                    a.Transponder = transponderList.Where(t => t.ClubNumber == a.Transponder).FirstOrDefault().Number;
                                }
                                transponderNumber = a.Transponder;
                            }

                            classCompetitorList.Add(new RunCompetitor
                            {
                                Number = cc.Number,
                                ClassName = rc.Name,
                                FirstName = cc.FirstName,
                                LastName = cc.LastName,
                                CarRegistration = cc.CarRegistration,
                                DriverRegistration = cc.DriverRegistration,
                                Transponder = transponderNumber,
                                Hidden = cc.Hidden,
                                Points = cc.Points,
                                StartPosition = cc.StartPosition
                            });
                        }
                    }
                    Console.WriteLine(className + " " + classCompetitorList.Count.ToString());
                    Groups.Add(new Group(className, startDate, raceDayFormat, groupCount, runFormats, classCompetitorList, points));
                    groupCount++;
                }
            }
        }

        public override string ToString()
        {
            StringBuilder resultStringBuilder = new StringBuilder();

            resultStringBuilder.AppendLine("<groups>");

            foreach (var group in Groups)
            {
                resultStringBuilder.Append(group.ToString());
            }

            resultStringBuilder.AppendLine("</groups>");

            return resultStringBuilder.ToString();
        }

        public string ToHtmlString(int copies = 1)
        {
            StringBuilder result = new StringBuilder();

            result.AppendLine("<!DOCTYPE html>");
            result.AppendLine("<html>");
            result.AppendLine("<head>");
            result.AppendLine("<meta charset=\"utf-8\">");
            result.AppendLine("<meta name=\"viewport\" content=\"width=device-width, initial-scale=1\">");
            result.AppendLine("<title>Badger Kart Club</title>");
            result.AppendLine("<script src=\"https://kit.fontawesome.com/4ede001748.js\" crossorigin=\"anonymous\"></script>");
            result.AppendLine("<style>");
            result.AppendLine("@page { size: auto; margin: 0.5in 0.5in 0.5in 0.5in; }");
            result.AppendLine("body { font-family: BlinkMacSystemFont,-apple-system,\"Segoe UI\",Roboto,Oxygen,Ubuntu,Cantarell,\"Fira Sans\",\"Droid Sans\",\"Helvetica Neue\",Helvetica,Arial,sans-serif; font-size: 11pt; }");
            result.AppendLine("h1 { page-break-before: always; margin-bottom: 0; font-size: 16pt; }");
            result.AppendLine("h2 { margin-bottom: 0; margin-top: 0; font-size: 14pt; }");
            result.AppendLine("h3 { font-size: 12pt; }");
            result.AppendLine(".flex-container { display: flex; }");
            result.AppendLine(".left-column { flex: 70 %; max-width: 70%; }");
            result.AppendLine(".right-column { flex: 30%; }");
            result.AppendLine(".starting-grid-container { display: flex; flex-wrap: wrap; }");
            result.AppendLine(".driver { flex: 40%; border-style: solid; border-bottom-style: none; margin: 0.5em; display: flex; flex-direction: column; }");
            result.AppendLine(".driver-position { font-weight: bold; font-size: 14pt; flex: 1; text-align: center; }");
            result.AppendLine(".driver-name { font-size: 12pt; flex: 1; text-align: center; }");
            result.AppendLine(".driver-points { flex: 1; text-align: center; }");
            result.AppendLine(".driver-points-container { display: flex; flex-wrap: wrap; margin-top: 0.5em; font-size: 10pt; }");
            result.AppendLine(".driver-points-name { flex: 3; text-align: left; min-width: 75% }");
            result.AppendLine(".driver-points-points { flex: 1; text-align: right; }");
            result.AppendLine("</style>");
            result.AppendLine("</head>");
            result.AppendLine("<body>");

            var runs = Groups.SelectMany(g => g.Runs).OrderBy(r => r.DateTime);

            int lastHour = runs.FirstOrDefault()?.DateTime.Hour ?? 0;

            StringBuilder runGroupStringBuilder = new StringBuilder();

            foreach (var run in runs)
            {
                if (run.DateTime.Hour != lastHour)
                {
                    for (int i = 1; i <= copies; i++)
                        result.Append(runGroupStringBuilder.ToString());

                    runGroupStringBuilder.Clear();
                }
                lastHour = run.DateTime.Hour;
                runGroupStringBuilder.Append(run.ToHtmlString());
            }

            result.AppendLine("</body>");
            result.AppendLine("</html>");

            return result.ToString();
        }
    }
}
