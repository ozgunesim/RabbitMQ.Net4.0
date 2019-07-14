using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using RabbitMQ.Client;

namespace Publisher
{
    public partial class Form1 : Form
    {
        IConnection connection;
        IModel channel;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            connection = factory.CreateConnection();
            channel = connection.CreateModel();
            channel.ExchangeDeclare(exchange: "servio_test", type: "fanout");

                
            

            Console.WriteLine(" Press [enter] to exit.");
            Console.ReadLine();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var message = textBox1.Text;
            var body = Encoding.UTF8.GetBytes(message);
            channel.BasicPublish(exchange: "servio_test",
                                 routingKey: "",
                                 basicProperties: null,
                                 body: body);
            Console.WriteLine(" [x] Sent {0}", message);
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            channel.Close();
            connection.Close();
        }
    }
}
