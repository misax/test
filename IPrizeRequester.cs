using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tournament.Library.Models;

namespace Tournament.Library.Data
{
    public interface IPrizeRequester
    {
        void PrizeComplete(Prize prize);
    }
}
