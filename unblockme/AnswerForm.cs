using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace unblockme
{
    public partial class AnswerForm : Form
    {
        public AnswerForm()
        {
            InitializeComponent();
            d = board.Width / N;
            g = board.CreateGraphics();
            board.BackColor = Color.FromArgb(192, 64, 0);
        }
        const int N = 6;
        int w = 3, d;
        public int[, ,] answer;
        public int ansSize;
        public int index = -1;
        private Graphics g;
        private void AnswerForm_Load(object sender, EventArgs e) { }

        private void btnNext_Click(object sender, EventArgs e)
        {
            goRight();
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


        private void btnPrev_Click(object sender, EventArgs e)
        {
            goLeft();
        }

        private void goLeft()
        {
            if (index == 0)
                return;
            index--;
            drawState(index);
        }

        private void init()
        {
            board.BackColor = Color.FromArgb(192, 64, 0);
            for (int i = board.Width / N; i < board.Width; i += board.Width / N)
            {
                g.DrawLine(new Pen(new SolidBrush(Color.Black)), 0, i, board.Height, i);
                g.DrawLine(new Pen(new SolidBrush(Color.Black)), i, 0, i, board.Width);
            }
        }

        private void drawState(int i)
        {
            for (int j = 0; j < N; j++)
            {
                for (int k = 0; k < N; k++)
                {
                    int num = answer[i, j, k];
                    if (num == 0)
                    {
                        g.FillRectangle(new SolidBrush(Color.FromArgb(192, 64, 0)), k * d + w - 1, j * d + w - 1, d - 2 * w + 1, d - 2 * w + 1);
                    }
                    else
                    {
                        g.FillRectangle(Brushes.Orange, k * d + w, j * d + w, d - 2 * w, d - 2 * w);
                        g.DrawString(num.ToString(), new Font("Arial", 18, FontStyle.Regular), Brushes.Black, k * d + d / 4 + 2, j * d + d / 4 + 2);
                    }
                }
            }
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.KeyCode == Keys.A)
                goLeft();
            else if (e.KeyCode == Keys.D)
                goRight();

            base.OnKeyDown(e);
        }
    }
}
