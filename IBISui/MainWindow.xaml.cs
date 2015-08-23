using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Timers;
using System.IO.Ports;

using IBIScmdline;

namespace IBISui
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        SerialManager comm = new SerialManager();
        static Timer myTimer;
        Boolean isOpen = false;
        Boolean isTimer = false;

        public MainWindow()
        {
            InitializeComponent();
            myTimer = new Timer(10000); // Set up the timer for 10 seconds
                                        //
                                        // Type "_timer.Elapsed += " and press tab twice.
                                        //
            myTimer.Elapsed += new ElapsedEventHandler(myTimer_Elapsed);
        }

        private void dlgOpenFile(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            // Set filter for file extension and default file extension
            dlg.DefaultExt = ".txt";
            dlg.Filter = "Text documents (.txt)|*.txt";

            // Display OpenFileDialog by calling ShowDialog method
            Nullable<bool> result = dlg.ShowDialog();

            // Get the selected file name and display in a TextBox
            if (result == true)
            {
                // Open document
                string filename = dlg.FileName;
                //                FileNameTextBox.Text = filename;
            }
        }

        private void dlgFileSave(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
            dlg.DefaultExt = ".txt";
            dlg.Filter = "Text documents (.txt)|*.txt";

            // Display OpenFileDialog by calling ShowDialog method
            Nullable<bool> result = dlg.ShowDialog();

            // Get the selected file name and display in a TextBox
            if (result == true)
            {
                // save document
                string filename = dlg.FileName;
            }
        }

        private void txtLiniennummer_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !AreAllValidNumericChars(e.Text);
            base.OnPreviewTextInput(e);
        }

        private void txtSonderzeichen_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !AreAllValidNumericChars(e.Text);
            base.OnPreviewTextInput(e);
        }

        bool AreAllValidNumericChars(string str)
        {
            bool ret = true;

            int l = str.Length;
            for (int i = 0; i < l; i++)
            {
                char ch = str[i];
                ret &= Char.IsDigit(ch);
            }
            return ret;
        }

        private void mnuSerial_Click(object sender, RoutedEventArgs e)
        {
            mnuSerial.Items.Clear();
            var myPort = SerialPort.GetPortNames();
            foreach (string port in myPort)
            {
                // Create the new menu item
                MenuItem item = new MenuItem();
                item.Header = port;
                item.Click += new RoutedEventHandler(mnuSerialPort_Click);
                mnuSerial.Items.Add(item);
                if (item.Header.ToString() == comm.PortName)
                {
                    item.IsChecked = true;
                }
            }
        }

        private void mnuSerialPort_Click(object sender, RoutedEventArgs e)
        {
            MenuItem mi = e.Source as MenuItem;
            comm.ClosePort();
            isOpen = false;
            comm.PortName = mi.Header.ToString();
            mi.IsChecked = true;
        }

        private string centerString(string s, int desiredLength)
        {
            if (s.Length >= desiredLength) return s;
            int firstpad = (s.Length + desiredLength) / 2;
            return s.PadLeft(firstpad).PadRight(desiredLength);
        }

        private void sendIBIS()
        {
            if (isOpen == false)
            {
                comm.Parity = Parity.None.ToString();
                comm.StopBits = StopBits.Two.ToString();
                comm.DataBits = "8";
                comm.BaudRate = "1200";
                comm.PortName = comm.PortName;
                comm.Emulate7e1 = true;
                comm.OpenPort();
                isOpen = true;
            }
            if (isOpen == true)
            {
                //btnSenden.
                if (cbLiniennummer.IsChecked == true)
                {
                    comm.WriteIbisData("l" + txtLiniennummer.Text);
                }
                if (cbSonderzeichen.IsChecked == true)
                {
                    comm.WriteIbisData("lE" + txtSonderzeichen.Text);
                }
                if (cbZieltext1_2.IsChecked == true && cbZieltext3_4.IsChecked == true)
                {
                    comm.WriteIbisData("zA4" + centerString(txtZieltext1.Text, 16) + centerString(txtZieltext2.Text, 16) + centerString(txtZieltext3.Text, 16) + centerString(txtZieltext4.Text, 16));
                }
                else if (cbZieltext1_2.IsChecked == true && cbZieltext3_4.IsChecked == false)
                {
                    comm.WriteIbisData("zA2" + centerString(txtZieltext1.Text, 16) + centerString(txtZieltext2.Text, 16));
                }
                if (cbHaltestelle.IsChecked == true)
                {
                    comm.WriteIbisData("v" + centerString(txtHaltestelle.Text, 24));
                }
            }
        }

        private void btnSenden_Click(object sender, RoutedEventArgs e)
        {
            sendIBIS();
        }

        private void cbWiederhole_Click(object sender, RoutedEventArgs e)
        {
            if (cbWiederhole.IsChecked == true)
            {
                isTimer = true;
                myTimer.Enabled = true; // Enable it
            }
            else
            {
                isTimer = false;
                myTimer.Enabled = false; // Disable it
            }
        }

        private void myTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (isTimer == true)
            {
                this.Dispatcher.Invoke((Action)delegate ()
                {
                    this.sendIBIS();
                });

            }
        }

        private void mnuTimer_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
