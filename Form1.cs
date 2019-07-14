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
using RabbitMQ.Client.Events;

namespace RabbitMQNet40
{
    public partial class Form1 : Form
    {
        IModel channel;
        IConnection conn;
        EventingBasicConsumer consumer;
        string queueName;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            ConnectionFactory factory = new ConnectionFactory();
            // "guest"/"guest" by default, limited to localhost connections
            factory.UserName = "guest";
            factory.Password = "guest";
            factory.VirtualHost = "/";
            factory.HostName = "localhost";
            factory.Port = 5672;

            conn = factory.CreateConnection();
            channel = conn.CreateModel();
            channel.ExchangeDeclare("servio_test", "fanout");

            queueName = channel.QueueDeclare().QueueName;
            channel.QueueBind(queue: queueName,
                              exchange: "servio_test",
                              routingKey: "");

            consumer = new EventingBasicConsumer(channel);
            //consumer.Received += Consumer_Received;
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body;
                var message = Encoding.UTF8.GetString(body);
                //MessageBox.Show(message.ToString());
                Console.WriteLine(message.ToString());
                textBox1.Invoke((MethodInvoker)delegate {
                    // Running on the UI thread
                    textBox1.Text += message.ToString() + Environment.NewLine;
                });
            };

            channel.BasicConsume(queue: queueName,
                                 noAck: true,
                                 consumer: consumer);


        }
        

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            channel.Close();
            conn.Close();
        }
    }
}
