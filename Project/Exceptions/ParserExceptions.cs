using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Exceptions
{
    public class ParsingException : Exception
    {
        public ParsingException(string message) : base(message) { }
        public ParsingException(string message, Exception innerException) : base(message, innerException) { }
    }

    public class InvalidActivityDataException : ParsingException
    {
        public InvalidActivityDataException(string message) : base(message) { }
    }

    public class DuplicateActivityException : ParsingException
    {
        public DuplicateActivityException(string teamName, string activityName)
            : base($"Aktywność '{activityName}' w zespole '{teamName}' jest zduplikowana.") { }
    }

    public class DuplicateTeamNameException : ParsingException
    {
        public DuplicateTeamNameException(string teamName)
            : base($"Nazwa zespołu: '{teamName}' jest zduplikowana.") { }
    }

    public class NoActivitiesException : ParsingException
    {
        public NoActivitiesException(string teamName)
            : base($"Zespół '{teamName}' nie ma zdefiniowanych aktywności.") { }
    }
}
