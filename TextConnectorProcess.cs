using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tournament.Library.Models;

namespace Tournament.Library.Data
{
    public static class TextConnectorProcess
    {
        public static string FullFilePath(this string path) // textFile.csv
        {
            return $"{ConfigurationManager.AppSettings["filePath"] }\\{path}";
        }
        //nacist textovy soubor
        public static List<string> LoadFile(this string file)
        {
            if (!File.Exists(file))
            {
                return new List<string>();
            }
           return File.ReadAllLines(file).ToList();
        }

        public static List<Team> ConvertToTeams(this List<string> lines,string peopleFile)
        {
            var output = new List<Team>();
            var people = peopleFile.FullFilePath().LoadFile().ConvertToPerson();

            foreach (var line in lines)
            {
                string[] cols = line.Split(',');
                var teams = new Team()
                {
                    Id = Convert.ToInt32(cols[0]),
                    TeamName = cols[1],
                    TeamMembers = ConvertToTeamMembers(cols[1])
                };
                string[] personId = cols[2].Split('|');

                foreach (var id in personId)
                {
                    teams.TeamMembers.Add(people.First(x => x.Id == int.Parse(id)));
                }
                output.Add(teams);

            }
            return output;
        }

        private static List<Person> ConvertToTeamMembers(string file)
        {
            return null;
        }
        //prevede vsechny string lines na prize hodnoty
        public static List<Prize> ConvertToPrize(this List<string> lines)
        {
            var output = new List<Prize>();

            foreach (var line in lines)
            {
                string[] cols = line.Split(',');
                var prize = new Prize
                {
                    Id = Convert.ToInt32(cols[0]),
                    PlaceNumber = Convert.ToInt32(cols[1]),
                    PlaceName = cols[2],
                    PrizeAmount = Convert.ToDecimal(cols[3]),
                    PrizePercentage = Convert.ToDouble(cols[4])
                };
                output.Add(prize);
            }
            return output;
        }

        public static List<Person> ConvertToPerson(this List<string> lines)
        {
            var output = new List<Person>();

            foreach (var line in lines)
            {
                string[] cols = line.Split(',');
                Person p = new Person
                {
                    Id = int.Parse(cols[0]),
                    FirstName = cols[1],
                    LastName = cols[2],
                    EmailAddress = cols[3],
                    CellPhoneNumber = cols[4]
                };

                output.Add(p);

            }
            return output;
        }

        public static List<Tournaments> ConvertToTournaments(
            this List<string> lines,string teamFileName, string peopleFileName,string prizeFileName)
        {
            var output = new List<Tournaments>();
            var teams = teamFileName.FullFilePath().LoadFile().ConvertToTeams(peopleFileName);
            var prizes = prizeFileName.FullFilePath().LoadFile().ConvertToPrize();

            foreach (var line in lines)
            {
                string[] cols = line.Split(',');
                Tournaments t = new Tournaments();
                t.Id = int.Parse(cols[0]);
                t.TournamentName = cols[1];
                t.EntryFee = decimal.Parse(cols[2]);
                string[] teamIds = cols[3].Split('|');

                foreach (var id in teamIds)
                {
                   t.EnteredTeams.Add(teams.First(x => x.Id == int.Parse(id)));
                }
                string[] prizeId = cols[4].Split('|');

                foreach (var id in prizeId)
                {
                    t.Prizes.Add(prizes.First(x => x.Id == int.Parse(id)));
                }
                //TODO - zachytit informace z kol

               output.Add(t);
            }
            return output;

        }

        private static string ConvertTeamListToString(List<Team> teams)
        {
            StringBuilder output = new StringBuilder();

            if (teams.Count == 0)
            {
                return string.Empty;
            }

            foreach (var person in teams)
            {
                output.Append($"{person.Id}|");
            }
            output = output.Insert(0, output.Length - 1);
            return output.ToString();
        }

        private static string ConvertPeopleListToString(List<Person> people)
        {
            StringBuilder output = new StringBuilder();

            if (people.Count == 0)
            {
                return string.Empty;
            }

            foreach (var person in people)
            {
                output.Append($"{person.Id}|") ;
            }
            output = output.Insert(0, output.Length - 1);
            return output.ToString();
        }

        private static string ConvertPrizeListToString(List<Prize> prizes)
        {
            StringBuilder output = new StringBuilder();
            if (prizes.Count == 0)
            {
                return "";
            }
            foreach (var prize in prizes)
            {
                output.Append($"{prize.Id}|");
            }
            output.Insert(0, output.Length - 1);
            return output.ToString();

        }

        private static string ConvertRoundListToString(List<List<Matchup>> rounds)
        {
            StringBuilder output = new StringBuilder();
            if (rounds.Count == 0)
            {
                return "";
            }
            foreach (List<Matchup> round in rounds)
            {
                output.Append($"{ConvertMatchupListToString(round)}|");
            }
            output.Insert(0, output.Length - 1);
            return output.ToString();
        }

        private static string ConvertMatchupListToString(List<Matchup> matchups)
        {
            StringBuilder output = new StringBuilder();
            if (matchups.Count == 0)
            {
                return "";
            }
            foreach (Matchup matchup in matchups)
            {
                output.Append($"{matchup.Id}^");
            }
            output.Insert(0, output.Length - 1);
            return output.ToString();
        }

        public static void SaveToTournamentFile(this List<Tournaments> tournaments,string fileName)
        {
            List<string> lines = new List<string>();
            foreach (var t in tournaments)
            {
                lines.Add($@"{t.Id}," +
                          $"{t.TournamentName}," +
                          $"{t.EntryFee}," +
                          $"{ConvertTeamListToString(t.EnteredTeams)}," +
                          $"{ConvertPrizeListToString(t.Prizes)},{ConvertRoundListToString(t.Rounds)}");
            }
            File.WriteAllLines(fileName,lines);
        }

        public static void SaveToTeamFile(this List<Team> teams, string file)
        {
            var lines = new List<string>();

            foreach (var team in teams)
            {
                lines.Add($"{team.Id},{team.TeamName},{ConvertPeopleListToString(team.TeamMembers)}");
            }
            File.WriteAllLines(file.FullFilePath(), lines);
        }
        public static void SaveToPrizeFile(this List<Prize> models, string file)
        {
            var lines = new List<string>();

            foreach (var model in models)
            {
                lines.Add($"{model.Id},{model.PlaceNumber},{model.PlaceName},{model.PrizeAmount},{model.PrizePercentage}");
            }

            File.WriteAllLines(file.FullFilePath(),lines);
        }

        public static void SaveToPersonFile(this List<Person> persons, string file)
        {
            var lines = new List<string>();

            foreach (var person in persons)
            {
                lines.Add($"{person.Id},{person.FirstName},{person.LastName},{person.EmailAddress},{person.CellPhoneNumber}");
            }
            File.WriteAllLines(file.FullFilePath(), lines);
        }
    }
}
