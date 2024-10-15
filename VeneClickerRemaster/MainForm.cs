using System;
using System.Drawing;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Threading;
using System.Diagnostics;
using WindowsInput;
using WindowsInput.Native;

namespace VeneClicker
{
    public partial class MainForm : Form
    {
        
        [DllImport("user32.dll")]
        static extern short GetAsyncKeyState(int vKey);

        [DllImport("user32.dll")]
        static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        static extern int GetWindowText(IntPtr hWnd, System.Text.StringBuilder text, int count);

        [DllImport("user32.dll")]
        static extern void mouse_event(uint dwFlags, uint dx, uint dy, uint cButtons, uint dwExtraInfo);

        private const int VK_LBUTTON = 0x01;
        private const int VK_RBUTTON = 0x02;
        private const uint MOUSEEVENTF_LEFTDOWN = 0x0002;
        private const uint MOUSEEVENTF_LEFTUP = 0x0004;
        private const uint MOUSEEVENTF_RIGHTDOWN = 0x0008;
        private const uint MOUSEEVENTF_RIGHTUP = 0x0010;

        private bool isRunning = true;
        private bool isEnabled = false;
        private Thread clickerThread;
        private int minCps = 5, maxCps = 15;
        private int rightMinCps = 5, rightMaxCps = 10;
        private bool onlyInMinecraft = true;
        private bool cpsDrops = false;
        private Keys toggleHotkey = Keys.Tab;
        private bool rightClickerEnabled = false;

        private RangeSlider cpsRangeSlider;
        private RangeSlider rightClickerSlider;
        private CheckBox rightClickerEnabledCheckBox;
        private Label rightClickerLabel;

        public MainForm()
        {
            InitializeComponent();
            this.FormBorderStyle = FormBorderStyle.None;
            this.BackColor = Color.FromArgb(40, 40, 40);
            this.ForeColor = Color.DodgerBlue;
            SetupCustomControls();

            toggleButton.Text = $"Hotkey: {toggleHotkey}";
            onlyMinecraftCheckBox.Checked = true;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            SetHighPriority();
        }
        private void SetupCustomControls()
        {
            // Set up custom title bar
            Panel titleBar = new Panel
            {
                Dock = DockStyle.Top,
                Height = 30,
                BackColor = Color.FromArgb(30, 30, 30)
            };
            this.Controls.Add(titleBar);

            Label titleLabel = new Label
            {
                Text = "VeneClicker",
                ForeColor = Color.DodgerBlue,
                Font = new Font("Arial", 12, FontStyle.Bold),
                Location = new Point(10, 5),
                AutoSize = true
            };
            titleBar.Controls.Add(titleLabel);

            rightClickerSlider = new RangeSlider
            {
                Location = new Point(12, 180),
                Size = new Size(200, 20),
                Minimum = 1,
                Maximum = 20,
                LowerValue = 5,
                UpperValue = 10
            };
            rightClickerSlider.ValuesChanged += RightClickerSlider_ValuesChanged;
            this.Controls.Add(rightClickerSlider);

            // Right Clicker Label
            rightClickerLabel = new Label
            {
                Location = new Point(12, 160),
                Size = new Size(200, 20),
                Text = "Right CPS Range: 5-10",
                ForeColor = Color.DodgerBlue
            };
            this.Controls.Add(rightClickerLabel);

            // Right Clicker Enabled Checkbox
            rightClickerEnabledCheckBox = new CheckBox
            {
                Location = new Point(12, this.ClientSize.Height - 55), // Adjust the Y-coordinate as needed
                Size = new Size(150, 20),
                Text = "Enable Right Clicker",
                ForeColor = Color.DodgerBlue,
                Anchor = AnchorStyles.Bottom | AnchorStyles.Left // This ensures it stays at the bottom left when the form is resized
            };
            rightClickerEnabledCheckBox.CheckedChanged += RightClickerEnabledCheckBox_CheckedChanged;
            this.Controls.Add(rightClickerEnabledCheckBox);




            // Add close button
            Button closeButton = new Button
            {
                Text = "×",
                FlatStyle = FlatStyle.Flat,
                FlatAppearance = { BorderSize = 0 },
                Size = new Size(30, 30),
                Location = new Point(this.Width - 30, 0),
                BackColor = Color.FromArgb(30, 30, 30),
                ForeColor = Color.DodgerBlue,
                Font = new Font("Arial", 15, FontStyle.Bold),
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };
            closeButton.Click += (sender, e) => this.Close();
            titleBar.Controls.Add(closeButton);

            // Make entire form draggable
            this.MouseDown += Form_MouseDown;
            this.MouseMove += Form_MouseMove;

            // Update toggle button
            // Update toggle button
            toggleButton.Text = $"Hotkey: {toggleHotkey}";
            toggleButton.Click += toggleButton_Click;
            toggleButton.Height = 30; // Reduce height
            toggleButton.FlatStyle = FlatStyle.Flat;
            toggleButton.FlatAppearance.BorderColor = Color.DodgerBlue;
            toggleButton.ForeColor = Color.DodgerBlue;

            // Add RangeSlider
            cpsRangeSlider = new RangeSlider
            {
                Location = new Point(12, 126),
                Size = new Size(200, 10), // Reduced height from 30 to 20
                Minimum = 5,
                Maximum = 25,
                LowerValue = 5,
                UpperValue = 15
            };
            cpsRangeSlider.ValuesChanged += CpsRangeSlider_ValuesChanged;
            this.Controls.Add(cpsRangeSlider);

            UpdateCpsRangeLabel();


            // Update checkboxes
            onlyMinecraftCheckBox.ForeColor = Color.DodgerBlue;
            cpsDropsCheckBox.ForeColor = Color.DodgerBlue;

            // Start the clicker thread
            clickerThread = new Thread(ClickerLoop);
            clickerThread.Start();
        }

