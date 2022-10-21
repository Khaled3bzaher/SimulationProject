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
        private void Form1_Load(object sender, EventArgs e)
        {
            // Read From Test Case File
            FileStream fileStream = new FileStream("TestCases/TestCase1.txt", FileMode.Open);
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
                else if(lineRead == "StoppingNumber")
                {
                    simSystem.StoppingNumber = int.Parse(reader.ReadLine());
                }
                else if(lineRead == "StoppingCriteria"){
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
                else if(lineRead== "SelectionMethod")
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
                else if(lineRead == "InterarrivalDistribution")
                {
                    while (true) { 
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
                }
            }
        }
    }
}
