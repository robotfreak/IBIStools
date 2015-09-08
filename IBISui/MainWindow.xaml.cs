using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Timers;
using System.IO.Ports;

using IBISutilities;

namespace IBISui
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Properties.Settings Props = null;
        SerialManager comm = new SerialManager();
        IBISutils utils = new IBISutils();
        static Timer myTimer;
        Boolean isTimer = false;
        private double timerInterval, timeLeft;
        Boolean isOpen = false;

        public MainWindow()
        {
            InitializeComponent();
            LoadSettings();
            getSerialPorts();
            InitTimer();
            var version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
            this.Title = "IBISui v" + version.ToString();
        }

        private void InitTimer()
        {
            timerInterval = Props.Timer;
            timeLeft = timerInterval / 1000;
            myTimer = new Timer(1000); // Set up the timer for 1 seconds
                                              //
                                              // Type "_timer.Elapsed += " and press tab twice.
                                              //
            myTimer.Elapsed += new ElapsedEventHandler(myTimer_Elapsed);
            if (timerInterval == 5000)
                mnuTimer5s.IsChecked = true;
            else if (timerInterval == 10000)
                mnuTimer10s.IsChecked = true;
            else if (timerInterval == 20000)
                mnuTimer20s.IsChecked = true;
            else if (timerInterval == 30000)
                mnuTimer30s.IsChecked = true;
        }

        private void LoadSettings()
        {
            Props = new Properties.Settings();
            comm.PortName = Props.ComPort;
            cbLiniennummer.IsChecked = Props.Liniennummer;
            cbSonderzeichen.IsChecked = Props.Sonderzeichen;
            cbWagennummer.IsChecked = Props.Wagennummer;
            if (Props.LA == "ds001")
            {
                mnuLAds001.IsChecked = true;
                mnuLAds001neu.IsChecked = false;
                txtLiniennummer.Text = "000";
                txtLiniennummer.MaxLength = 3;
            }
            if (Props.LA == "ds001neu")
            {
                mnuLAds001.IsChecked = false;
                mnuLAds001neu.IsChecked = true;
                txtLiniennummer.Text = "0000";
                txtLiniennummer.MaxLength = 4;
            }
            cbZieltext.IsChecked = Props.Zieltext;
            if (Props.ZA == "ds003a")
            {
                mnuZAds003a.IsChecked = true;
                mnuZAds003aMAS.IsChecked = false;
                mnuZAds003aMASctrl.IsChecked = false;
            }
            else if (Props.ZA == "ds3aMAS")
            {
                mnuZAds003a.IsChecked = false;
                mnuZAds003aMAS.IsChecked = true;
                mnuZAds003aMASctrl.IsChecked = false;
            }
            else if (Props.ZA == "ds3aMAS Control")
            {
                mnuZAds003a.IsChecked = false;
                mnuZAds003aMAS.IsChecked = false;
                mnuZAds003aMASctrl.IsChecked = true;
            }
            cbHaltestelle.IsChecked = Props.Haltestelle;
            txtHaltestelle.MaxLength = Convert.ToInt32(Props.HSAlen);
            foreach(MenuItem item in mnuHSAlen.Items)
            {
                if (item.Header.ToString() == Props.HSAlen)
                    item.IsChecked = true;
                else
                    item.IsChecked = false;
            }
            if (Props.HSA == "ds003c")
            {
                mnuHSAds003c.IsChecked = true;
                mnuHSAds009.IsChecked = false;
            }
            else if (Props.HSA == "ds009")
            {
                mnuHSAds003c.IsChecked = false;
                mnuHSAds009.IsChecked = true;
            }
            cbRAW.IsChecked = Props.RAW;
            this.updateUI();
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

        private void txtBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
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
            getSerialPorts();
        }

        private void mnuSerialPort_Click(object sender, RoutedEventArgs e)
        {
            MenuItem mi = e.Source as MenuItem;
            comm.ClosePort();
            isOpen = false;
            comm.PortName = mi.Header.ToString();
            Props.ComPort = comm.PortName;
            mi.IsChecked = true;
            updateUI();
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
                    if (mnuLAds001.IsChecked == true)
                    {
                        comm.WriteIbisData("l" + utils.fillDecimalString(txtLiniennummer.Text, 3));
                    }
                    else if (mnuLAds001neu.IsChecked == true)
                    {
                        comm.WriteIbisData("q" + utils.fillDecimalString(txtLiniennummer.Text, 4));
                    }
                }
                if (cbSonderzeichen.IsChecked == true)
                {
                    if (mnuLAds001.IsChecked == true)
                    {
                        if (txtSonderzeichen.Text == "00")
                            comm.WriteIbisData("lE" + "94");
                        else
                            comm.WriteIbisData("lE" + utils.fillDecimalString(txtSonderzeichen.Text, 2));
                    }
                    else if (mnuLAds001neu.IsChecked == true)
                    {
                        if (txtSonderzeichen.Text == "00")
                            comm.WriteIbisData("qE" + "94");
                        else
                            comm.WriteIbisData("qE" + utils.fillDecimalString(txtSonderzeichen.Text, 2));
                    }
                }
                if (cbWagennummer.IsChecked == true)
                {
                    comm.WriteIbisData("nB" + utils.dec2hexString(txtWagennummer.Text));
                }
                if (mnuZAds003aMAS.IsChecked == true && cbZieltext.IsChecked == true)
                {
                    if (cbLiniennummer.IsChecked == false && cbSonderzeichen.IsChecked == false)
                        comm.WriteIbisData("lE" + "94");
                    comm.WriteIbisData("zA4" + utils.centerString(txtZieltext1.Text, txtZieltext1.MaxLength) + 
                                               utils.centerString(txtZieltext2.Text, txtZieltext2.MaxLength) + 
                                               utils.centerString(txtZieltext3.Text, txtZieltext3.MaxLength) + 
                                               utils.centerString(txtZieltext4.Text, txtZieltext4.MaxLength));
                }
                else if (mnuZAds003a.IsChecked == true && cbZieltext.IsChecked == true)
                {
                    if (cbLiniennummer.IsChecked == false && cbSonderzeichen.IsChecked == false)
                        comm.WriteIbisData("lE" + "94");
                    comm.WriteIbisData("zA2" + utils.centerString(txtZieltext1.Text, txtZieltext1.MaxLength) +
                                               utils.centerString(txtZieltext2.Text, txtZieltext2.MaxLength));
                }
                else if (mnuZAds003aMASctrl.IsChecked == true && cbZieltext.IsChecked == true)
                {
                    if (cbLiniennummer.IsChecked == false && cbSonderzeichen.IsChecked == false)
                        comm.WriteIbisData("lE" + "94");
                    comm.WriteIbisData("zA5" + utils.leftalignedString(txtZieltext1.Text, txtZieltext1.MaxLength) +
                                               utils.leftalignedString(txtZieltext2.Text, txtZieltext2.MaxLength) +
                                               utils.leftalignedString(txtZieltext3.Text, txtZieltext3.MaxLength) +
                                               utils.leftalignedString(txtZieltext4.Text, txtZieltext4.MaxLength) + 
                                               "\n.BI@MBI@M      ");
                }
                if (cbHaltestelle.IsChecked == true)
                {
                    if (mnuHSAds003c.IsChecked == true)
                    {
                        string cmd = "";
                        if (txtHaltestelle.MaxLength == 16)
                            cmd = "zI4";
                        else if (txtHaltestelle.MaxLength == 20)
                            cmd = "zI5";
                        else if (txtHaltestelle.MaxLength == 24)
                            cmd = "zI6";
                        else if (txtHaltestelle.MaxLength == 28)
                            cmd = "zI7";
                        else if (txtHaltestelle.MaxLength == 32)
                            cmd = "zI8";
                        else if (txtHaltestelle.MaxLength == 36)
                            cmd = "zI9";
                        else if (txtHaltestelle.MaxLength == 40)
                            cmd = "zI:";
                        else if (txtHaltestelle.MaxLength == 44)
                            cmd = "zI;";
                        else if (txtHaltestelle.MaxLength == 48)
                            cmd = "zI<";
                        comm.WriteIbisData(cmd + utils.leftalignedString(utils.replaceString(txtHaltestelle.Text, '@', '\n'), txtHaltestelle.MaxLength));
                    }
                    else if (mnuHSAds009.IsChecked == true)
                        comm.WriteIbisData("v" + utils.centerString(txtHaltestelle.Text, txtHaltestelle.MaxLength));
                }
                if (cbRAW.IsChecked == true)
                {
                    comm.WriteIbisData(txtRAW.Text);
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
                pbTime.Visibility = Visibility.Visible;
                sendIBIS();
            }
            else
            {
                isTimer = false;
                myTimer.Enabled = false; // Disable it
                pbTime.Visibility = Visibility.Hidden;
            }
        }

        private void myTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (isTimer == true)
            {
                if (timeLeft > 0)
                {
                    timeLeft--;
                    this.Dispatcher.Invoke((Action)delegate ()
                    {
                        pbTime.Value = (timerInterval / 1000 - timeLeft) / (timerInterval / 1000) * 100;
                    });
                }
                else
                {
                    timeLeft = timerInterval / 1000;
                    this.Dispatcher.Invoke((Action)delegate ()
                    {
                        this.sendIBIS();
                        pbTime.Value = 100;
                    });
                }
                myTimer.Enabled = true;
            }
        }

        private void updateUI()
        {
            cbWiederhole.Content = "Datensatz alle " + Props.Timer/1000 + "s Senden";
            btnSenden.Content = "Senden " + Props.ComPort;

            cbHaltestelle.Content = "Haltestelle (" + Props.HSA + ") " + Props.HSAlen + " Zeichen";
            if (cbHaltestelle.IsChecked == true)
            {
                lblHaltestelle.Visibility = Visibility.Visible;
                txtHaltestelle.Visibility = Visibility.Visible;
            }
            else
            {
                lblHaltestelle.Visibility = Visibility.Collapsed;
                txtHaltestelle.Visibility = Visibility.Collapsed;
            }
            cbZieltext.Content = "Zieltext (" + Props.ZA + ")";
            if (cbZieltext.IsChecked == true)
            {
                lblZieltext1.Visibility = Visibility.Visible;
                txtZieltext1.Visibility = Visibility.Visible;
                lblZieltext2.Visibility = Visibility.Visible;
                txtZieltext2.Visibility = Visibility.Visible;
                if ((mnuZAds003aMAS.IsChecked == true || mnuZAds003aMASctrl.IsChecked == true) )
                {
                    lblZieltext3.Visibility = Visibility.Visible;
                    txtZieltext3.Visibility = Visibility.Visible;
                    lblZieltext4.Visibility = Visibility.Visible;
                    txtZieltext4.Visibility = Visibility.Visible;
                }
            }
            else
            {
                lblZieltext1.Visibility = Visibility.Collapsed;
                txtZieltext1.Visibility = Visibility.Collapsed;
                lblZieltext2.Visibility = Visibility.Collapsed;
                txtZieltext2.Visibility = Visibility.Collapsed;
                lblZieltext3.Visibility = Visibility.Collapsed;
                txtZieltext3.Visibility = Visibility.Collapsed;
                lblZieltext4.Visibility = Visibility.Collapsed;
                txtZieltext4.Visibility = Visibility.Collapsed;
            }
            cbLiniennummer.Content = "Liniennummer (" + Props.LA + ")";
            if (cbLiniennummer.IsChecked == true)
            {
                txtLiniennummer.Visibility = Visibility.Visible;
            }
            else
            {
                txtLiniennummer.Visibility = Visibility.Collapsed;
            }
            if (cbSonderzeichen.IsChecked == true)
            {
                txtSonderzeichen.Visibility = Visibility.Visible;
            }
            else
            {
                txtSonderzeichen.Visibility = Visibility.Collapsed;
            }
            if (cbWagennummer.IsChecked == true)
            {
                txtWagennummer.Visibility = Visibility.Visible;
            }
            else
            {
                txtWagennummer.Visibility = Visibility.Collapsed;
            }
            if (cbRAW.IsChecked == true)
            {
                lblRAW.Visibility = Visibility.Visible;
                txtRAW.Visibility = Visibility.Visible;
            }
            else
            {
                lblRAW.Visibility = Visibility.Collapsed;
                txtRAW.Visibility = Visibility.Collapsed;
            }
        }

        private void cbHaltestelle_Click(object sender, RoutedEventArgs e)
        {
            Props.Haltestelle = cbHaltestelle.IsChecked.Value;
            this.updateUI();
        }

        private void cbZieltext_Click(object sender, RoutedEventArgs e)
        {
            Props.Zieltext = cbZieltext.IsChecked.Value;
            this.updateUI();
        }

        private void cbLiniennummer_Click(object sender, RoutedEventArgs e)
        {
            Props.Liniennummer = cbLiniennummer.IsChecked.Value;
            this.updateUI();
        }

        private void cbSonderzeichen_Click(object sender, RoutedEventArgs e)
        {
            Props.Sonderzeichen = cbSonderzeichen.IsChecked.Value;
            this.updateUI();
        }

        private void cbWagennummer_Click(object sender, RoutedEventArgs e)
        {
            Props.Wagennummer = cbWagennummer.IsChecked.Value;
            this.updateUI();
        }

        private void cbRAW_Click(object sender, RoutedEventArgs e)
        {
            Props.RAW = cbRAW.IsChecked.Value;
            this.updateUI();
        }

        private void mnuTimer_Click(object sender, RoutedEventArgs e)
        {
            foreach (MenuItem item in mnuTimer.Items)
            {
                item.IsChecked = false;
            }

            MenuItem mi = e.Source as MenuItem;
            mi.IsChecked = true;
            Props.Timer = Convert.ToInt64(mi.Header.ToString()) * 1000.0;
            timerInterval = Props.Timer;
            this.updateUI();
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

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Props.Save();
            Props.Reload();
        }

        private void mnuHSAds003c_Click(object sender, RoutedEventArgs e)
        {
            mnuHSAds003c.IsChecked = true;
            mnuHSAds009.IsChecked = false;
            Props.HSA = "ds003c";
            this.updateUI();
        }

        private void mnuHSAds009_Click(object sender, RoutedEventArgs e)
        {
            mnuHSAds003c.IsChecked = false;
            mnuHSAds009.IsChecked = true;
            Props.HSA = "ds009";
            this.updateUI();
        }

        private void mnuZAds003a_Click(object sender, RoutedEventArgs e)
        {
            mnuZAds003a.IsChecked = true;
            mnuZAds003aMAS.IsChecked = false;
            mnuZAds003aMASctrl.IsChecked = false;
            Props.ZA = "ds003a";
            this.updateUI();
        }

        private void mnuZAds003aMAS_Click(object sender, RoutedEventArgs e)
        {
            mnuZAds003a.IsChecked = false;
            mnuZAds003aMAS.IsChecked = true;
            mnuZAds003aMASctrl.IsChecked = false;
            Props.ZA = "ds3aMAS";
            this.updateUI();
        }

        private void mnuZAds003aMASctrl_Click(object sender, RoutedEventArgs e)
        {
            mnuZAds003a.IsChecked = false;
            mnuZAds003aMAS.IsChecked = false;
            mnuZAds003aMASctrl.IsChecked = true;
            Props.ZA = "ds3aMAS Control";
            this.updateUI();

        }

        private void mnuLAds001_Click(object sender, RoutedEventArgs e)
        {
            mnuLAds001.IsChecked = true;
            mnuLAds001neu.IsChecked = false;
            txtLiniennummer.Text = "000";
            txtLiniennummer.MaxLength = 3;
            Props.LA = "ds001";
            this.updateUI();
        }

        private void mnuLAds001neu_Click(object sender, RoutedEventArgs e)
        {
            mnuLAds001.IsChecked = false;
            mnuLAds001neu.IsChecked = true;
            txtLiniennummer.Text = "0000";
            txtLiniennummer.MaxLength = 4;
            Props.LA = "ds001neu";
            this.updateUI();
        }

        private void mnuHSAlen_Click(object sender, RoutedEventArgs e)
        {
            foreach (MenuItem item in mnuHSAlen.Items)
            {
                item.IsChecked = false;
            }

            MenuItem mi = e.Source as MenuItem;
            mi.IsChecked = true;
            Props.HSAlen = mi.Header.ToString();
            txtHaltestelle.MaxLength = Convert.ToInt32(Props.HSAlen);
            this.updateUI();
        }
    }
}
