using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SimensS7_Test
{
    public partial class Form1 : Form
    {
        static libnodave.daveOSserialType fds;
        static libnodave.daveInterface di;
        static libnodave.daveConnection dc;
        static int initSuccess = 0;
        static int localMPI = 0;
        static int plcMPI = 2;
        static int plc2MPI = -1;
        static int adrPos = 0;
        static int useProto = libnodave.daveProtoISOTCP;
        static int speed = libnodave.daveSpeed187k;
        static int rack = 0;
        static int slot = 3;
        static string IP = "192.168.8.2";
        static bool Connection = false;
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            fds.rfd = libnodave.openSocket(102,IP);
            fds.wfd = fds.rfd;

            if (fds.rfd > 0)
            {
                di = new libnodave.daveInterface(fds, "IF1", localMPI, useProto, speed);
                di.setTimeout(5000000);

                if (0 == di.initAdapter())
                {
                    initSuccess = 1;
                    dc = new libnodave.daveConnection(di, plcMPI, rack, slot);
                    int ret = dc.connectPLC();
                    if (ret == 0)
                    {
                        Connection = true;
                        this.lblstatus.Text = "已经连接PLC" + IP;
                    }
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                dc.disconnectPLC();
                //di.disconnectAdapter();
                //libnodave.closeS7online(fds.rfd);
                GC.Collect();
                GC.WaitForPendingFinalizers();

                Connection = false;
                this.lblstatus.Text = "已经断开与PLC连接";
            }
            catch (Exception ex)
            {
                this.lblstatus.Text = "断开错误:" + ex.Message;return;
            }
            

        }
        int i, a = 0, j, res, b = 0, c = 0;

        private void button6_Click(object sender, EventArgs e)
        {
            //读取：DB200.DBW4
            if (!Connection) { MessageBox.Show("与PLC连接已断开!"); return; }
            res = dc.readBytes(libnodave.daveDB, 200, 6, 2, null);
            if (res == 0)
            {
                short b = (short)dc.getS16(); textBox7.Text = b.ToString();
            }
            else
            {
                label7.Text = "error " + res + " " + libnodave.daveStrerror(res);
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            //读取：DB200.DBB1
            if (!Connection) { MessageBox.Show("与PLC连接已断开!"); return; }
            //res = dc.readBytes(address.Area, address.DBNumber, address.Start, 1, null);
            res = dc.readBytes(libnodave.daveDB, 200, 1, 1, null);
            if (res == 0)
            {
                byte b = (byte)dc.getS8(); textBox6.Text = b.ToString();
            }
            else
            {
                label6.Text = "error " + res + " " + libnodave.daveStrerror(res);
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            //读取：DB200.DBX0.0 
            if (!Connection) { MessageBox.Show("与PLC连接已断开!"); return; }
            //res = dc.readBits(address.Area, address.DBNumber, address.Start * 8 + address.Bit, 1, null);
            res = dc.readBits(libnodave.daveDB, 200, 0, 1, null);
            if (res == 0)
            {
                bool bl = dc.getS8()!=0; textBox5.Text = (bl==true)?"true":"false";
            }
            else
            {
                label5.Text = "error " + res + " " + libnodave.daveStrerror(res);
            }
        }

        float d = 0;
        byte[] buf1 = new byte[libnodave.davePartnerListSize];
        private void button3_Click(object sender, EventArgs e)
        {
            if (!Connection) { MessageBox.Show("与PLC连接已断开!");return; }
            int res = dc.readBytes(libnodave.daveDB, 200, 8, 16, null);
            if (res == 0)
            {
                a = dc.getS32(); textBox1.Text = a.ToString();
                b = dc.getS32(); textBox2.Text = b.ToString();
                c = dc.getS32(); textBox3.Text = c.ToString();
                d = dc.getFloat(); textBox4.Text = d.ToString();
            }
            else
            {
                label5.Text = "error " + res + " " + libnodave.daveStrerror(res);
            }
        }
    }
}