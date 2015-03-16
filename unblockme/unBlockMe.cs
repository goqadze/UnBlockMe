using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;
using System.Threading;

namespace unblockme
{
    public partial class unBlockMe : Form
    {
        public unBlockMe()
        {
            InitializeComponent();
            
            inputBoard = new int[N, N];
            d = board.Width / N;
            bitMap = new Bitmap(board.Width, board.Height);
            g = Graphics.FromImage(bitMap);
        }

        Graphics g;
        Bitmap bitMap;
        const int N = 6;
        int[,] inputBoard;
        int d;
        int w = 3;
        int red, redX = 0;
        public int[, ,] answer;
        public int ansSize;
        public int index = -1;

        private void plankNumber_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void unBlockMe_Load(object sender, EventArgs e)
        {
            Reset();
            this.btnGoToSolve.Visible = false;
        }

        private void Reset(bool msgBox = false)
        {
            if (msgBox && MessageBox.Show("Sure?", "", MessageBoxButtons.OKCancel, MessageBoxIcon.Information) != System.Windows.Forms.DialogResult.OK)
                return;
            for (int i = 0; i < N; i++)
                for (int j = 0; j < N; j++)
                    inputBoard[i, j] = 0;
            this.plankNumber.Visible = true;
            this.btnGoToSolve.Visible = true;
            this.btnPrev.Visible = false;
            this.btnNext.Visible = false;
            index = -1;
            init();
            board.Image = bitMap;
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            board.Enabled = true;
            plankNumber.Enabled = true;
            init();
            board.Image = bitMap;
            btnStart.Visible = false;
            btnGoToSolve.Visible = true;
            btnReset.Visible = true;
        }

        private void init()
        {
            g.FillRectangle(new SolidBrush(Color.FromArgb(192, 64, 0)), 0, 0, board.Width, board.Height);
            for (int i = board.Width / N; i < board.Width; i += board.Width / N)
            {
                g.DrawLine(new Pen(new SolidBrush(Color.Black), 2), 0, i, board.Height, i);
                g.DrawLine(new Pen(new SolidBrush(Color.Black)), i, 0, i, board.Width);
            }
        }

