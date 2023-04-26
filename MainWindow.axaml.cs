using System;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;

namespace GuessingGameClientAvalonia
{
    public partial class MainWindow : Window
    {
        private TcpClient? _client;
        private StreamReader? _reader;
        private StreamWriter? _writer;

        private TextBox _txtOutput;
        private TextBox _txtInput;
        private Button _btnSend;

        public MainWindow()
        {
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
            ConnectToServer();

            _txtOutput = this.FindControl<TextBox>("txtOutput");
            _txtInput = this.FindControl<TextBox>("txtInput");
            _btnSend = this.FindControl<Button>("btnSend");
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        private void ConnectToServer()
        {
            _client = new TcpClient("127.0.0.1", 8080);
            NetworkStream stream = _client.GetStream();
            _reader = new StreamReader(stream);
            _writer = new StreamWriter(stream);

            Thread readThread = new Thread(ReadMessages);
            readThread.Start();
        }

        private void ReadMessages()
        {
            while (_reader != null)
            {
                string? message = _reader.ReadLine();
                if (message != null)
                {
                    Dispatcher.UIThread.InvokeAsync(() => _txtOutput.Text += message + Environment.NewLine);
                }
            }
        }

        private void BtnSend_Click(object sender, RoutedEventArgs e)
        {
            if (_writer != null)
            {
                string input = _txtInput.Text;
                _writer.WriteLine(input);
                _writer.Flush();
                _txtInput.Text = string.Empty;
            }
        }
    }
}
