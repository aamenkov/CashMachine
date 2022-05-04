using System.Collections.Concurrent;

namespace CashMachineWebApp.Models
{
    public class DTOServerResponse
    {
        public string Message { get;  set; }
        public ConcurrentDictionary<int, int> Dictionary { get; private set; }
        public string RunTime { get; set; }

        public DTOServerResponse(string message, string runTime, ConcurrentDictionary<int, int> dictionary)
        {
            Message = message;
            RunTime = runTime;
            Dictionary = dictionary;
        }
    }
}