        private void RightClickerSlider_ValuesChanged(object sender, EventArgs e)
        {
            rightMinCps = rightClickerSlider.LowerValue;
            rightMaxCps = rightClickerSlider.UpperValue;
            UpdateRightClickerLabel();
        }

        private void UpdateRightClickerLabel()
        {
            rightClickerLabel.Text = $"Right CPS: {rightMinCps}-{rightMaxCps}";
        }

        private void RightClickerEnabledCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            rightClickerEnabled = rightClickerEnabledCheckBox.Checked;
        }

        private Point lastPoint;

        private void Form_MouseDown(object sender, MouseEventArgs e)
        {
            lastPoint = new Point(e.X, e.Y);
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);
            isRunning = false;
            if (clickerThread != null && clickerThread.IsAlive)
            {
                clickerThread.Join(1000); // Wait up to 1 second for the thread to finish
                if (clickerThread.IsAlive)
                {
                    clickerThread.Abort(); // Force thread to stop if it doesn't finish in time
                }
            }
        }
        private void Form_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                this.Left += e.X - lastPoint.X;
                this.Top += e.Y - lastPoint.Y;
            }
        }

        private void toggleButton_Click(object sender, EventArgs e)
        {
            toggleButton.Text = "Press a key...";
            toggleButton.Enabled = false;
            this.KeyPreview = true;
            this.KeyPress += MainForm_KeyPress;
        }

        private void MainForm_KeyPress(object sender, KeyPressEventArgs e)
        {
            toggleHotkey = (Keys)char.ToUpper(e.KeyChar);
            toggleButton.Text = $"Hotkey: {toggleHotkey}";
            toggleButton.Enabled = true;
            this.KeyPreview = false;
            this.KeyPress -= MainForm_KeyPress;
        }

        private void CpsRangeSlider_ValuesChanged(object sender, EventArgs e)
        {
            minCps = cpsRangeSlider.LowerValue;
            maxCps = cpsRangeSlider.UpperValue;
            UpdateCpsRangeLabel();
        }
        private void UpdateCpsRange(int min, int max, bool isRightClick = false)
        {
            if (isRightClick)
            {
                rightMinCps = min;
                rightMaxCps = max;
            }
            else
            {
                minCps = min;
                maxCps = max;
            }
            Console.WriteLine($"Updated CPS Range - Left: {minCps}-{maxCps}, Right: {rightMinCps}-{rightMaxCps}");
        }
        private void UpdateCpsRangeLabel()
        {
            cpsRangeLabel.Text = $"CPS Range: {cpsRangeSlider.LowerValue}-{cpsRangeSlider.UpperValue}";
        }

        private void ClickerLoop()
        {
            Random random = new Random();
            bool[] lastMouseState = { false, false };
            bool lastHotkeyState = false;
            Stopwatch[] stopwatches = { new Stopwatch(), new Stopwatch() };
            double[] currentCps = { 0, 0 };
            int[] clickCounter = { 0, 0 };
            int[] dropCounter = { 0, 0 };

            InputSimulator inputSimulator = new InputSimulator();

            while (true)
            {
                bool[] currentMouseState = {
            (GetAsyncKeyState(VK_LBUTTON) & 0x8000) != 0,
            (GetAsyncKeyState(VK_RBUTTON) & 0x8000) != 0
        };
                bool currentHotkeyState = (GetAsyncKeyState((int)toggleHotkey) & 0x8000) != 0;

                if (currentHotkeyState && !lastHotkeyState)
                {
                    this.Invoke((MethodInvoker)delegate { ToggleClicker(); });
                }
                lastHotkeyState = currentHotkeyState;

                for (int i = 0; i < 2; i++)
                {
                    bool isClickerEnabled = i == 0 ? isEnabled : rightClickerEnabled;

                    if (!currentMouseState[i])
                    {
                        if (lastMouseState[i])
                        {
                            // Mouse button just released
                            if (i == 0)
                                inputSimulator.Mouse.LeftButtonUp();
                            else
                                inputSimulator.Mouse.RightButtonUp();

                            stopwatches[i].Reset();
                            currentCps[i] = 0;
                            clickCounter[i] = 0;
                            dropCounter[i] = 0;
                        }
                        lastMouseState[i] = false;
                        continue;
                    }

                    if (!isClickerEnabled || (onlyInMinecraft && !IsMinecraftActive()))
                    {
                        lastMouseState[i] = currentMouseState[i];
                        continue;
                    }

                    if (!lastMouseState[i])
                    {
                        // Mouse button just pressed
                        stopwatches[i].Restart();
                        currentCps[i] = 0;
                        clickCounter[i] = 0;
                        dropCounter[i] = 0;
                    }

                    int minCpsForButton = i == 0 ? minCps : rightMinCps;
                    int maxCpsForButton = i == 0 ? maxCps : rightMaxCps;

                    if (cpsDrops)
                    {
                        clickCounter[i]++;
                        if (clickCounter[i] >= random.Next(10, 16))
                        {
                            dropCounter[i] = random.Next(3, 6);
                            clickCounter[i] = 0;
                        }

                        if (dropCounter[i] > 0)
                        {
                            currentCps[i] = minCpsForButton + (maxCpsForButton - minCpsForButton) * 0.3;
                            dropCounter[i]--;
                        }
                        else
                        {
                            currentCps[i] = random.NextDouble() * (maxCpsForButton - minCpsForButton) + minCpsForButton;
                        }
                    }
                    else
                    {
                        currentCps[i] = random.NextDouble() * (maxCpsForButton - minCpsForButton) + minCpsForButton;
                    }

                    double clickInterval = 1000.0 / currentCps[i];
                    clickInterval *= 1 + (random.NextDouble() * 2 - 1) * 0.15; // 15% variation

                    if (stopwatches[i].ElapsedMilliseconds >= clickInterval)
                    {
                        // Recheck mouse state before clicking
                        if ((GetAsyncKeyState(i == 0 ? VK_LBUTTON : VK_RBUTTON) & 0x8000) != 0)
                        {
                            Thread.Sleep(random.Next(1, 5));

                            if (i == 0)
                            {
                                inputSimulator.Mouse.LeftButtonUp();
                                inputSimulator.Mouse.LeftButtonDown();
                            }
                            else
                            {
                                inputSimulator.Mouse.RightButtonUp();
                                inputSimulator.Mouse.RightButtonDown();
                            }

                            stopwatches[i].Restart();

                            if (random.NextDouble() < 0.01)
                            {
                                Thread.Sleep(random.Next(50, 150));
                            }
                        }
                        else
                        {
                            // Mouse button released during interval, reset state
                            stopwatches[i].Reset();
                            currentCps[i] = 0;
                            clickCounter[i] = 0;
                            dropCounter[i] = 0;
                        }
                    }

                    lastMouseState[i] = currentMouseState[i];
                }

                Thread.Sleep(1);
            }
        }
        private void SetHighPriority()
        {
            this.BeginInvoke(new Action(() =>
            {
                try
                {
                    using (Process process = Process.GetCurrentProcess())
                    {
                        process.PriorityClass = ProcessPriorityClass.High;
                    }
                }
                catch (Exception ex)
                {
                    // Log the error or show a message to the user
                    Console.WriteLine($"Failed to set high priority: {ex.Message}");
                }
            }));
        }
        private void ToggleClicker()
        {
            isEnabled = !isEnabled;
            toggleButton.Text = isEnabled ? $"Hotkey: {toggleHotkey} (On)" : $"Hotkey: {toggleHotkey} (Off)";
            toggleButton.BackColor = isEnabled ? Color.Red : Color.Green;
        }

      

        private bool IsMinecraftActive()
        {
            IntPtr handle = GetForegroundWindow();
            System.Text.StringBuilder sb = new System.Text.StringBuilder(256);
            GetWindowText(handle, sb, 256);
            string title = sb.ToString().ToLower();
            return title.Contains("minecraft");
        }

        private void onlyMinecraftCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            onlyInMinecraft = onlyMinecraftCheckBox.Checked;
        }

        private void cpsDropsCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            cpsDrops = cpsDropsCheckBox.Checked;
        }
    }

    public class RangeSlider : Control
    {
        public event EventHandler ValuesChanged;

        private int _minimum = 0;
        private int _maximum = 100;
        private int _lowerValue = 0;
        private int _upperValue = 100;
        private Rectangle _lowerHandle, _upperHandle;
        private bool _isDraggingLower, _isDraggingUpper;
        private const int HandleSize = 12;

        public int Minimum
        {
            get => _minimum;
            set { _minimum = value; Invalidate(); }
        }

        public int Maximum
        {
            get => _maximum;
            set { _maximum = value; Invalidate(); }
        }

        public int LowerValue
        {
            get => _lowerValue;
            set { _lowerValue = Math.Max(Minimum, Math.Min(value, UpperValue)); Invalidate(); }
        }

        public int UpperValue
        {
            get => _upperValue;
            set { _upperValue = Math.Min(Maximum, Math.Max(value, LowerValue)); Invalidate(); }
        }

        public RangeSlider()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.DoubleBuffer, true);
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            this.Height = Math.Max(this.Height, HandleSize + 4); // Ensure minimum height for handles
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            var g = e.Graphics;

            int barHeight = Height / 3;
            int barY = (Height - barHeight) / 2;

            // Calculate the drawable width (accounting for handle size)
            int drawableWidth = Width - HandleSize;
            int barLeft = HandleSize / 2;

            // Draw background
            g.FillRectangle(Brushes.DarkGray, barLeft, barY, drawableWidth, barHeight);

            // Draw selected range
            int range = Maximum - Minimum;
            int lowerPos = barLeft + (LowerValue - Minimum) * drawableWidth / range;
            int upperPos = barLeft + (UpperValue - Minimum) * drawableWidth / range;
            g.FillRectangle(Brushes.DodgerBlue, lowerPos, barY, upperPos - lowerPos, barHeight);

            // Draw handles
            int handleY = (Height - HandleSize) / 2;
            _lowerHandle = new Rectangle(lowerPos - HandleSize / 2, handleY, HandleSize, HandleSize);
            _upperHandle = new Rectangle(upperPos - HandleSize / 2, handleY, HandleSize, HandleSize);

            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            g.FillEllipse(Brushes.White, _lowerHandle);
            g.FillEllipse(Brushes.White, _upperHandle);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            if (_lowerHandle.Contains(e.Location)) _isDraggingLower = true;
            else if (_upperHandle.Contains(e.Location)) _isDraggingUpper = true;
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (_isDraggingLower || _isDraggingUpper)
            {
                int range = Maximum - Minimum;
                int drawableWidth = Width - HandleSize;
                int value = Minimum + (e.X - HandleSize / 2) * range / drawableWidth;
                value = Math.Max(Minimum, Math.Min(value, Maximum));

                if (_isDraggingLower) LowerValue = value;
                else UpperValue = value;

                ValuesChanged?.Invoke(this, EventArgs.Empty);
                Invalidate();
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            _isDraggingLower = _isDraggingUpper = false;
        }
    }

    internal static class NativeMethods
    {
        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern bool ReleaseCapture();
    }
}