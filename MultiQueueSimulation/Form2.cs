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

namespace MultiQueueSimulation
{
    public partial class Form2 : Form
    {
        SimulationSystem simSystem;
        int serverID;
        int systemFinishTime;
        public Form2(SimulationSystem simulationSystem,int serverIIIIIID,int finishTime)
        {
            InitializeComponent();
            simSystem = simulationSystem;
            serverID = serverIIIIIID;
            systemFinishTime = finishTime;
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            var object1 = chart1.ChartAreas[0];
            object1.AxisX.IntervalType = System.Windows.Forms.DataVisualization.Charting.DateTimeIntervalType.Number;
            object1.AxisX.Minimum = 0;
            object1.AxisX.Maximum = systemFinishTime;
            object1.AxisY.IntervalType = System.Windows.Forms.DataVisualization.Charting.DateTimeIntervalType.Number;
            object1.AxisY.Minimum = 0;
            object1.AxisY.Maximum = 1;
            foreach(SimulationCase oneCase in simSystem.SimulationTable)
            {
                if(oneCase.AssignedServer.ID== serverID)
                {
                    for(int startTime=oneCase.StartTime;startTime<= oneCase.EndTime; startTime++)
                    {
                        chart1.Series["Busy"].Points.AddXY(startTime, 1);
                    }
                }
            }
        }
    }
}
