using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Poker
{
    public partial class frmPicTest : Form
    {
        public frmPicTest()
        {
            InitializeComponent();
        }
        private Image GetImage(string name)
        {
            return Poker.Properties.Resources.ResourceManager.GetObject(name) as Image;
        }

        private Image GetImage(int num)
        {
            return GetImage($"pic{num}");
        }

        private void btnTest_Click(object sender, EventArgs e)
        {

            Random random = new Random();
            int r = random.Next(1, 53);

            //this.picTest.Image = GetImage("back");
            this.picTest.Image = GetImage(r);

            this.lblNum.Text = $"{r}";
        }

        private void picTest_Click(object sender, EventArgs e)
        {
            this.picTest.Image = GetImage("back");
        }
    }
}
