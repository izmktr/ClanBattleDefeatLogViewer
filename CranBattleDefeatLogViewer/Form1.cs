using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace CranBattleDefeatLogViewer
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        List<DateTime> datelist = null;

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            var g = e.Graphics;


            if (datelist != null)
            {
                DrawGraph(g, datelist);
            }
            else
            {
                Font fnt = new Font("MS UI Gothic", 12);

                g.DrawString("行動ファイルをドラッグ＆ドロップしてください", fnt, Brushes.Black, 0, 0);
            }

        }

        private void DrawGraph(Graphics g, List<DateTime> datelist)
        {
            Brush[] brush =
            {
                Brushes.CornflowerBlue,
                Brushes.Aquamarine,
                Brushes.ForestGreen,
                Brushes.Gold,
                Brushes.OrangeRed,
            };

            var ox = 32;
            var y = 48;
            var start = datelist[0];
            var prevtime = new DateTime(start.Year, start.Month, start.Day, 5, 0, 0);
            var limittime = prevtime.AddDays(1);
            var starttime = prevtime;
            int boss = 0;
            Font fnt = new Font("MS UI Gothic", 12);

            for (int i = 0; i < 5; i++)
            {
                g.FillRectangle(brush[i], i * 48 + 32, 8, 48, 16);
                g.DrawString((i + 1).ToString(), fnt, Brushes.Black, i * 48 + 32 + 16, 8);
            }


            for (int i = 0; i < 25; i++)
            {
                int x = ox + i * 30;
                g.DrawLine(Pens.White, x, 32, x, 300);

                if (i < 24)
                {
                    g.DrawString(((i + 5) % 24).ToString(), fnt, Brushes.Black, x, 32);
                }

            }

            y += 16;
            g.DrawString(starttime.Day.ToString(), fnt, Brushes.Black, 4, y + 8);
            foreach (var dt in datelist)
            {
                int bindex = boss % 5;

                if (limittime <= dt)
                {
                    DrawBox(g, brush[bindex], ox, y, prevtime, starttime, limittime.AddMinutes(-1));
                    prevtime = limittime;
                    limittime = limittime.AddDays(1);
                    starttime = starttime.AddDays(1);
                    y += 48;

                    g.DrawString(starttime.Day.ToString(), fnt, Brushes.Black, 4, y + 8);
                }
                DrawBox(g, brush[bindex], ox, y, prevtime, starttime, dt);

                if (boss % 5 == 0)
                {
                    g.DrawString((boss / 5 + 1).ToString(), fnt, Brushes.Black, ox + Px(prevtime, starttime), y - 16);
                }

                prevtime = dt;

                boss++;
            }
        }

        private void DrawBox(Graphics g, Brush brush, int ox, int y, DateTime prevtime, DateTime starttime, DateTime dt)
        {
            int fx = Px(prevtime, starttime);
            var width = Px(dt, starttime) - fx;

            g.FillRectangle(brush, ox + fx, y, width, 32);
        }

        private int Px(DateTime dt, DateTime start)
        {
            var d = dt - start;
            return (int)(d.TotalSeconds / 60) / 2;
        }

        private void Form1_DragDrop(object sender, DragEventArgs e)
        {
            //コントロール内にドロップされたとき実行される
            //ドロップされたすべてのファイル名を取得する
            string[] fileName =
                (string[])e.Data.GetData(DataFormats.FileDrop, false);

            if (0 < fileName.Length)
            {
                List<DateTime> d = new List<DateTime>();
                using (var sm = new StreamReader(fileName[0]))
                {
                    while (0 <= sm.Peek())
                    {
                        var line = sm.ReadLine();
                        try
                        {
                            var date = DateTime.Parse(line);
                            d.Add(date);
                        }
                        catch (FormatException)
                        {

                        }
                    }
                }

                if (0 < d.Count)
                {
                    datelist = d;
                    var last = datelist.Last();
                    datelist.Add(new DateTime(last.Year, last.Month, last.Day, last.Hour < 5 ? 4 : 23, 59, 0));
                    this.Refresh();
                }
            }
        }

        private void Form1_DragEnter(object sender, DragEventArgs e)
        {
            //コントロール内にドラッグされたとき実行される
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                //ドラッグされたデータ形式を調べ、ファイルのときはコピーとする
                e.Effect = DragDropEffects.Copy;
            else
                //ファイル以外は受け付けない
                e.Effect = DragDropEffects.None;
        }
    }
}
