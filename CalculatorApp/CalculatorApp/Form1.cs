using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AdvancedMath;   // DLL 1
using StatsLibrary;   // DLL 2



namespace CalculatorApp
{
    public partial class Form1 : Form
    {
        private TextBox display;
        private TextBox txtList;
        private Label lblStats;
        private Label exprLabel;          // shows live expression (e.g., "12 + 7 ×")

        // calculator state
        private double? accumulator = null;
        private string pendingOp = null;
        private bool newEntry = true;     // start a new number after an op/equals
        private bool lastWasEquals = false;
        private string expression = "";
        private double memory = 0;

        public Form1()
        {
            InitializeComponent();
            BuildUI();
        }

        private void BuildUI()
        {
            // ---- window & scaling ----
            AutoScaleMode = AutoScaleMode.Dpi;
            Font = new System.Drawing.Font("Segoe UI", 9F);
            Text = "CalculatorApp";                        // remove "(DLL demo)"
            StartPosition = FormStartPosition.CenterScreen;
            ClientSize = new System.Drawing.Size(560, 640); // wider & taller to fit everything
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;

            // ---- root layout (expr, display, keypad, stats) ----
            var root = new TableLayoutPanel { Dock = DockStyle.Fill, RowCount = 4, ColumnCount = 1, Padding = new Padding(8) };
            root.RowStyles.Add(new RowStyle(SizeType.AutoSize));     // expression label
            root.RowStyles.Add(new RowStyle(SizeType.AutoSize));     // main display
            root.RowStyles.Add(new RowStyle(SizeType.Percent, 100)); // keypad
            root.RowStyles.Add(new RowStyle(SizeType.AutoSize));     // stats strip
            Controls.Add(root);

            // expression label (right-aligned, small)
            exprLabel = new Label
            {
                Text = "",
                AutoSize = false,
                Height = 26,
                Dock = DockStyle.Top,
                TextAlign = System.Drawing.ContentAlignment.MiddleRight,
                ForeColor = System.Drawing.Color.DimGray
            };
            root.Controls.Add(exprLabel, 0, 0);

            // main display
            display = new TextBox
            {
                ReadOnly = true,
                Text = "0",
                TextAlign = HorizontalAlignment.Right,
                Font = new System.Drawing.Font("Segoe UI", 20, System.Drawing.FontStyle.Bold),
                Dock = DockStyle.Top
            };
            root.Controls.Add(display, 0, 1);

            // keypad (6x6)
            string[,] keys = new string[,]
            {
                { "MC","MR","M+","M-","C","⌫" },
                { "sin","cos","tan","x²","√","1/x" },
                { "7","8","9","÷","%","^" },
                { "4","5","6","×","log","ln" },
                { "1","2","3","-","x!","+/-" },
                { "0",".","π","+","=","Exp" }
            };

            var grid = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 6, RowCount = 6 };
            for (int c = 0; c < 6; c++) grid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 16.66f));
            for (int r = 0; r < 6; r++) grid.RowStyles.Add(new RowStyle(SizeType.Percent, 16.66f));
            root.Controls.Add(grid, 0, 2);

            for (int r = 0; r < keys.GetLength(0); r++)
            {
                for (int c = 0; c < keys.GetLength(1); c++)
                {
                    var b = new Button
                    {
                        Text = keys[r, c],
                        Dock = DockStyle.Fill,
                        TabStop = false,
                        Margin = new Padding(4),
                        Font = new System.Drawing.Font(Font, System.Drawing.FontStyle.Bold) // bold text
                    };
                    b.Click += ButtonClick;
                    grid.Controls.Add(b, c, r);
                }
            }

            // stats strip (uses StatsLibrary)
            var stats = new TableLayoutPanel { Dock = DockStyle.Bottom, ColumnCount = 5, AutoSize = true, Padding = new Padding(0, 8, 0, 0) };
            stats.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 40));
            stats.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 15));
            stats.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 15));
            stats.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 15));
            stats.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 15));

            lblStats = new Label { Text = "Stats:", AutoSize = true, Dock = DockStyle.Fill };
            txtList = new TextBox { Dock = DockStyle.Fill, Text = "1,2,3,4.5" };
            txtList.GotFocus += (_, __) => { if (txtList.Text == "1,2,3,4.5") txtList.Clear(); };
            txtList.LostFocus += (_, __) => { if (string.IsNullOrWhiteSpace(txtList.Text)) txtList.Text = "1,2,3,4.5"; };

            var btnMean = new Button { Text = "Mean", Dock = DockStyle.Fill };
            var btnMed = new Button { Text = "Median", Dock = DockStyle.Fill };
            var btnStd = new Button { Text = "Std Dev", Dock = DockStyle.Fill };
            btnMean.Click += (_, __) => ComputeStats(Stats.Mean);
            btnMed.Click += (_, __) => ComputeStats(Stats.Median);
            btnStd.Click += (_, __) => ComputeStats(Stats.StdDev);

            stats.Controls.Add(lblStats, 0, 0);
            stats.Controls.Add(txtList, 0, 1);
            stats.SetColumnSpan(txtList, 1);
            stats.Controls.Add(btnMean, 1, 1);
            stats.Controls.Add(btnMed, 2, 1);
            stats.Controls.Add(btnStd, 3, 1);
            root.Controls.Add(stats, 0, 3);
        }

        // ========= Calculator behavior =========
        double DisplayValue
        {
            get { double.TryParse(display.Text, out var v); return v; }
            set { display.Text = double.IsNaN(value) || double.IsInfinity(value) ? "Error" : value.ToString(); }
        }

        private void SetExpr(string text) => exprLabel.Text = text;

        private void ButtonClick(object sender, EventArgs e)
        {
            var key = ((Button)sender).Text;

            // digits & dot
            if (char.IsDigit(key[0]) || key == ".")
            {
                if (newEntry || display.Text == "0" || lastWasEquals)
                {
                    if (lastWasEquals) { expression = ""; SetExpr(""); lastWasEquals = false; }
                    display.Text = (key == ".") ? "0." : key;
                    newEntry = false;
                }
                else if (!(key == "." && display.Text.Contains(".")))
                {
                    display.Text += key;
                }
                return;
            }

            switch (key)
            {
                case "C":
                    accumulator = null; pendingOp = null; DisplayValue = 0;
                    newEntry = true; lastWasEquals = false; expression = ""; SetExpr("");
                    break;

                case "⌫":
                    if (!newEntry && display.Text.Length > 0)
                    {
                        display.Text = display.Text.Substring(0, display.Text.Length - 1);
                        if (display.Text == "" || display.Text == "-") display.Text = "0";
                    }
                    break;

                case "+/-": DisplayValue = Adv.Negate(DisplayValue); break;
                case "π": DisplayValue = Math.PI; newEntry = true; break;

                // memory
                case "MC": memory = 0; break;
                case "MR": DisplayValue = memory; newEntry = true; break;
                case "M+": memory += DisplayValue; newEntry = true; break;
                case "M-": memory -= DisplayValue; newEntry = true; break;

                // unary ops
                case "x²": DisplayValue = Adv.Square(DisplayValue); newEntry = true; break;
                case "√": DisplayValue = Adv.Sqrt(DisplayValue); newEntry = true; break;
                case "1/x": DisplayValue = Adv.Reciprocal(DisplayValue); newEntry = true; break;
                case "log": DisplayValue = Adv.Log10(DisplayValue); newEntry = true; break;
                case "ln": DisplayValue = Adv.Ln(DisplayValue); newEntry = true; break;
                case "sin": DisplayValue = Adv.SinDeg(DisplayValue); newEntry = true; break;
                case "cos": DisplayValue = Adv.CosDeg(DisplayValue); newEntry = true; break;
                case "tan": DisplayValue = Adv.TanDeg(DisplayValue); newEntry = true; break;
                case "x!": DisplayValue = Adv.Factorial(DisplayValue); newEntry = true; break;
                case "Exp": DisplayValue = Adv.Exp(DisplayValue); newEntry = true; break;

                // binary ops
                case "+":
                case "-":
                case "×":
                case "÷":
                case "^":
                case "%":
                    ApplyPendingThenSet(key);
                    break;

                case "=":
                    if (pendingOp != null)
                    {
                        string before = $"{expression} {display.Text} =";
                        ApplyPending();
                        SetExpr(before);           // keep full expression visible
                        pendingOp = null;
                        lastWasEquals = true;
                    }
                    break;
            }
        }

        private void ApplyPendingThenSet(string newOp)
        {
            if (pendingOp == null)
            {
                accumulator = DisplayValue;
                expression = $"{display.Text} {newOp}";
            }
            else
            {
                ApplyPending(); // updates accumulator
                expression = $"{accumulator} {newOp}";
            }
            SetExpr(expression);
            pendingOp = newOp;
            newEntry = true;
        }

        private void ApplyPending()
        {
            if (pendingOp == null || accumulator == null) return;
            double a = accumulator.Value, b = DisplayValue, r = 0;

            switch (pendingOp)
            {
                case "+": r = Adv.Add(a, b); break;
                case "-": r = Adv.Subtract(a, b); break;
                case "×": r = Adv.Multiply(a, b); break;
                case "÷": r = Adv.Divide(a, b); break;
                case "^": r = Adv.Pow(a, b); break;
                case "%": r = Adv.PercentOf(a, b); break; // "b % of a"
            }

            DisplayValue = r;
            accumulator = r;
            newEntry = true;
        }

        // ========= Stats strip =========
        private void ComputeStats(Func<System.Collections.Generic.IEnumerable<double>, double> f)
        {
            try
            {
                var nums = txtList.Text.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                                       .Select(s => double.Parse(s.Trim()));
                double val = f(nums);
                lblStats.Text = "Stats: " + (double.IsNaN(val) ? "Error" : val.ToString());
            }
            catch { lblStats.Text = "Stats: Error"; }
        }
    }
}

