using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;

namespace BannerlordArchipelago.Hooks
{
    public static class HeroLevelUpHook
    {
        public static void ClearUnspentPoints(Hero hero)
        {
            var developer = hero.HeroDeveloper;
            developer.UnspentAttributePoints = 0;
            developer.UnspentFocusPoints = 0;
        }
    }
}
