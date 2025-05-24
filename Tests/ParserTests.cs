using Parser.Models;
using Parser.Exceptions;

namespace Tests
{
    [TestFixture]
    public class ParserTests
    {

        [Test]
        public void ShouldParsesCorrectly_SingleTeamMultipleActivities()
        {
            string input = "GOPR|Akcja ratunkowa w górach|08:00|180|Szkolenie z u¿yciem helikoptera|12:15|90|Oznaczanie niebezpiecznych tras|15:00|60";

            var teams = InputParser.Parse(input);

            Assert.IsTrue(teams.ContainsKey("GOPR"), "Team GOPR should exist");

            var schedule = teams["GOPR"];
            Assert.That(schedule.Count, Is.EqualTo(3));

            AssertActivity(schedule, "Akcja ratunkowa w górach", new TimeOnly(8, 0), 180);
            AssertActivity(schedule, "Szkolenie z u¿yciem helikoptera", new TimeOnly(12, 15), 90);
            AssertActivity(schedule, "Oznaczanie niebezpiecznych tras", new TimeOnly(15, 0), 60);
        }

        [Test]
        public void ShouldParsesCorrectly_MultipleTeams_ParsesCorrectly()
        {
            string input =
                "GOPR|Akcja ratunkowa w górach|08:00|180|Szkolenie z u¿yciem helikoptera|12:15|90|Oznaczanie niebezpiecznych tras|15:00|60\n" +
                "TOPR|Akcja ratunkowa w górach|06:00|250|Szkolenie z u¿yciem helikoptera|10:30|90";

            var teams = InputParser.Parse(input);

            Assert.IsTrue(teams.ContainsKey("GOPR"));
            Assert.IsTrue(teams.ContainsKey("TOPR"));

            var goprSchedule = teams["GOPR"];
            Assert.That( goprSchedule.Count, Is.EqualTo(3));

            var toprSchedule = teams["TOPR"];
            Assert.That( toprSchedule.Count, Is.EqualTo(2));

            AssertActivity(toprSchedule, "Akcja ratunkowa w górach", new TimeOnly(6, 0), 250);
            AssertActivity(toprSchedule, "Szkolenie z u¿yciem helikoptera", new TimeOnly(10, 30), 90);
        }

        [Test]
        public void ShouldParsesCorrectly_ReturnsEmptyDictionaryForEmptyString()
        {
            var teams = InputParser.Parse("");
            Assert.IsNotNull(teams);
            Assert.That(teams.Count, Is.EqualTo( 0));
        }

        private void AssertActivity(Dictionary<string, TimeSlot> schedule, string activityName, TimeOnly start, int duration)
        {
            Assert.IsTrue(schedule.ContainsKey(activityName), $"Activity '{activityName}' should exist.");
            var activity = schedule[activityName];
            Assert.That(start, Is.EqualTo(activity.Start));
            Assert.That(duration,Is.EqualTo( activity.Duration));
        }
    }
}