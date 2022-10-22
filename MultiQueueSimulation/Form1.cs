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
            simSystem.readFromFile("G:/FCIS/Seventh Semester/Modeling & Simulation/Labs/Lab 2/Template_Students/MultiQueueSimulation/MultiQueueSimulation/TestCases/TestCase1.txt",simSystem);
            
        }
    }
}
