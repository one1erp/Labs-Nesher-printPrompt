using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace printPrompt
{
    public partial class DialogForm : Form
    {
        private int numOfCopies;

        public DialogForm()
        {
            InitializeComponent();
        }

        public int NumOfCopies
        {
            get { return numOfCopies; }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            numOfCopies = (int) txtNumOfCopies.Value;
            Close();
        }
    }
}
