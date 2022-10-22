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
        private void Form1_Load(object sender, EventArgs e)
        {
            simSystem.readFromFile("G:/FCIS/Seventh Semester/Modeling & Simulation/Labs/Lab 2/Template_Students/MultiQueueSimulation/MultiQueueSimulation/TestCases/TestCase1.txt",simSystem);
            //Starting System
            Random RandomInterArrival = new Random();
            Random RandomService = new Random();
            Random nextServer = new Random();
            if (simSystem.StoppingCriteria == Enums.StoppingCriteria.NumberOfCustomers)
            {
                for(int customer = 0; customer < simSystem.StoppingNumber; customer++)
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
                        simSystem.SimulationTable.Add(oneCase);
                    }
                    else
                    {
                        break;
                        //Remove Break and Continue..
                    }
                }
            }
            string result = TestingManager.Test(simSystem, Constants.FileNames.TestCase1);
            MessageBox.Show(result);
        }
    }
}
