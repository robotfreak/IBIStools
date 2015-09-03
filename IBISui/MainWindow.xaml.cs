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
            getSerialPorts();
            myTimer = new Timer(10000); // Set up the timer for 10 seconds
                                        //
                                        // Type "_timer.Elapsed += " and press tab twice.
                                        //
            mnuTimer5s.IsChecked = false;
            mnuTimer10s.IsChecked = true;
            mnuTimer20s.IsChecked = false;
            mnuTimer30s.IsChecked = false;
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

        private void getSerialPorts()
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

        private void mnuSerial_Click(object sender, RoutedEventArgs e)
        {
            getSerialPorts();        }

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
                    comm.WriteIbisData("zA4" + centerString(txtZieltext1.Text, txtZieltext1.MaxLength) + centerString(txtZieltext2.Text, txtZieltext2.MaxLength) + centerString(txtZieltext3.Text, txtZieltext3.MaxLength) + centerString(txtZieltext4.Text, txtZieltext4.MaxLength));
                }
                else if (cbZieltext1_2.IsChecked == true && cbZieltext3_4.IsChecked == false)
                {
                    comm.WriteIbisData("zA2" + centerString(txtZieltext1.Text, txtZieltext1.MaxLength) + centerString(txtZieltext2.Text, txtZieltext2.MaxLength));
                }
                if (cbHaltestelle.IsChecked == true)
                {
                    comm.WriteIbisData("v" + centerString(txtHaltestelle.Text, txtHaltestelle.MaxLength));
                }
                if (cbRAW.IsChecked == true)
                {
                    comm.WriteIbisData(centerString(txtRAW.Text, txtRAW.MaxLength));
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
                sendIBIS();
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

        private void mnuHSA_Click(object sender, RoutedEventArgs e)
        {

        }

        private void cbHaltestelle_Click(object sender, RoutedEventArgs e)
        {
            if (cbHaltestelle.IsChecked == true)
            {
                txtHaltestelle.Visibility = Visibility.Visible;
            }
            else
            {
                txtHaltestelle.Visibility = Visibility.Collapsed;
            }
        }

        private void cbZieltext3_4_Click(object sender, RoutedEventArgs e)
        {
            if (cbZieltext3_4.IsChecked == true)
            {
                txtZieltext3.Visibility = Visibility.Visible;
                txtZieltext4.Visibility = Visibility.Visible;
            }
            else
            {
                txtZieltext3.Visibility = Visibility.Collapsed;
                txtZieltext4.Visibility = Visibility.Collapsed;
            }
        }

        private void cbZieltext1_2_Click(object sender, RoutedEventArgs e)
        {
            if (cbZieltext1_2.IsChecked == true)
            {
                txtZieltext1.Visibility = Visibility.Visible;
                txtZieltext2.Visibility = Visibility.Visible;
            }
            else
            {
                txtZieltext1.Visibility = Visibility.Collapsed;
                txtZieltext2.Visibility = Visibility.Collapsed;
            }
        }

        private void cbLiniennummer_Click(object sender, RoutedEventArgs e)
        {
            if (cbLiniennummer.IsChecked == true)
            {
                txtLiniennummer.Visibility = Visibility.Visible;
            }
            else
            {
                txtLiniennummer.Visibility = Visibility.Collapsed;
            }
        }

        private void cbSonderzeichen_Click(object sender, RoutedEventArgs e)
        {
            if (cbSonderzeichen.IsChecked == true)
            {
                txtSonderzeichen.Visibility = Visibility.Visible;
            }
            else
            {
                txtSonderzeichen.Visibility = Visibility.Collapsed;
            }
        }

        private void cbRAW_Click(object sender, RoutedEventArgs e)
        {
            if (cbRAW.IsChecked == true)
            {
                txtRAW.Visibility = Visibility.Visible;
            }
            else
            {
                txtRAW.Visibility = Visibility.Collapsed;
            }

        }

        private void mnuTimer5s_Click(object sender, RoutedEventArgs e)
        {
            myTimer.Interval = 5000;
            mnuTimer5s.IsChecked = true;
            mnuTimer10s.IsChecked = false;
            mnuTimer20s.IsChecked = false;
            mnuTimer30s.IsChecked = false;

        }

        private void mnuTimer10s_Click(object sender, RoutedEventArgs e)
        {
            myTimer.Interval = 10000;
            mnuTimer5s.IsChecked = false;
            mnuTimer10s.IsChecked = true;
            mnuTimer20s.IsChecked = false;
            mnuTimer30s.IsChecked = false;

        }

        private void mnuTimer20s_Click(object sender, RoutedEventArgs e)
        {
            myTimer.Interval = 20000;
            mnuTimer5s.IsChecked = false;
            mnuTimer10s.IsChecked = false;
            mnuTimer20s.IsChecked = true;
            mnuTimer30s.IsChecked = false;

        }

        private void mnuTimer30s_Click(object sender, RoutedEventArgs e)
        {
            myTimer.Interval = 30000;
            mnuTimer5s.IsChecked = false;
            mnuTimer10s.IsChecked = false;
            mnuTimer20s.IsChecked = false;
            mnuTimer30s.IsChecked = true;

        }

        private void txtZieltext1_TextChanged(object sender, TextChangedEventArgs e)
        {
            lblZieltext1.Content = txtZieltext1.Text.Length;
        }

        private void txtZieltext2_TextChanged(object sender, TextChangedEventArgs e)
        {
            lblZieltext2.Content = txtZieltext2.Text.Length;

        }

        private void txtZieltext3_TextChanged(object sender, TextChangedEventArgs e)
        {
            lblZieltext3.Content = txtZieltext3.Text.Length;
        }

        private void txtZieltext4_TextChanged(object sender, TextChangedEventArgs e)
        {
            lblZieltext4.Content = txtZieltext4.Text.Length;
        }

        private void txtHaltestelle_TextChanged(object sender, TextChangedEventArgs e)
        {
            lblHaltestelle.Content = txtHaltestelle.Text.Length;
        }

        private void txtRAW_TextChanged(object sender, TextChangedEventArgs e)
        {
            lblRAW.Content = txtRAW.Text.Length;
        }
    }
}
