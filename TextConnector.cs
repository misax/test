using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tournament.Library.Models;
using Tournament.Library.Data;

namespace Tournament.Library.Data
{              
    public class TextConnector : IDataConnection
    {
        private const string PrizeFile = "prizeFile.csv";
        private const string PeopleFile = "personFile.csv";
        private const string TeamFile = "teamFile.csv";
        private const string TournamentFile = "tornamentFile.csv";

        public Person CreatePerson(Person p)
        {
            var people = PeopleFile.FullFilePath().LoadFile().ConvertToPerson();

            int currId = 1;
            if (people.Count > 0)
            {
                 currId = people.OrderByDescending(x => x.Id == p.Id).First().Id + 1; 
            }
          
            p.Id = currId;

            people.Add(p);

            people.SaveToPersonFile(PeopleFile);
            return p;

        }


        //spojit Metodu CreatePrize s text souborem
        public Prize CreatePrize(Prize model)
        {
            //nacist textovy soubor
            //prevest text do List<Prize>
           var prizes = PrizeFile.FullFilePath().LoadFile().ConvertToPrize();
           //najit max id
            int currId = 1;
            if (prizes.Count > 0)
            {
                  currId = prizes.OrderByDescending(x => x.Id == model.Id).First().Id + 1;
            }
          
            model.Id = currId;
             //pridat novy zaznam s novym id + 1
            prizes.Add(model);
            //prevest ceny do List<string>
            //ulozit list do text souboru
            prizes.SaveToPrizeFile(PrizeFile);
            
            return model;
            
        }

        public Team CreateTeam(Team team)
        {
            var teams = TeamFile.FullFilePath().LoadFile().ConvertToTeams(PeopleFile);
            int currId = 1;
            if (teams.Count > 0)
            {
                currId = teams.OrderByDescending(x => x.Id == team.Id).First().Id + 1;
            }

            team.Id = currId;
            teams.Add(team);

            teams.SaveToTeamFile(TeamFile);
            return team;
        }

        public void  CreateTournament(Tournaments t)
        {
            var tournaments = TournamentFile.
                FullFilePath()
                .LoadFile()
                . ConvertToTournaments(TeamFile,PeopleFile,PrizeFile);
            int currId = 1;
            if (tournaments.Count > 0)
            {
                currId = tournaments.OrderByDescending(x => x.Id).First().Id + 1;
            }
            t.Id = currId;
            tournaments.Add(t);
            tournaments.SaveToTournamentFile(TournamentFile);

        }

        public List<Person> GetPerson_All()
        {
            return PeopleFile.FullFilePath().LoadFile().ConvertToPerson();
        }

        public List<Team> GetTeam_All()
        {
            return TeamFile.FullFilePath().LoadFile().ConvertToTeams(PeopleFile);
        }
    }
}
