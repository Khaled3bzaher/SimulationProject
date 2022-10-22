using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiQueueModels
{
    public class SimulationSystem
    {
        public SimulationSystem()
        {
            this.Servers = new List<Server>();
            this.InterarrivalDistribution = new List<TimeDistribution>();
            this.PerformanceMeasures = new PerformanceMeasures();
            this.SimulationTable = new List<SimulationCase>();
        }
        //Calculate Cumulative Probability and Range for each Row in Interarrival Distribution
        public void calculateCummProb_Ranges_InterarrivalDistribution()
        {
            decimal sumCProb = 0;
            foreach (TimeDistribution timeRow in this.InterarrivalDistribution)
            {
                timeRow.MinRange = (int)(sumCProb * 100) + 1;
                sumCProb += timeRow.Probability;
                timeRow.CummProbability = sumCProb;
                timeRow.MaxRange = (int)(sumCProb * 100);
            }
        }
        //Calculate Cumulative Probability and Range for each Row in Servers
        public void calculateCummProb_Ranges_Servers()
        {
            foreach(Server outerServer in this.Servers) { 
                decimal sumCProb = 0;
                foreach (TimeDistribution timeRow in outerServer.TimeDistribution)
                {
                    timeRow.MinRange = (int)(sumCProb * 100) + 1;
                    sumCProb += timeRow.Probability;
                    timeRow.CummProbability = sumCProb;
                    timeRow.MaxRange = (int)(sumCProb * 100);
                }
            }
        }
        //Read From File
        public void readFromFile(string filePath, SimulationSystem simSystem)
        {
            // Read From Test Case File
            FileStream fileStream = new FileStream(filePath, FileMode.Open);
            StreamReader reader = new StreamReader(fileStream);
            while (reader.Peek() != -1)
            {
                string lineRead = reader.ReadLine();
                if (lineRead == "NumberOfServers")
                {
                    simSystem.NumberOfServers = int.Parse(reader.ReadLine());
                }
                else if (lineRead == "")
                    continue;
                else if (lineRead == "StoppingNumber")
                {
                    simSystem.StoppingNumber = int.Parse(reader.ReadLine());
                }
                else if (lineRead == "StoppingCriteria")
                {
                    int stopCri = int.Parse(reader.ReadLine());
                    switch (stopCri)
                    {
                        case 1:
                            simSystem.StoppingCriteria = Enums.StoppingCriteria.NumberOfCustomers;
                            break;
                        case 2:
                            simSystem.StoppingCriteria = Enums.StoppingCriteria.SimulationEndTime;
                            break;
                        default:
                            break;
                    }
                }
                else if (lineRead == "SelectionMethod")
                {
                    int selection = int.Parse(reader.ReadLine());
                    switch (selection)
                    {
                        case 1:
                            simSystem.SelectionMethod = Enums.SelectionMethod.HighestPriority;
                            break;
                        case 2:
                            simSystem.SelectionMethod = Enums.SelectionMethod.Random;
                            break;
                        default:
                            simSystem.SelectionMethod = Enums.SelectionMethod.LeastUtilization;
                            break;
                    }
                }
                else if (lineRead == "InterarrivalDistribution")
                {
                    while (true)
                    {
                        string intervalLine = reader.ReadLine();
                        if (intervalLine == "")
                        {
                            break;
                        }
                        TimeDistribution oneRaw = new TimeDistribution();
                        string[] intervalIntoTime_Prob = intervalLine.Split(',');
                        oneRaw.Time = int.Parse(intervalIntoTime_Prob[0]);
                        oneRaw.Probability = decimal.Parse(intervalIntoTime_Prob[1]);
                        simSystem.InterarrivalDistribution.Add(oneRaw);
                    }
                    //Calculate Cumulative Probability and Range for each Row in Interarrival Distribution
                    simSystem.calculateCummProb_Ranges_InterarrivalDistribution();
                }
                else if (lineRead.Contains("ServiceDistribution_Server"))
                {
                    for (int server = 0; server < simSystem.NumberOfServers; server++)
                    {
                        Server localServer = new Server();
                        while (true)
                        {
                            string nextLine = reader.ReadLine();
                            if (nextLine == null || nextLine == "")
                            {
                                reader.ReadLine();
                                break;
                            }
                            else
                            {
                                TimeDistribution oneRaw = new TimeDistribution();
                                string[] serverDistrubation = nextLine.Split(',');
                                oneRaw.Time = int.Parse(serverDistrubation[0]);
                                oneRaw.Probability = decimal.Parse(serverDistrubation[1]);
                                localServer.TimeDistribution.Add(oneRaw);
                            }
                        }
                        localServer.ID = server + 1;
                        simSystem.Servers.Add(localServer);
                    }
                    //Calculate Cumulative Probability and Range for each Row in Servers
                    simSystem.calculateCummProb_Ranges_Servers();
                }
            }
            fileStream.Close();
        }
        ///////////// INPUTS ///////////// 
        public int NumberOfServers { get; set; }
        public int StoppingNumber { get; set; }
        public List<Server> Servers { get; set; }
        public List<TimeDistribution> InterarrivalDistribution { get; set; }
        public Enums.StoppingCriteria StoppingCriteria { get; set; }
        public Enums.SelectionMethod SelectionMethod { get; set; }

        ///////////// OUTPUTS /////////////
        public List<SimulationCase> SimulationTable { get; set; }
        public PerformanceMeasures PerformanceMeasures { get; set; }

    }
}
