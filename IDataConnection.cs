using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tournament.Library.Models;

namespace Tournament.Library.Data
{
    public interface IDataConnection
    {
        Prize CreatePrize(Prize model);
        Person CreatePerson(Person p);
        List<Person> GetPerson_All();
        Team CreateTeam(Team team);
        List<Team> GetTeam_All();
       void CreateTournament(Tournaments t);
    }
}
