using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TradeFinder.Models;
using TradeFinder.PlayerPool;

namespace TradeFinder.Models
{
    public class Trade
    {
        public Player MyPlayers { get; set; }
        public Player TheirPlayers { get; set; }
        public decimal MyStarterDifferential { get; set; }
        public decimal MyBenchDifferential { get; set; }
        public decimal TheirStarterDifferential { get; set; }
        public decimal TheirBenchDifferential { get; set; }
        public decimal Fairness { get; set; }

        public Trade()
        {

        }

        public Trade(Player myPlayer, Player theirPlayer)
        {
            MyPlayers = myPlayer;
            TheirPlayers = theirPlayer;
            Fairness = myPlayer.TradeValue - theirPlayer.TradeValue;
        }

        public void CalculateDifferentials(TeamPlayerPool myTeamPlayerPool, TeamPlayerPool theirTeamPlayerPool)
        {
            decimal myStarterDifferential = 0, myBenchDifferential = 0, theirStarterDifferential = 0, theirBenchDifferential = 0;

            myTeamPlayerPool.CalculateDifferentials(TheirPlayers, MyPlayers, ref myStarterDifferential, ref myBenchDifferential);
            theirTeamPlayerPool.CalculateDifferentials(MyPlayers, TheirPlayers, ref theirStarterDifferential, ref theirBenchDifferential);

            MyStarterDifferential = myStarterDifferential;
            MyBenchDifferential = myBenchDifferential;
            TheirStarterDifferential = theirStarterDifferential;
            TheirBenchDifferential = theirBenchDifferential;
        }
    }
}