        private void board_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(plankNumber.Text))
            {
                var ee = (MouseEventArgs)e;
                int x = ee.X;
                int y = ee.Y;

                x = x / d * d;
                y = y / d * d;

                int num = int.Parse(plankNumber.Text);
                if (inputBoard[x / d, y / d] > 0 && inputBoard[x / d, y / d] != num)
                    return;

                if (inputBoard[x / d, y / d] > 0 && inputBoard[x / d, y / d] == num)
                {
                    inputBoard[x / d, y / d] = 0;
                    g.FillRectangle(new SolidBrush(Color.FromArgb(192, 64, 0)), x + w - 1, y + w - 1, d - 2 * w + 1, d - 2 * w + 1);
                    board.Image = bitMap;
                    return;
                }

                inputBoard[x / d, y / d] = num;
                g.FillRectangle(Brushes.Orange, x + w, y + w, d - 2 * w, d - 2 * w);
                g.DrawString(plankNumber.Text, new Font("Arial", 18, FontStyle.Regular), Brushes.Black, x + d / 4 + 2, y + d / 4 + 2);
                board.Image = bitMap;
            }
        }

        private void btnGoToSolve_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Sure?", "", MessageBoxButtons.OKCancel, MessageBoxIcon.Information) != System.Windows.Forms.DialogResult.OK)
                return;

            StreamWriter sw = new StreamWriter("input.txt");
            red = 0;
            for (int i = 0; i < N; i++)
            {
                for (int j = 0; j < N; j++)
                {
                    sw.Write(inputBoard[j, i] + " ");
                    if (inputBoard[j, i] == red) redX = i;
                    red = Math.Max(red, inputBoard[j, i]);
                }
                sw.Write("\n");
            }

            for (int i = 0; i < N; i++)
            {
                for (int j = 0; j < N; j++)
                {
                    if (inputBoard[i, j] == red)
                    {
                        int x = i * d;
                        int y = j * d;
                        g.FillRectangle(Brushes.Red, x + w, y + w, d - 2 * w, d - 2 * w);
                        g.DrawString(plankNumber.Text, new Font("Arial", 18, FontStyle.Regular), Brushes.Black, x + d / 4 + 2, y + d / 4 + 2);
                    }
                }
            }

            sw.Close();

            this.btnGoToSolve.Visible = false;
            
            // source -- main.cpp
            Process.Start("Code.exe");
            
            board.Image = bitMap;
            
            Thread.Sleep(5000);
            
            StreamReader sr = new StreamReader("output.txt");
            ansSize = int.Parse(sr.ReadLine());
            if (ansSize == 0)
            {
                MessageBox.Show("Error", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            answer = new int[ansSize, N, N];
            for (int i = 0; i < ansSize; i++)
            {
                for (int j = 0; j < N; j++)
                {
                    string[] s = sr.ReadLine().Split();
                    for (int k = 0; k < N; k++)
                    {
                        int num = int.Parse(s[k]);
                        answer[i, j, k] = num;
                    }
                }
                sr.ReadLine();
            }

            this.plankNumber.Text = string.Empty;
            this.plankNumber.Visible = false;
            this.btnPrev.Enabled = false;
            this.btnPrev.Visible = true;
            this.btnNext.Visible = true;
        }

        private void btnPrev_Click(object sender, EventArgs e)
        {
            goLeft();
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            goRight();
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            Reset(true);
        }

        private void goRight()
        {
            if (index == ansSize - 1)
                return;
            if (!this.btnPrev.Enabled)
            {
                this.btnPrev.Enabled = true;
                init();
            }
            index++;
            drawState(index);
        }

        private void goLeft()
        {
            if (index == 0)
                return;
            index--;
            drawState(index);
        }

        private void drawState(int k)
        {
            init();
            int vis = 0;
            for (int i = 0; i < N; i++)
            {
                for (int j = 0; j < N; j++)
                {
                    int num = answer[k, i, j];
                    if ((vis & (1 << num)) == 1) continue;
                    int sx = 0, sy = 0, dx = 0, dy = 0;
                    if (num > 0)
                    {
                        int ii = i, jj = j;
                        if (j > 0 && j < N - 1 && (answer[k, ii, jj - 1] == num || answer[k, ii, jj + 1] == num))
                        {
                            vis |= 1 << num;
                            while (jj > 0 && answer[k, ii, jj - 1] == num)
                                jj--;

                            sx = ii;
                            sy = jj;
                            while (jj < N && answer[k, ii, jj] == num)
                                jj++;

                            dx = 1;
                            dy = jj - sy;
                        }
                        else if (i > 0 && i < N - 1 && (answer[k, ii - 1, jj] == num || answer[k, ii + 1, jj] == num))
                        {
                            vis |= 1 << num;
                            while (ii > 0 && answer[k, ii - 1, jj] == num)
                                ii--;
                            sx = ii;
                            sy = jj;
                            while (ii < N && answer[k, ii, jj] == num)
                                ii++;
                            dx = ii - sx;
                            dy = 1;
                        }
                        Brush brush = num == red ? Brushes.Red : Brushes.Orange;
                        g.FillRectangle(brush, sy * d + w, sx * d + w, dy * d - 2 * w, dx * d - 2 * w);
                        //g.DrawString(num.ToString(), new Font("Arial", 18, FontStyle.Regular), Brushes.Black, k * d + d / 4 + 2, j * d + d / 4 + 2);
                    }
                }
            }
            if (k == ansSize - 1)
                g.DrawLine(new Pen(new SolidBrush(Color.Green), 3), 0, redX * d + d / 2, board.Width, redX * d + d / 2);
            board.Image = bitMap;
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.KeyCode == Keys.A)
                goLeft();
            else if (e.KeyCode == Keys.D)
                goRight();
            else if (e.KeyCode == Keys.R)
                Reset(true);

            base.OnKeyDown(e);
        }


    }
}
