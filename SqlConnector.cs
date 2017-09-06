using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Tournament.Library.Models;

namespace Tournament.Library.Data
{
    public class SqlConnector : IDataConnection
    {
        private const string Value = "Test";

      
        public Person CreatePerson(Person person)
        {
            using (IDbConnection connect = new SqlConnection(GlobalConfiguration.CnnValue(Value)))
            {
                var p = new DynamicParameters();
                p.Add("@firstName", person.FirstName);
                p.Add("@lastName", person.LastName);
                p.Add("@emailAddress",person.EmailAddress);
                p.Add("@cellPhoneNumber", person.CellPhoneNumber);
                p.Add("@id", 0, DbType.Int32,ParameterDirection.Output);

                connect.Execute("dbo.spPeople_Insert", p, commandType:CommandType.StoredProcedure);
                person.Id = p.Get<int>("@id");

                return person;
            }
        }
            
        //TODO- Vytvorit metodu CreatePrize a ulozit do database
        /// <summary>
        /// ulozi novou cenu do database
        /// </summary>
        /// <param name="model">Informace o cene </param>
        /// <returns>Informace o cene plus unikatni identifikaci</returns>
        public Prize CreatePrize(Prize model)
        {
            using (IDbConnection connection = new SqlConnection(GlobalConfiguration.CnnValue(Value)))
            {
               var p = new DynamicParameters();
                p.Add("@PlaceNumber", model.PlaceNumber);
                p.Add("@PlaceName",model.PlaceName);
                p.Add("@PrizeAmount",model.PrizeAmount);
                p.Add("@PrizePersentage",model.PrizePercentage);
                p.Add("@id", 0, dbType:DbType.Int32, direction:ParameterDirection.Output);

                connection.Execute("dbo.spPrizes_Insert", p, commandType: CommandType.StoredProcedure);

                model.Id = p.Get<int>("@id");

               return model;
            }
           
        }

        public Team CreateTeam(Team team)
        {
            using (IDbConnection connection = new SqlConnection(GlobalConfiguration.CnnValue(Value)))
            {
                var p = new DynamicParameters();
                p.Add("@PlaceNumber", team.TeamName);
             
                p.Add("@id", 0, dbType: DbType.Int32, direction: ParameterDirection.Output);

                connection.Execute("dbo.spTeams_Insert_Insert", p, commandType: CommandType.StoredProcedure);

                team.Id = p.Get<int>("@id");

                foreach (var tm in team.TeamMembers)
                {
                    p = new DynamicParameters();
                    p.Add("@teamId", team.Id);
                    p.Add("@teamMemberId", tm.Id);

                    connection.Execute("dbo.spTeamMembers_Insert", p, commandType: CommandType.StoredProcedure);
                }
                return team;
            }
        }

        public void CreateTournament(Tournaments t)
        {
            using (IDbConnection connection = new SqlConnection(GlobalConfiguration.CnnValue(Value)))
            {
               SaveTournament(connection,t);

               SaveTournamnetPrizes(connection,t);

                SaveEnteredTeams(connection,t);
            }
        }

        private void SaveTournament(IDbConnection con, Tournaments t)
        {
            var p = new DynamicParameters();
            p.Add("@tournamentName", t.TournamentName);
            p.Add("@entryFee", t.EntryFee);
            p.Add("@id", 0, dbType: DbType.Int32, direction: ParameterDirection.Output);
            con.Execute("dbo.spTournaments_Insert", p, commandType: CommandType.StoredProcedure);
            t.Id = p.Get<int>("@id");
        }

        private void SaveTournamnetPrizes(IDbConnection con, Tournaments t)
        {
            foreach (var prize in t.Prizes)
            {
                var p = new DynamicParameters();
                p.Add("@tournamentId", t.Id);
                p.Add("@entryFee", prize.Id);
                p.Add("@id", 0, dbType: DbType.Int32, direction: ParameterDirection.Output);
                con.Execute("dbo.spTournamentPrizes_Insert", p, commandType: CommandType.StoredProcedure);
            }
        }

        private void SaveEnteredTeams(IDbConnection con, Tournaments t)
        {
            foreach (var tm in t.EnteredTeams)
            {
                var p = new DynamicParameters();
                p.Add("@tournamentId", t.Id);
                p.Add("@teamId", tm.Id);
                p.Add("@id", 0, dbType: DbType.Int32, direction: ParameterDirection.Output);
                con.Execute("dbo.spTuornamentEntries_Insert", p, commandType: CommandType.StoredProcedure);
            }
        }

        public List<Person> GetPerson_All()
        {
            List<Person> output;
            using (IDbConnection connection = new SqlConnection(GlobalConfiguration.CnnValue(Value)))
            {
                output = connection.Query<Person>("dbo.spPerson_GetAll").ToList();
            }
            return output;
        }

        public List<Team> GetTeam_All()
        {
            List<Team> output;
            using (IDbConnection connect = new SqlConnection(GlobalConfiguration.CnnValue(Value)))
            {
                output = connect.Query<Team>("dbo.spTeam_GetAll").ToList();

                foreach (var team in output)
                {
                    var p = new DynamicParameters();
                    p.Add("@teamId",team.Id);
                    p.Add("@teamName",team.TeamName);
                    team.TeamMembers = connect.Query<Person>("dbo.spTeamMembers_GetByTeam").ToList();
                }
            }
            return output;
        }
    }
}
