using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Models {
    internal class Team {
        public string Name { get; set; }

        public static Dictionary<string, int> Activity = new() {
            ["gaszenie pożaru"] = 30,
            ["pierwsza pomoc"] = 20,
            ["zabezpieczanie terenu"] = 10,
            ["wyważanie drzwi"] = 2,
            ["wyniesienie poszkodowanych z pożaru"] = 5
        };

        public Dictionary<string, (TimeOnly, TimeOnly)> ActivitySchedule { get; set; }

        public Team(string Name, Dictionary<string, TimeOnly> ActivityWithDate) {
            this.Name = Name;
            ActivitySchedule = [];

            foreach (var pair in ActivityWithDate) {
                string name = pair.Key;
                TimeOnly start_time = pair.Value;
                TimeOnly end_time = start_time.AddMinutes(Activity[name]);
                ActivitySchedule.Add(name, (start_time, end_time));
            }
        }

        public void ShowSchedule() {
            Console.WriteLine("-----" + this.Name + "-----");

            foreach (var pair in ActivitySchedule) {
                string name = pair.Key;
                TimeOnly start_time = pair.Value.Item1;
                TimeOnly end_time = pair.Value.Item2;
                Console.WriteLine(name + " " + start_time + " " + end_time);
            }
            Console.WriteLine("");
        }

    }
}
