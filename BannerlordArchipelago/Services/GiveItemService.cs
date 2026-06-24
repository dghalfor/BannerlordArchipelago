using Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem.Roster;

namespace BannerlordArchipelago.Services
{
    public class GiveItemService
    {
        public GiveItemService()
        {
            ItemRoster archipelagoItems = new ItemRoster();
            InventoryScreenHelper.OpenScreenAsStash(archipelagoItems);
        }
    }
}
