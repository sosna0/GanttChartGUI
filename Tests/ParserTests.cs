using Parser.Models;
using Parser.Exceptions;

namespace Tests
{
    [TestFixture]
    public class ParserTests
    {
        [Test]
        public void ShouldParsesCorrectly_SingleTeamSingleActivity()
        {
            string input = "TEAM_A|task1|12:00|60";

            var teams = InputParser.Parse(input);

            Assert.IsTrue(teams.ContainsKey("TEAM_A"), "Team TEAM_A should exist");

            var schedule = teams["TEAM_A"];
            Assert.That(schedule.Count, Is.EqualTo(1));

            AssertActivity(schedule, "task1", new TimeOnly(12, 0), 60);
        }

        [Test]
        public void ShouldParsesCorrectly_SingleTeamMultipleActivities()
        {
            string input = "TEAM_A|task1|08:00|180|task2|12:15|90|task3|15:00|60";

            var teams = InputParser.Parse(input);

            Assert.IsTrue(teams.ContainsKey("TEAM_A"), "Team TEAM_A should exist");

            var schedule = teams["TEAM_A"];
            Assert.That(schedule.Count, Is.EqualTo(3));

            AssertActivity(schedule, "task1", new TimeOnly(8, 0), 180);
            AssertActivity(schedule, "task2", new TimeOnly(12, 15), 90);
            AssertActivity(schedule, "task3", new TimeOnly(15, 0), 60);
        }

        [Test]
        public void ShouldParsesCorrectly_MultipleTeams_ParsesCorrectly()
        {
            string input =
                "TEAM_A|task1|08:00|180|task2|12:15|90|task3|15:00|60\n" +
                "TEAM_B|task1|06:00|250|task2|10:30|90";

            var teams = InputParser.Parse(input);

            Assert.IsTrue(teams.ContainsKey("TEAM_A"));
            Assert.IsTrue(teams.ContainsKey("TEAM_B"));

            var aSchedule = teams["TEAM_A"];
            Assert.That(aSchedule.Count, Is.EqualTo(3));

            var bSchedule = teams["TEAM_B"];
            Assert.That(bSchedule.Count, Is.EqualTo(2));

            AssertActivity(aSchedule, "task1", new TimeOnly(8, 0), 180);
            AssertActivity(aSchedule, "task2", new TimeOnly(12, 15), 90);
            AssertActivity(aSchedule, "task3", new TimeOnly(15, 0), 60);

            AssertActivity(bSchedule, "task1", new TimeOnly(6, 0), 250);
            AssertActivity(bSchedule, "task2", new TimeOnly(10, 30), 90);
        }

        [Test]
        public void ShouldParsesCorrectly_ReturnsEmptyDictionaryForEmptyString()
        {
            var teams = InputParser.Parse("");
            Assert.IsNotNull(teams);
            Assert.That(teams.Count, Is.EqualTo( 0));
        }

        [Test]
        public void ShouldThrowParsingException_OnEmptyLine()
        {
            string input = "Team1|Activity|08:00|30\n" +
                            " \n" +
                            "Team2|Activity2|09:00|45";

            var ex = Assert.Throws<ParsingException>(() => InputParser.Parse(input));
            StringAssert.Contains("Pusta linia", ex.Message);
        }

        [Test]
        public void ShouldThrowNoActivitiesException_WhenLineHasNoActivities()
        {
            string input = "TeamWithoutActivities";

            var ex = Assert.Throws<NoActivitiesException>(() => InputParser.Parse(input));
            StringAssert.Contains("Zespó³ 'TeamWithoutActivities' nie ma zdefiniowanych aktywnoœci", ex.Message);
        }

        [Test]
        public void ShouldThrowDuplicateTeamNameException_WhenTeamNamesAreDuplicated()
        {
            string input =
                "Team1|Activity|08:00|30\n" +
                "Team1|Activity2|09:00|45";

            var ex = Assert.Throws<DuplicateTeamNameException>(() => InputParser.Parse(input));
            StringAssert.Contains("Nazwa zespo³u: 'Team1' jest zduplikowana", ex.Message);
        }

        [Test]
        public void ShouldThrowInvalidActivityDataException_WhenActivityDataIsIncomplete()
        {
            string input = "Team1|Activity1|08:00"; // Missing duration

            var ex = Assert.Throws<InvalidActivityDataException>(() => InputParser.Parse(input));
            StringAssert.Contains("Niekompletne dane aktywnoœci", ex.Message);
        }

        [Test]
        public void ShouldThrowDuplicateActivityException_WhenActivityNamesAreDuplicated()
        {
            string input = "Team1|Activity1|08:00|30|Activity1|09:00|45";

            var ex = Assert.Throws<DuplicateActivityException>(() => InputParser.Parse(input));
            StringAssert.Contains("Aktywnoœæ 'Activity1' w zespole 'Team1' jest zduplikowana", ex.Message);
        }

        [Test]
        public void ShouldThrowInvalidActivityDataException_WhenStartTimeIsInvalid()
        {
            string input = "Team1|Activity1|invalid-time|30";

            var ex = Assert.Throws<InvalidActivityDataException>(() => InputParser.Parse(input));
            StringAssert.Contains("B³¹d parsowania czasu rozpoczêcia", ex.Message);
        }

        [Test]
        public void ShouldThrowInvalidActivityDataException_WhenDurationIsInvalid()
        {
            string input = "Team1|Activity1|08:00|invalid-duration";

            var ex = Assert.Throws<InvalidActivityDataException>(() => InputParser.Parse(input));
            StringAssert.Contains("B³¹d parsowania czasu trwania", ex.Message);
        }

        private void AssertActivity(Dictionary<string, TimeSlot> schedule, string activityName, TimeOnly start, int duration)
        {
            Assert.IsTrue(schedule.ContainsKey(activityName), $"Activity '{activityName}' should exist.");
            var activity = schedule[activityName];
            Assert.That(start, Is.EqualTo(activity.Start));
            Assert.That(duration,Is.EqualTo( activity.Duration));
        }

        [Test]
        public void ShouldThrowNoActivitiesException_WhenScheduleIsEmpty()
        {
            var teams = new TeamsMap
            {
                { "TeamA", new ScheduleMap() }
            };

            Assert.Throws<NoActivitiesException>(() => InputParser.Validate(teams));
        }

        [Test]
        public void ShouldThrowOverlappingActivitiesException_WhenActivitiesOverlap()
        {
            var overlappingSchedule = new ScheduleMap
            {
                { "Activity1", new TimeSlot(new TimeOnly(9, 0), 60) },
                { "Activity2", new TimeSlot(new TimeOnly(9, 30), 30) }
            };

            var teams = new TeamsMap
            {
                { "TeamB", overlappingSchedule }
            };

            Assert.Throws<OverlappingActivitiesException>(() => InputParser.Validate(teams));
        }

        [Test]
        public void ShouldNotThrow_WhenActivitiesDoNotOverlap()
        {
            var nonOverlappingSchedule = new ScheduleMap
            {
                { "Activity1", new TimeSlot(new TimeOnly(9, 0), 60) },
                { "Activity2", new TimeSlot(new TimeOnly(10, 0), 30) }
            };

            var teams = new TeamsMap
            {
                { "TeamC", nonOverlappingSchedule }
            };

            try
            {
                InputParser.Validate(teams);
            }
            catch (OverlappingActivitiesException)
            {
                Assert.Fail("OverlappingActivitiesException should not have been thrown.");
            }
        }
    }
}