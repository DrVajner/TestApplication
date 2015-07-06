using OracleConn;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TestApplication
{
    public partial class TestForm : Form
    {
        public TestForm()
        {
            //DateTime? date = DateTime.Now;
            //var type = date.GetType();
            //if (type == typeof(Nullable<DateTime>))
            //{
            //}

            InitializeComponent();
        }

        private void ButtonRun_Click(object sender, EventArgs e)
        {
            var conn = new OraConnection();
            conn.CreateNewConnection("Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(Host=11.22.33.44)(PORT=1521))(CONNECT_DATA=(SERVICE_NAME=CUSTOMDB)));User Id=system;Password=System01;");
            var obj = conn.GetScalar(@"select {0} from dual", string.Empty);
            //var date = conn.TestMethod();
            //textBoxResult.Text = date.ToString();
            //textBoxResult.Text += Environment.NewLine;
            //textBoxResult.Text += conn.TestMethodTwo(514);
            conn.CloseConnection();
        }
    }
}
