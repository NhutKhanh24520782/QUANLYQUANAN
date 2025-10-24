using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RestaurantClient
{
    public partial class DangNhap : Form
    {
        public DangNhap()
        {
            InitializeComponent();
        }

        private void groupBox_dangnhap_Enter(object sender, EventArgs e)
        {

        }

        private void linkLabel_dangky_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {

        }

        private void btn_dangnhap_Click(object sender, EventArgs e)
        {

        }

        private void linkLabel_forgetpasswd_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            NhapEmail a = new NhapEmail();
            a.Show();
        }
    }
}
