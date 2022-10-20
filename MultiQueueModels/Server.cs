using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiQueueModels
{
    public class Server
    {
        public Server()
        {
            this.TimeDistribution = new List<TimeDistribution>();
            decimal sumCProb = 0;
           foreach(TimeDistribution timeServer in this.TimeDistribution)
            {
                timeServer.MinRange = (int)(sumCProb * 100) + 1;
                sumCProb += timeServer.Probability;
                timeServer.CummProbability = sumCProb;
                timeServer.MaxRange = (int)(sumCProb * 100);
            }
        }

        public int ID { get; set; }
        public decimal IdleProbability { get; set; }
        public decimal AverageServiceTime { get; set; } 
        public decimal Utilization { get; set; }

        public List<TimeDistribution> TimeDistribution;
        

        //optional if needed use them
        public int FinishTime { get; set; }
        public int TotalWorkingTime { get; set; }
    }
}
