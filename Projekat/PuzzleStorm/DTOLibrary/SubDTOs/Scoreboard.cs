using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOLibrary.SubDTOs
{
    public class Scoreboard
    {
        public List<Tuple<Player, int>> Scores { get; set; }
    }
}
