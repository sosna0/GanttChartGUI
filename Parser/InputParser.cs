using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Parser.Exceptions;

namespace Parser.Models
{

    public record TimeSlot(TimeOnly Start, int Duration);
    public class ScheduleMap : Dictionary<string, TimeSlot> { }
    public class TeamsMap : Dictionary<string, ScheduleMap> { }

    public static class InputParser {

        
        public static TeamsMap Parse(string input, char part_sep='|')
        {
            var schedule = new TeamsMap();

            string[] lines = input.Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

            int lineNumber = 0;

            foreach (string line in lines)
            {
                lineNumber++;
                string[] parts = line.Split(part_sep);
                if (string.IsNullOrWhiteSpace(parts[0]))
                {
                    throw new ParsingException($"Pusta linia lub linia bez nazwy zespołu w linii {lineNumber}.");
                }

                string teamName = parts[0];
                var activities = new ScheduleMap();
                
                // Pierwsza część to zawsze nazwa, więcj tutaj brakuje aktywności
                if (parts.Length < 2)
                {
                    throw new NoActivitiesException(teamName);
                }

                // Sprawdzamy duplikaty nazwy zespołu
                if (schedule.ContainsKey(teamName))
                {
                    throw new DuplicateTeamNameException(teamName);
                }

                for (int i = 1; i < parts.Length; i += 3)
                {
                    if (i + 2 >= parts.Length)
                    {
                        throw new InvalidActivityDataException($"Niekompletne dane aktywności w linii {lineNumber} dla zespołu '{teamName}'. Oczekiwano 3 elementów (nazwa, początek, trwanie), znaleziono mniej.");
                    }

                    string activityName = parts[i];
                    TimeOnly startTime;
                    int durationInMinutes;

                    // Sprawdzamy duplikaty aktywności
                    if (activities.ContainsKey(activityName))
                    {
                        throw new DuplicateActivityException(teamName, activityName);
                    }

                    // Parsowaniu czasu rozpoczęcia
                    try
                    {
                        startTime = TimeOnly.Parse(parts[i + 1]);
                    }
                    catch (Exception)
                    {
                        throw new InvalidActivityDataException($"Błąd parsowania czasu rozpoczęcia dla aktywności '{activityName}' w zespole '{teamName}' w linii {lineNumber}. Wartość: '{parts[i + 1]}'.");
                    }

                    // Parsowaniu czasu trwania
                    try
                    {
                        durationInMinutes = int.Parse(parts[i + 2]);
                    }
                    catch (Exception)
                    {
                        throw new InvalidActivityDataException($"Błąd parsowania czasu trwania dla aktywności '{activityName}' w zespole '{teamName}' w linii {lineNumber}. Wartość: '{parts[i + 2]}'.");
                    }
                    activities.Add(activityName, new TimeSlot(startTime, durationInMinutes));
                }
                schedule.Add(teamName, activities);
            }
            return schedule;

        }
    }
}
