using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vampire
{

    [Serializable]
    public class Config
    {
        public int Heal_level_per
        {
            get;
            set;
        }

        public int Heal_kill_per
        {
            get;
            set;
        }

        public int Heal_kill_max
        {
            get;
            set;
        }

        public int Heal_skill_kill_max
        {
            get;
            set;
        }

        public int Heal_skill_passive_max
        {
            get;
            set;
        }

        public int Heal_maxhp
        {
            get;
            set;
        }

        public float Start_delay
        {
            get;
            set;
        }

        public float Injury_delay
        {
            get;
            set;
        }

        public float Heal_delay
        {
            get;
            set;
        }

        public bool Display_info
        {
            get;
            set;
        }

        public bool All_areas
        {
            get;
            set;
        }

        public bool Slow_heal_mode
        {
            get;
            set;
        }

        public int Slow_heal_sec
        {
            get;
            set;
        }

        public string debug_mess
        {
            get;
            set;
        } = "true";

        private static Config _instance;

        public static Config Instance
        {
            get
            {
                return _instance != null ? _instance : new Config
                {
                    Heal_level_per = 10,
                    Heal_kill_per = 1,
                    Heal_kill_max = 10,
                    Heal_skill_kill_max = 10,
                    Heal_skill_passive_max = 5,
                    Heal_maxhp = 100,
                    Start_delay = 30f,
                    Injury_delay = 15f,
                    Heal_delay = 5f,
                    Display_info = true,
                    All_areas = true,
                    Slow_heal_mode = false,
                    Slow_heal_sec = 10,
                }; ;
            }
        }
    }

}
