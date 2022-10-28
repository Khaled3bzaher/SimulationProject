using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MultiQueueSimulation
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
            
        }

        private void chart2_Click(object sender, EventArgs e)
        {

        }
        private void Form2_Load(object sender, EventArgs e)
        {
            int TheEndTime=Form1.simSystem.Servers[serverID].FinishTime;
            int NumberOfServer = Form1.serverID+1;
            var object1 = chart2.ChartAreas[0];
            object1.AxisX.IntervalType = System.Windows.Forms.DataVisualization.Charting.DateTimeIntervalType.Number;
            object1.AxisX.Minimum = 0;
            object1.AxisY.IntervalType = System.Windows.Forms.DataVisualization.Charting.DateTimeIntervalType.Number;
            object1.AxisY.Minimum = 0;
            object1.AxisY.Maximum = 1;
            int counter = 0;
            for (int k = 0; k < Form1.simSystem.SimulationTable.Count; k++)
            {
                bool total_work = (Form1.simSystem.SimulationTable[k].AssignedServer.ID == NumberOfServer);
                counter=(total_work ? 1 : 0);
                chart2.Series["Busy"].Points.AddXY(k.ToString(), counter.ToString());
            
            
            }
        }

       
    }
}
