using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MultiQueueModels;
using MultiQueueTesting;
using System.IO;
namespace MultiQueueSimulation
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        SimulationSystem simSystem = new SimulationSystem();
        public int calculateServiceForServer(int serverID,int RandomService)
        {
            foreach(TimeDistribution oneRow in simSystem.Servers[serverID].TimeDistribution)
            {
                if (RandomService >= oneRow.MinRange && RandomService <= oneRow.MaxRange)
                    return oneRow.Time;
            }
            return 0;
        }
        public int calculateArrivalTime(int RandomInterArrival)
        {
            foreach (TimeDistribution oneRow in simSystem.InterarrivalDistribution)
            {
                if (RandomInterArrival >= oneRow.MinRange && RandomInterArrival <= oneRow.MaxRange)
                    return oneRow.Time;
            }
            return 0;
        }
        public void calculateSystemPerformance()
        {
            int waitedCustomers = 0;
            int waitedTimeInQueue = 0;
            int maxInQueue = 0;
            foreach (SimulationCase oneRow in simSystem.SimulationTable)
            {
                waitedTimeInQueue += oneRow.TimeInQueue;
                if (oneRow.TimeInQueue > 0)
                {
                    waitedCustomers += 1;
                    if (oneRow.TimeInQueue > maxInQueue)
                        maxInQueue = oneRow.TimeInQueue;
                }
            }
            simSystem.PerformanceMeasures.AverageWaitingTime = (decimal)waitedTimeInQueue / simSystem.StoppingNumber;
            simSystem.PerformanceMeasures.WaitingProbability = (decimal)waitedCustomers / simSystem.StoppingNumber;
            simSystem.PerformanceMeasures.MaxQueueLength = maxInQueue;
        }
        public void calculateServersPerformace()
        {
            int finishTime = 0;
            foreach (Server localServer in simSystem.Servers)
            {
                if (finishTime < localServer.FinishTime)
                    finishTime = localServer.FinishTime;
            }
            //Avarage Services
            List<int> customersOfServer = new List<int>();
            foreach (Server localServer in simSystem.Servers)
                customersOfServer.Add(0);
            foreach (SimulationCase oneCase in simSystem.SimulationTable)
                customersOfServer[oneCase.AssignedServer.ID - 1] += 1;
            for (int server = 0; server < simSystem.NumberOfServers; server++)
            {
                if (customersOfServer[server] == 0)
                    simSystem.Servers[server].AverageServiceTime = 0;
                else
                    simSystem.Servers[server].AverageServiceTime = (decimal)simSystem.Servers[server].TotalWorkingTime / customersOfServer[server];
                
                simSystem.Servers[server].IdleProbability = (decimal)(finishTime - simSystem.Servers[server].TotalWorkingTime) / finishTime;
                simSystem.Servers[server].Utilization = (decimal)simSystem.Servers[server].TotalWorkingTime / finishTime;
            }

        }
        private void Form1_Load(object sender, EventArgs e)
        {
            simSystem.readFromFile("C:/Users/Computer Shop/Downloads/SimulationProject-Nada/MultiQueueSimulation/TestCases/TestCase1.txt", simSystem);
            //Starting System
            Random RandomInterArrival = new Random();
            Random RandomService = new Random();
            Random nextServer = new Random();
            if (simSystem.StoppingCriteria == Enums.StoppingCriteria.NumberOfCustomers)
            {
                int eachArrivalTime = 0;
                
                for (int customer = 0; customer < simSystem.StoppingNumber; customer++)
                {
                    
                    SimulationCase oneCase = new SimulationCase();
                    if (customer == 0)
                    {
                        oneCase.CustomerNumber = customer + 1;
                        oneCase.RandomInterArrival = 1;
                        oneCase.InterArrival = 0;
                        oneCase.ArrivalTime = 0;
                        oneCase.RandomService = RandomService.Next(1, 100);
                        //Check Type Of System
                        int serverID = 0;
                        if (simSystem.SelectionMethod == Enums.SelectionMethod.Random)
                        {
                            serverID = nextServer.Next(1, simSystem.NumberOfServers);
                        }
                        oneCase.ServiceTime = calculateServiceForServer(serverID, oneCase.RandomService);
                        oneCase.AssignedServer = simSystem.Servers[serverID];
                        oneCase.StartTime = 0;
                        oneCase.EndTime = oneCase.ServiceTime;
                        oneCase.TimeInQueue = 0;
                        simSystem.Servers[serverID].FinishTime = oneCase.EndTime;
                        simSystem.Servers[serverID].TotalWorkingTime += oneCase.ServiceTime;
                        simSystem.SimulationTable.Add(oneCase);
                    }
                    else
                    {
                        oneCase.CustomerNumber = customer + 1;
                        oneCase.RandomInterArrival = RandomInterArrival.Next(1,100);
                        oneCase.InterArrival = calculateArrivalTime(oneCase.RandomInterArrival);
                        eachArrivalTime += oneCase.InterArrival;
                        oneCase.ArrivalTime = eachArrivalTime;
                        //Check Type Of System
                        // Highest HighestPriority
                        int serverID = 0; // Default
                        if (simSystem.SelectionMethod == Enums.SelectionMethod.Random)
                        {
                            List<Server> emptyServers = new List<Server>();
                            foreach (Server localServer in simSystem.Servers)
                            {
                                if (oneCase.ArrivalTime >= localServer.FinishTime)
                                    emptyServers.Add(localServer);
                            }
                            if (emptyServers.Count != 0)
                            {
                                Random rndServers = new Random();
                                serverID = (emptyServers[rndServers.Next(0, emptyServers.Count)].ID) - 1;
                            }
                            else
                            {
                                int firstFinish = 9999999;
                                serverID = 0;
                                foreach (Server localServer in simSystem.Servers)
                                {
                                    if (firstFinish > localServer.FinishTime)
                                    {
                                        firstFinish = localServer.FinishTime;
                                        serverID = localServer.ID - 1;
                                    }
                                }
                            }
                        }
                        else if (simSystem.SelectionMethod == Enums.SelectionMethod.HighestPriority)
                        {
                            int firstFinish = 10000;
                            serverID = 0;
                            foreach (Server localServer in simSystem.Servers)
                            {
                                if (localServer.FinishTime <= oneCase.ArrivalTime)
                                {
                                    serverID = localServer.ID - 1;
                                    break;
                                }
                                else if (firstFinish > localServer.FinishTime)
                                {
                                    firstFinish = localServer.FinishTime;
                                    serverID = localServer.ID - 1;
                                }
                            }
                            //return serverID;
                        }
                       
                        oneCase.RandomService = RandomService.Next(1, 100);
                        oneCase.ServiceTime = calculateServiceForServer(serverID, oneCase.RandomService);
                        oneCase.StartTime = Math.Max(oneCase.ArrivalTime, simSystem.Servers[serverID].FinishTime);
                        oneCase.EndTime = oneCase.StartTime + oneCase.ServiceTime;
                        oneCase.TimeInQueue = oneCase.StartTime - oneCase.ArrivalTime;
                        oneCase.AssignedServer = simSystem.Servers[serverID];
                        simSystem.Servers[serverID].FinishTime = oneCase.EndTime;
                        simSystem.Servers[serverID].TotalWorkingTime += oneCase.ServiceTime;
                        simSystem.SimulationTable.Add(oneCase);
                    }
                }
            }///
            else if(simSystem.StoppingCriteria == Enums.StoppingCriteria.SimulationEndTime)
            {
                
                int eachArrivalTime = 0;

                for (int customer = 0; customer < simSystem.StoppingNumber; customer++)
                {

                    SimulationCase oneCase = new SimulationCase();
                    if (customer == 0)
                    {
                        oneCase.CustomerNumber = customer + 1;
                        oneCase.RandomInterArrival = 1;
                        oneCase.InterArrival = 0;
                        oneCase.ArrivalTime = 0;
                        oneCase.RandomService = RandomService.Next(1, 100);
                        //Check Type Of System
                        int serverID = 0;
                        if (simSystem.SelectionMethod == Enums.SelectionMethod.Random)
                        {
                            serverID = nextServer.Next(1, simSystem.NumberOfServers);
                        }
                        oneCase.ServiceTime = calculateServiceForServer(serverID, oneCase.RandomService);
                        oneCase.AssignedServer = simSystem.Servers[serverID];
                        oneCase.StartTime = 0;
                        oneCase.EndTime = oneCase.ServiceTime;
                        oneCase.TimeInQueue = 0;
                        simSystem.Servers[serverID].FinishTime = oneCase.EndTime;
                        simSystem.Servers[serverID].TotalWorkingTime += oneCase.ServiceTime;
                        simSystem.SimulationTable.Add(oneCase);
                    }
                    else
                    {
                        oneCase.CustomerNumber = customer + 1;
                        oneCase.RandomInterArrival = RandomInterArrival.Next(1, 100);
                        oneCase.InterArrival = calculateArrivalTime(oneCase.RandomInterArrival);
                        eachArrivalTime += oneCase.InterArrival;
                        oneCase.ArrivalTime = eachArrivalTime;
                        //Check Type Of System
                        // Highest HighestPriority
                        int serverID = 0; // Default
                        if (simSystem.SelectionMethod == Enums.SelectionMethod.Random)
                        {
                            List<Server> emptyServers = new List<Server>();
                            foreach (Server localServer in simSystem.Servers)
                            {
                                if (oneCase.ArrivalTime >= localServer.FinishTime)
                                    emptyServers.Add(localServer);
                            }
                            if (emptyServers.Count != 0)
                            {
                                Random rndServers = new Random();
                                serverID = (emptyServers[rndServers.Next(0, emptyServers.Count)].ID) - 1;
                            }
                            else
                            {
                                int firstFinish = 9999999;
                                serverID = 0;
                                foreach (Server localServer in simSystem.Servers)
                                {
                                    if (firstFinish > localServer.FinishTime)
                                    {
                                        firstFinish = localServer.FinishTime;
                                        serverID = localServer.ID - 1;
                                    }
                                }
                            }
                        }
                        else if (simSystem.SelectionMethod == Enums.SelectionMethod.HighestPriority)
                        {
                            int firstFinish = 10000;
                            serverID = 0;
                            foreach (Server localServer in simSystem.Servers)
                            {
                                if (localServer.FinishTime <= oneCase.ArrivalTime)
                                {
                                    serverID = localServer.ID - 1;
                                    break;
                                }
                                else if (firstFinish > localServer.FinishTime)
                                {
                                    firstFinish = localServer.FinishTime;
                                    serverID = localServer.ID - 1;
                                }
                            }
                            //return serverID;
                        }

                        oneCase.RandomService = RandomService.Next(1, 100);
                        oneCase.ServiceTime = calculateServiceForServer(serverID, oneCase.RandomService);
                        oneCase.StartTime = Math.Max(oneCase.ArrivalTime, simSystem.Servers[serverID].FinishTime);
                        oneCase.EndTime = oneCase.StartTime + oneCase.ServiceTime;
                        oneCase.TimeInQueue = oneCase.StartTime - oneCase.ArrivalTime;
                        oneCase.AssignedServer = simSystem.Servers[serverID];
                        simSystem.Servers[serverID].FinishTime = oneCase.EndTime;
                        simSystem.Servers[serverID].TotalWorkingTime += oneCase.ServiceTime;
                        simSystem.SimulationTable.Add(oneCase);
                    }
                }
            }
            //Performace Measures For System
            calculateSystemPerformance();
            //Performance Measures For Each Server
            calculateServersPerformace();
            //Show Test Results.
            string result = TestingManager.Test(simSystem, Constants.FileNames.TestCase1);
            MessageBox.Show(result);
            var binding = new BindingList<SimulationCase>(simSystem.SimulationTable);
            var src = new BindingSource(binding, null);
            dataGridView1.DataSource = src;
        }

       
    }
}
