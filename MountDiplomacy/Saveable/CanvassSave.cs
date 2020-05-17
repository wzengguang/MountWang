using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.SaveSystem;

namespace Wang.Saveable
{
    public class CanvassSave
    {

        [SaveableField(1)]
        private int _hero;

        [SaveableField(2)]
        private int _clan;

        [SaveableProperty(3)]
        public float DayTime { get; set; }

        [SaveableProperty(4)]
        public float Bonus { get; set; }


        public Hero Hero
        {
            get
            {
                return Clan.PlayerClan.Companions.FirstOrDefault(a => a.Id.GetHashCode() == _hero);
            }
            set
            {
                _hero = value == null ? -1 : value.Id.GetHashCode();
            }
        }

        public int HeroId
        {
            get { return _hero; }
        }

        public int ClanId
        {
            get { return _clan; }
        }

        public bool IsCurrent()
        {
            return _hero != -1;
        }

        public Clan Clan
        {
            get
            {
                return Clan.All.FirstOrDefault(a => a.Id.GetHashCode() == _clan);
            }
            set
            {
                _clan = value == null ? -1 : value.Id.GetHashCode();
            }
        }


    }

}
