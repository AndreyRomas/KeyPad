using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OpenHardwareMonitor.Hardware;
using System.Management;
using System.Diagnostics;
using System.IO.Ports;
using System.Threading;

using WindowsInput.Native;
using WindowsInput;

namespace keypadnamespace
{
    public partial class Form1 : Form
    {
        private Thread trd;
        bool thrwork = true;
        bool connected = false;
        Computer computer = new Computer() { CPUEnabled = true, GPUEnabled = true };
        static PerformanceCounter memCounter;
        ManagementObjectSearcher wmiObject = new ManagementObjectSearcher("select * from Win32_OperatingSystem");
        int cputemp = 0;
        int gputemp = 0;
        int memused = 0;
        int memtotal = 0;
        int cpupercent = 0;
        int gpupercent = 0;
        int mempercent = 0;
        String[] ports;
        SerialPort port;
        string msg = "";
        InputSimulator sim = new InputSimulator();
        readonly VirtualKeyCode[] Keylist = new VirtualKeyCode[118]
        {
            VirtualKeyCode.TAB,
            VirtualKeyCode.RETURN,
            VirtualKeyCode.SHIFT,
            VirtualKeyCode.CONTROL,
            VirtualKeyCode.MENU,
            VirtualKeyCode.PAUSE,
            VirtualKeyCode.CAPITAL,
            VirtualKeyCode.FINAL,
            VirtualKeyCode.ESCAPE,
            VirtualKeyCode.SPACE,
            VirtualKeyCode.NEXT,
            VirtualKeyCode.END,
            VirtualKeyCode.HOME,
            VirtualKeyCode.LEFT,
            VirtualKeyCode.UP,
            VirtualKeyCode.RIGHT,
            VirtualKeyCode.DOWN,
            VirtualKeyCode.SELECT,
            VirtualKeyCode.PRINT,
            VirtualKeyCode.EXECUTE,
            VirtualKeyCode.SNAPSHOT,
            VirtualKeyCode.INSERT,
            VirtualKeyCode.DELETE,
            VirtualKeyCode.HELP,
            VirtualKeyCode.VK_0,
            VirtualKeyCode.VK_1,
            VirtualKeyCode.VK_2,
            VirtualKeyCode.VK_3,
            VirtualKeyCode.VK_4,
            VirtualKeyCode.VK_5,
            VirtualKeyCode.VK_6,
            VirtualKeyCode.VK_7,
            VirtualKeyCode.VK_8,
            VirtualKeyCode.VK_9,
            VirtualKeyCode.VK_A,
            VirtualKeyCode.VK_B,
            VirtualKeyCode.VK_C,
            VirtualKeyCode.VK_D,
            VirtualKeyCode.VK_E,
            VirtualKeyCode.VK_F,
            VirtualKeyCode.VK_G,
            VirtualKeyCode.VK_H,
            VirtualKeyCode.VK_I,
            VirtualKeyCode.VK_J,
            VirtualKeyCode.VK_K,
            VirtualKeyCode.VK_L,
            VirtualKeyCode.VK_M,
            VirtualKeyCode.VK_N,
            VirtualKeyCode.VK_O,
            VirtualKeyCode.VK_P,
            VirtualKeyCode.VK_Q,
            VirtualKeyCode.VK_R,
            VirtualKeyCode.VK_S,
            VirtualKeyCode.VK_T,
            VirtualKeyCode.VK_U,
            VirtualKeyCode.VK_V,
            VirtualKeyCode.VK_W,
            VirtualKeyCode.VK_X,
            VirtualKeyCode.VK_Y,
            VirtualKeyCode.VK_Z,
            VirtualKeyCode.LWIN,
            VirtualKeyCode.RWIN,
            VirtualKeyCode.SLEEP,
            VirtualKeyCode.NUMPAD0,
            VirtualKeyCode.NUMPAD1,
            VirtualKeyCode.NUMPAD2,
            VirtualKeyCode.NUMPAD3,
            VirtualKeyCode.NUMPAD4,
            VirtualKeyCode.NUMPAD5,
            VirtualKeyCode.NUMPAD6,
            VirtualKeyCode.NUMPAD7,
            VirtualKeyCode.NUMPAD8,
            VirtualKeyCode.NUMPAD9,
            VirtualKeyCode.MULTIPLY,
            VirtualKeyCode.ADD,
            VirtualKeyCode.SEPARATOR,
            VirtualKeyCode.SUBTRACT,
            VirtualKeyCode.DECIMAL,
            VirtualKeyCode.DIVIDE,
            VirtualKeyCode.F1,
            VirtualKeyCode.F2,
            VirtualKeyCode.F3,
            VirtualKeyCode.F4,
            VirtualKeyCode.F5,
            VirtualKeyCode.F6,
            VirtualKeyCode.F7,
            VirtualKeyCode.F8,
            VirtualKeyCode.F9,
            VirtualKeyCode.F10,
            VirtualKeyCode.F11,
            VirtualKeyCode.F12,
            VirtualKeyCode.F13,
            VirtualKeyCode.F14,
            VirtualKeyCode.F15,
            VirtualKeyCode.F16,
            VirtualKeyCode.F17,
            VirtualKeyCode.F18,
            VirtualKeyCode.F19,
            VirtualKeyCode.F20,
            VirtualKeyCode.F21,
            VirtualKeyCode.F22,
            VirtualKeyCode.F23,
            VirtualKeyCode.F24,
            VirtualKeyCode.NUMLOCK,
            VirtualKeyCode.SCROLL,
            VirtualKeyCode.LSHIFT,
            VirtualKeyCode.RSHIFT,
            VirtualKeyCode.LCONTROL,
            VirtualKeyCode.RCONTROL,
            VirtualKeyCode.LMENU,
            VirtualKeyCode.RMENU,
            VirtualKeyCode.VOLUME_MUTE,
            VirtualKeyCode.VOLUME_DOWN,
            VirtualKeyCode.VOLUME_UP,
            VirtualKeyCode.MEDIA_NEXT_TRACK,
            VirtualKeyCode.MEDIA_PREV_TRACK,
            VirtualKeyCode.MEDIA_STOP,
            VirtualKeyCode.MEDIA_PLAY_PAUSE
        };
        readonly string[] act = new string[119] 
        {   
            "",
            "TAB",
            "RETURN",
            "SHIFT",
            "CONTROL",
            "MENU",
            "PAUSE",
            "CAPITAL",
            "FINAL",
            "ESCAPE",
            "SPACE",
            "NEXT",
            "END",
            "HOME",
            "LEFT",
            "UP",
            "RIGHT",
            "DOWN",
            "SELECT",
            "PRINT",
            "EXECUTE",
            "SNAPSHOT",
            "INSERT",
            "DELETE",
            "HELP",
            "0",
            "1",
            "2",
            "3",
            "4",
            "5",
            "6",
            "7",
            "8",
            "9",
            "A",
            "B",
            "C",
            "D",
            "E",
            "F",
            "G",
            "H",
            "I",
            "J",
            "K",
            "L",
            "M",
            "N",
            "O",
            "P",
            "Q",
            "R",
            "S",
            "T",
            "U",
            "V",
            "W",
            "X",
            "Y",
            "Z",
            "LWIN",
            "RWIN",
            "SLEEP",
            "NUMPAD0",
            "NUMPAD1",
            "NUMPAD2",
            "NUMPAD3",
            "NUMPAD4",
            "NUMPAD5",
            "NUMPAD6",
            "NUMPAD7",
            "NUMPAD8",
            "NUMPAD9",
            "MULTIPLY",
            "ADD",
            "SEPARATOR",
            "SUBTRACT",
            "DECIMAL",
            "DIVIDE",
            "F1",
            "F2",
            "F3",
            "F4",
            "F5",
            "F6",
            "F7",
            "F8",
            "F9",
            "F10",
            "F11",
            "F12",
            "F13",
            "F14",
            "F15",
            "F16",
            "F17",
            "F18",
            "F19",
            "F20",
            "F21",
            "F22",
            "F23",
            "F24",
            "NUMLOCK",
            "SCROLL",
            "LSHIFT",
            "RSHIFT",
            "LCONTROL",
            "RCONTROL",
            "LMENU",
            "RMENU",
            "VOLUME_MUTE",
            "VOLUME_DOWN",
            "VOLUME_UP",
            "MEDIA_NEXT_TRACK",
            "MEDIA_PREV_TRACK",
            "MEDIA_STOP",
            "MEDIA_PLAY_PAUSE"
        };
        byte[] button1color = new byte[6] { 0, 0, 0, 0, 0, 0 };
        byte[] button2color = new byte[6] { 0, 0, 0, 0, 0, 0 };
        byte[] button3color = new byte[6] { 0, 0, 0, 0, 0, 0 };
        byte[] button4color = new byte[6] { 0, 0, 0, 0, 0, 0 };
        byte[] button5color = new byte[6] { 0, 0, 0, 0, 0, 0 };
        byte[] button6color = new byte[6] { 0, 0, 0, 0, 0, 0 };
        public Form1()
        {
            computer.Open();
            memCounter = new PerformanceCounter("Memory", "% Committed Bytes In Use", null);
            InitializeComponent();
            ports = SerialPort.GetPortNames();
            foreach (string port in ports)
            {
                comboBox1.Items.Add(port);
                Console.WriteLine(port);
                if (ports[0] != null)
                {
                    comboBox1.SelectedItem = ports[0];
                }
            }
            Thread trd = new Thread(new ThreadStart(this.ThreadTask));
            trd.IsBackground = true;
            trd.Start();
            for (int i = 0; i < act.Length; i++)
            {
                comboBox2.Items.Add(act[i]);
                comboBox3.Items.Add(act[i]);
                comboBox4.Items.Add(act[i]);
                comboBox5.Items.Add(act[i]);
                comboBox6.Items.Add(act[i]);
                comboBox7.Items.Add(act[i]);
                comboBox8.Items.Add(act[i]);
                comboBox9.Items.Add(act[i]);
                comboBox10.Items.Add(act[i]);
                comboBox11.Items.Add(act[i]);
                comboBox12.Items.Add(act[i]);
                comboBox13.Items.Add(act[i]);
                comboBox14.Items.Add(act[i]);
                comboBox15.Items.Add(act[i]);
                comboBox16.Items.Add(act[i]);
                comboBox17.Items.Add(act[i]);
                comboBox18.Items.Add(act[i]);
                comboBox19.Items.Add(act[i]);
            }
            comboBox2.SelectedIndex = 0;
            comboBox3.SelectedIndex = 0;
            comboBox4.SelectedIndex = 0;
            comboBox5.SelectedIndex = 0;
            comboBox6.SelectedIndex = 0;
            comboBox7.SelectedIndex = 0;
            comboBox8.SelectedIndex = 0;
            comboBox9.SelectedIndex = 0;
            comboBox10.SelectedIndex = 0;
            comboBox11.SelectedIndex = 0;
            comboBox12.SelectedIndex = 0;
            comboBox13.SelectedIndex = 0;
            comboBox14.SelectedIndex = 0;
            comboBox15.SelectedIndex = 0;
            comboBox16.SelectedIndex = 0;
            comboBox17.SelectedIndex = 0;
            comboBox18.SelectedIndex = 0;
            comboBox19.SelectedIndex = 0;
            comboBox1.SelectedItem = null;


        }
        private void ConnectToArduino()
        {
            string selectedPort = comboBox1.GetItemText(comboBox1.SelectedItem);
            if (selectedPort != "")
            {
                port = new SerialPort(selectedPort, 9600, Parity.None, 8, StopBits.One);
                port.DtrEnable = false;
                port.ReadTimeout = 7000;
                System.Threading.Thread.Sleep(300);
                try
                {
                    label38.Text = "";
                    thrwork = false;
                    port.Open();
                    System.Threading.Thread.Sleep(2000);
                    port.Write("#");
                    System.Threading.Thread.Sleep(3000);
                    port.Write("#STAR\n");
                    string heymsg = "";
                    heymsg = port.ReadTo("\n").ToString();
                    if (heymsg.Contains("HEY"))
                    {
                        port.ReadTimeout = 1000;
                        label38.Text = "CONNECTED!";
                        label38.BackColor = Color.Green;
                        connected = true;
                        thrwork = true;
                        Sendlightinfo();
                        Senddispinfo();
                    }
                }
                catch
                {
                    MessageBox.Show("Ошибка подключения.", "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("Вы не выбрали порт!", "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void DisconnectFromArduino()
        {
            port.Write("#STOP\n");
            port.Close();
            label38.BackColor = Color.Red;
            label38.Text = "Not Connected";
            connected = false;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            System.Windows.Forms.Timer timer1 = new System.Windows.Forms.Timer();
            timer1.Interval = 1000;
            timer1.Tick += new System.EventHandler(Timer1_Tick);
            timer1.Start();
            GetSettings();
        }
        private void Timer1_Tick(object sender, EventArgs e)
        {
            if (port != null)
            {
                if (!port.IsOpen)
                {
                    label38.BackColor = Color.Red;
                    label38.Text = "Not Connected";
                }
            }
            else
            {
                label38.BackColor = Color.Red;
                label38.Text = "Not Connected";
            }
            Updateinfo();
            Showinfo();
            Sendinfo();
        }
        private void ThreadTask()
        {
            while (true)
            {
                while (thrwork && IsConnected())
                {
                    try
                    {
                        msg = port.ReadTo("\n").ToString();
                        if (msg[0].ToString() == "b")
                        {
                            msg = msg.Substring(1, 6);
                            run(msg);
                        }
                    }
                    catch { }

                }
            }
        }
        private void runbuttons(string data)
        {
            bool[] buttons = new bool[6];
            buttons[0] = false;
            buttons[1] = false;
            buttons[2] = false;
            buttons[3] = false;
            buttons[4] = false;
            buttons[5] = false;
            for (byte i = 0; i <= 5; i++)
            {
                if (data[i].ToString() == "1") buttons[i] = true; else buttons[i] = false;
            }
            if (buttons[0] == true) label10.BackColor = Color.Green; else label10.BackColor = Color.Red;
            if (buttons[1] == true) label11.BackColor = Color.Green; else label11.BackColor = Color.Red;
            if (buttons[2] == true) label12.BackColor = Color.Green; else label12.BackColor = Color.Red;
            if (buttons[3] == true) label13.BackColor = Color.Green; else label13.BackColor = Color.Red;
            if (buttons[4] == true) label14.BackColor = Color.Green; else label14.BackColor = Color.Red;
            if (buttons[5] == true) label15.BackColor = Color.Green; else label15.BackColor = Color.Red;
            //if (buttons[0] == true) SendKeys.Send(comboBox2.Text + comboBox13.Text + comboBox19.Text);//тут!
            //if (buttons[1] == true) SendKeys.Send(comboBox3.Text + comboBox12.Text + comboBox18.Text);
            //if (buttons[2] == true) SendKeys.Send(comboBox4.Text + comboBox11.Text + comboBox17.Text);
            //if (buttons[3] == true) SendKeys.Send(comboBox5.Text + comboBox10.Text + comboBox16.Text);
            //if (buttons[4] == true) SendKeys.Send(comboBox6.Text + comboBox9.Text + comboBox15.Text);
            //if (buttons[5] == true) SendKeys.Send(comboBox7.Text + comboBox8.Text + comboBox14.Text);
            if (buttons[0] == true)
            {
                if(comboBox2.SelectedIndex > 0) sim.Keyboard.KeyDown(Keylist[comboBox2.SelectedIndex-1]);
                if(comboBox13.SelectedIndex > 0) sim.Keyboard.KeyDown(Keylist[comboBox13.SelectedIndex - 1]);
                if(comboBox19.SelectedIndex > 0) sim.Keyboard.KeyDown(Keylist[comboBox19.SelectedIndex - 1]);
                if (comboBox2.SelectedIndex > 0) sim.Keyboard.KeyUp(Keylist[comboBox2.SelectedIndex - 1]);
                if (comboBox13.SelectedIndex > 0) sim.Keyboard.KeyUp(Keylist[comboBox13.SelectedIndex - 1]);
                if (comboBox19.SelectedIndex > 0) sim.Keyboard.KeyUp(Keylist[comboBox19.SelectedIndex - 1]);
            }
            if (buttons[1] == true)
            {
                if (comboBox3.SelectedIndex > 0) sim.Keyboard.KeyDown(Keylist[comboBox3.SelectedIndex - 1]);
                if (comboBox12.SelectedIndex > 0) sim.Keyboard.KeyDown(Keylist[comboBox12.SelectedIndex - 1]);
                if (comboBox18.SelectedIndex > 0) sim.Keyboard.KeyDown(Keylist[comboBox18.SelectedIndex - 1]);
                if (comboBox3.SelectedIndex > 0) sim.Keyboard.KeyUp(Keylist[comboBox3.SelectedIndex - 1]);
                if (comboBox12.SelectedIndex > 0) sim.Keyboard.KeyUp(Keylist[comboBox12.SelectedIndex - 1]);
                if (comboBox18.SelectedIndex > 0) sim.Keyboard.KeyUp(Keylist[comboBox18.SelectedIndex - 1]);
            }
            if (buttons[2] == true)
            {
                if (comboBox4.SelectedIndex > 0) sim.Keyboard.KeyDown(Keylist[comboBox4.SelectedIndex - 1]);
                if (comboBox11.SelectedIndex > 0) sim.Keyboard.KeyDown(Keylist[comboBox11.SelectedIndex - 1]);
                if (comboBox17.SelectedIndex > 0) sim.Keyboard.KeyDown(Keylist[comboBox17.SelectedIndex - 1]);
                if (comboBox4.SelectedIndex > 0) sim.Keyboard.KeyUp(Keylist[comboBox4.SelectedIndex - 1]);
                if (comboBox11.SelectedIndex > 0) sim.Keyboard.KeyUp(Keylist[comboBox11.SelectedIndex - 1]);
                if (comboBox17.SelectedIndex > 0) sim.Keyboard.KeyUp(Keylist[comboBox17.SelectedIndex - 1]);
            }
            if (buttons[3] == true)
            {
                if (comboBox5.SelectedIndex > 0) sim.Keyboard.KeyDown(Keylist[comboBox5.SelectedIndex - 1]);
                if (comboBox10.SelectedIndex > 0) sim.Keyboard.KeyDown(Keylist[comboBox10.SelectedIndex - 1]);
                if (comboBox16.SelectedIndex > 0) sim.Keyboard.KeyDown(Keylist[comboBox16.SelectedIndex - 1]);
                if (comboBox5.SelectedIndex > 0) sim.Keyboard.KeyUp(Keylist[comboBox5.SelectedIndex - 1]);
                if (comboBox10.SelectedIndex > 0) sim.Keyboard.KeyUp(Keylist[comboBox10.SelectedIndex - 1]);
                if (comboBox16.SelectedIndex > 0) sim.Keyboard.KeyUp(Keylist[comboBox16.SelectedIndex - 1]);
            }
            if (buttons[4] == true)
            {
                if (comboBox6.SelectedIndex > 0) sim.Keyboard.KeyDown(Keylist[comboBox6.SelectedIndex - 1]);
                if (comboBox9.SelectedIndex > 0) sim.Keyboard.KeyDown(Keylist[comboBox9.SelectedIndex - 1]);
                if (comboBox15.SelectedIndex > 0) sim.Keyboard.KeyDown(Keylist[comboBox15.SelectedIndex - 1]);
                if (comboBox6.SelectedIndex > 0) sim.Keyboard.KeyUp(Keylist[comboBox6.SelectedIndex - 1]);
                if (comboBox9.SelectedIndex > 0) sim.Keyboard.KeyUp(Keylist[comboBox9.SelectedIndex - 1]);
                if (comboBox15.SelectedIndex > 0) sim.Keyboard.KeyUp(Keylist[comboBox15.SelectedIndex - 1]);
            }
            if (buttons[5] == true)
            {
                if (comboBox7.SelectedIndex > 0) sim.Keyboard.KeyDown(Keylist[comboBox7.SelectedIndex - 1]);
                if (comboBox8.SelectedIndex > 0) sim.Keyboard.KeyDown(Keylist[comboBox8.SelectedIndex - 1]);
                if (comboBox14.SelectedIndex > 0) sim.Keyboard.KeyDown(Keylist[comboBox14.SelectedIndex - 1]);
                if (comboBox7.SelectedIndex > 0) sim.Keyboard.KeyUp(Keylist[comboBox7.SelectedIndex - 1]);
                if (comboBox8.SelectedIndex > 0) sim.Keyboard.KeyUp(Keylist[comboBox8.SelectedIndex - 1]);
                if (comboBox14.SelectedIndex > 0) sim.Keyboard.KeyUp(Keylist[comboBox14.SelectedIndex - 1]);
            }
        }
        private void run(string data)
        {
            this.Invoke(new MethodInvoker(delegate
            {
                runbuttons(data);
            }
            ));
        }
        private void Updateinfo()
        {
            foreach (IHardware hardware in computer.Hardware)
            {
                hardware.Update();
                foreach (ISensor sensor in hardware.Sensors)
                {
                    if (hardware.HardwareType == HardwareType.GpuAti || hardware.HardwareType == HardwareType.GpuNvidia)
                    {
                        if (sensor.SensorType == SensorType.Temperature) gputemp = (int)sensor.Value;
                        if (sensor.SensorType == SensorType.Load && sensor.Name.Contains("Core")) gpupercent = (int)sensor.Value;
                    }
                    if (hardware.HardwareType == HardwareType.CPU)
                    {
                        if (sensor.SensorType == SensorType.Temperature) cputemp = (int)sensor.Value;
                        if (sensor.SensorType == SensorType.Load) cpupercent = (int)sensor.Value;
                    }
                }

            }
            var pcValues = wmiObject.Get().Cast<ManagementObject>().Select(mo => new
            {
                FreePhysicalMemory = Double.Parse(mo["FreePhysicalMemory"].ToString()),
                TotalVisibleMemorySize = Double.Parse(mo["TotalVisibleMemorySize"].ToString())
            }).FirstOrDefault();
            memtotal = (int)(pcValues.TotalVisibleMemorySize / 1024);
            memused = (int)((pcValues.TotalVisibleMemorySize - pcValues.FreePhysicalMemory) / 1024);
            mempercent = (int)(((pcValues.TotalVisibleMemorySize - pcValues.FreePhysicalMemory) / pcValues.TotalVisibleMemorySize) * 100);
        }
        private void Showinfo()
        {
            label4.Text = cputemp.ToString() + "C";
            label5.Text = cpupercent.ToString() + "%";
            label6.Text = mempercent.ToString() + "%";
            label7.Text = memused.ToString() + "/" + memtotal.ToString() + "MB";
            label8.Text = gputemp.ToString() + "С";
            label9.Text = gpupercent.ToString() + "%";
        }
        private void Sendinfo()
        {
            string datatosend =
            "#data#" +
            cputemp.ToString() + "#" +
            cpupercent.ToString() + "#" +
            mempercent.ToString() + "#" +
            memused.ToString() + "#" +
            memtotal.ToString() + "#" +
            gputemp.ToString() + "#" +
            gpupercent.ToString() + "\n";
            if (IsConnected())
            {
                port.Write(datatosend);
            }
        }
        private void button2_Click(object sender, EventArgs e)
        {
            if (IsConnected())
            {
                DisconnectFromArduino();
            }
            else
            {
                ConnectToArduino();
            }
        }
        private bool IsConnected()
        {
            if (port != null)
            {
                if (port.IsOpen)
                {
                    return true;
                }
                else return false;
            }
            else return false;
        }
        private void Form1_FormClosing(object sender, EventArgs e)
        {
            DisconnectFromArduino();
            //trd.Abort();
        }


        private void button16_Click(object sender, EventArgs e)
        {
            ports = SerialPort.GetPortNames();
            comboBox1.Items.Clear();
            foreach (string port in ports)
            {
                comboBox1.Items.Add(port);
                Console.WriteLine(port);
                if (ports[0] != null)
                {
                    comboBox1.SelectedItem = ports[0];
                }
            }
        }

        private void ComboBox20_SelectedIndexChanged(object sender, EventArgs e)
        {
            Sendlightinfo();
        }

        private void button9_Click(object sender, EventArgs e)
        {
            Sendlightinfo();
            SaveSettings();
        }
        private void Sendlightinfo()
        {
            int selectedpreset = comboBox20.SelectedIndex;
            string datatosend =
            "#ligh#" +
            selectedpreset.ToString() + "\n";
            if (IsConnected())
            {
                port.Write(datatosend);
            }
        }
        private void Senddispinfo()
        {
            int selectedval = trackBar1.Value;
            string datatosend =
            "#disp#" +
            selectedval.ToString() + "\n";
            if (IsConnected())
            {

                port.Write(datatosend);
            }
        }
        public void GetSettings()
        {
            comboBox2.SelectedIndex = Properties.Settings.Default.combobox2;
            comboBox3.SelectedIndex = Properties.Settings.Default.combobox3;
            comboBox4.SelectedIndex = Properties.Settings.Default.combobox4;
            comboBox5.SelectedIndex = Properties.Settings.Default.combobox5;
            comboBox6.SelectedIndex = Properties.Settings.Default.combobox6;
            comboBox7.SelectedIndex = Properties.Settings.Default.combobox7;
            comboBox8.SelectedIndex = Properties.Settings.Default.combobox8;
            comboBox9.SelectedIndex = Properties.Settings.Default.combobox9;
            comboBox10.SelectedIndex = Properties.Settings.Default.combobox10;
            comboBox11.SelectedIndex = Properties.Settings.Default.combobox11;
            comboBox12.SelectedIndex = Properties.Settings.Default.combobox12;
            comboBox13.SelectedIndex = Properties.Settings.Default.combobox13;
            comboBox14.SelectedIndex = Properties.Settings.Default.combobox14;
            comboBox15.SelectedIndex = Properties.Settings.Default.combobox15;
            comboBox16.SelectedIndex = Properties.Settings.Default.combobox16;
            comboBox17.SelectedIndex = Properties.Settings.Default.combobox17;
            comboBox18.SelectedIndex = Properties.Settings.Default.combobox18;
            comboBox19.SelectedIndex = Properties.Settings.Default.combobox19;

            comboBox20.SelectedIndex = Properties.Settings.Default.lightmode;
            comboBox20.SelectedIndex = Properties.Settings.Default.lightmode;

            trackBar1.Value = Properties.Settings.Default.lightdisp;
        }
        public void SaveSettings()
        {
            Properties.Settings.Default.lightmode = comboBox20.SelectedIndex;
            Properties.Settings.Default.lightdisp = trackBar1.Value; 
            Properties.Settings.Default.Save();
        }

        private void Form1_SizeChanged(object sender, EventArgs e)
        {
            bool MousePointerNotOnTaskBar = Screen.GetWorkingArea(this).Contains(Cursor.Position);
            if (this.WindowState == FormWindowState.Minimized && MousePointerNotOnTaskBar)
            {
                notifyIcon1.BalloonTipText = "Я в трее";
                notifyIcon1.ShowBalloonTip(1000);
                this.ShowInTaskbar = false;
                notifyIcon1.Visible = true;
            }
        }
        private void notifyIcon1_MouseClick(object sender, MouseEventArgs e)
        {
            this.WindowState = FormWindowState.Normal;
            if (this.WindowState == FormWindowState.Normal)
            {
                this.ShowInTaskbar = true;
                notifyIcon1.Visible = true;
            }
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            Properties.Settings.Default.combobox2 = comboBox2.SelectedIndex;
            Properties.Settings.Default.combobox3 = comboBox3.SelectedIndex;
            Properties.Settings.Default.combobox4 = comboBox4.SelectedIndex;
            Properties.Settings.Default.combobox5 = comboBox5.SelectedIndex;
            Properties.Settings.Default.combobox6 = comboBox6.SelectedIndex;
            Properties.Settings.Default.combobox7 = comboBox7.SelectedIndex;
            Properties.Settings.Default.combobox8 = comboBox8.SelectedIndex;
            Properties.Settings.Default.combobox9 = comboBox9.SelectedIndex;
            Properties.Settings.Default.combobox10 = comboBox10.SelectedIndex;
            Properties.Settings.Default.combobox11 = comboBox11.SelectedIndex;
            Properties.Settings.Default.combobox12 = comboBox12.SelectedIndex;
            Properties.Settings.Default.combobox13 = comboBox13.SelectedIndex;
            Properties.Settings.Default.combobox14 = comboBox14.SelectedIndex;
            Properties.Settings.Default.combobox15 = comboBox15.SelectedIndex;
            Properties.Settings.Default.combobox16 = comboBox16.SelectedIndex;
            Properties.Settings.Default.combobox17 = comboBox17.SelectedIndex;
            Properties.Settings.Default.combobox18 = comboBox18.SelectedIndex;
            Properties.Settings.Default.combobox19 = comboBox19.SelectedIndex;
            Properties.Settings.Default.Save();
        }

        private void trackBar1_ValueChanged(object sender, EventArgs e)
        {
            Senddispinfo();
        }

        private void comboBox14_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void comboBox15_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void comboBox16_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void comboBox17_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void comboBox18_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void comboBox19_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void comboBox13_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void comboBox12_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void comboBox11_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void comboBox10_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void comboBox9_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void comboBox8_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void comboBox7_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void comboBox6_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void comboBox5_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void comboBox4_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void tabPage3_Click(object sender, EventArgs e)
        {

        }
    }

}
    
