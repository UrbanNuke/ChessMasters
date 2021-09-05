using System.Collections.Generic;
using Misc;

namespace Gameplay
{
    public class HistoryService
    {
        public List<HistoryEl> History { get; private set; }
        
        public HistoryService()
        {
            History = new List<HistoryEl>();
        }

        public void Add(HistoryEl historyEl) => History.Add(historyEl);
    }
}