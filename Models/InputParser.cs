using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;

namespace Project.Models {
    internal static class InputParser {


        // to domyślnie będzie wczytywane na wejściu
        public static List<string> input = [
            "pogotownie, pierwsza pomoc, 10:30",
            "policja, zabezpieczanie terenu, 10:35",
            "strażacy1, gaszenie pożaru, 10:00",
            "strażacy1, wyważanie drzwi, 10:25",
            "strażacy1, wyniesienie poszkodowanych z pożaru, 10:35"
        ];

        public static List<Team> teams = [];
        public static Dictionary<string, Dictionary<string, TimeOnly>> TeamWithActivity = [];

        // dopisać funkcję wczytującą dane z pliku do listy
        public static void Parse(List<string> input) {
            foreach (var item in input) {
                var tasks = item.Split(", ");
                string team_name = tasks[0];
                string activity_name = tasks[1];
                var start_time = TimeOnly.Parse(tasks[2]);

                if (TeamWithActivity.ContainsKey(team_name)) {
                    //Console.WriteLine("contains");
                    TeamWithActivity[team_name][activity_name] = start_time;
                }
                else {
                    //Console.WriteLine("doesn't contain");
                    TeamWithActivity[team_name] = new Dictionary<string, TimeOnly>();
                    TeamWithActivity[team_name][activity_name] = start_time;
                }
            }

            foreach (var item in TeamWithActivity) {
                var team = new Team(item.Key, item.Value);
                teams.Add(team);
                team.ShowSchedule();
            }


        }


    }
}
