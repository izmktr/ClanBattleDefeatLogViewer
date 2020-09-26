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

        List<DateTime> defeatlist = null;
        List<DateTime> attacklist = null;

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
        }

        IEnumerable<DateTime> LastAdd(List<DateTime> datelist)
        {
            foreach (var item in datelist)
            {
                yield return item;
            }
            if (0 < datelist.Count)
            {
                var last = datelist.Last();
                if (last.Hour < 5)
                {
                    yield return new DateTime(last.Year, last.Month, last.Day, 4, 59, 59);
                }
                else
                {
                    yield return new DateTime(last.Year, last.Month, last.Day, 23, 59, 59);
                }
            }
        }

        private void DrawBossGraph(Graphics g, List<DateTime> datelist)
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
            foreach (var dt in LastAdd(datelist))
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

        IEnumerable<List<Point>> DayData(List<DateTime> datelist)
        {
            var start = datelist[0];
            var prevtime = new DateTime(start.Year, start.Month, start.Day, 5, 0, 0);
            var limittime = prevtime.AddDays(1);
            var starttime = prevtime;

            int attacknum = 0;
            int day = 0;

            List<Point> result = new List<Point>();
            result.Add(new Point(0, 0));

            foreach (var dt in datelist)
            {
                if (limittime <= dt)
                {
                    result.Add(new Point( Px(starttime.AddDays(1), starttime), attacknum - 1));
                    yield return result;

                    result.Clear();
                    prevtime = limittime;
                    limittime = limittime.AddDays(1);
                    starttime = starttime.AddDays(1);
                    attacknum = 0;
                    day++;

                    result.Add(new Point(0, 0));
                }

                result.Add(new Point(Px(dt, starttime), attacknum));

                prevtime = dt;
                attacknum++;
            }
            yield return result;
        }



        IEnumerable<List<PointF>> AttackSpeedList(List<DateTime> datelist)
        {
            foreach (var daylist in DayData(datelist))
            {
                var list = new List<PointF>();

                int left = 0;
                int right = 0;

                var lastx = daylist.Last().X;
                for(int i = 0; i < lastx; i++)
                {
                    float lvalue = CalcValue(daylist, ref left, i - 15);
                    float rvalue = CalcValue(daylist, ref right, i + 15);

                    list.Add(new PointF(i, rvalue - lvalue));
                }

                yield return list;
            }
        }

        private static float CalcValue(List<Point> daylist, ref int index, int xvalue)
        {
            for (; index < daylist.Count; index++)
            {
                if (xvalue < daylist[index].X) break;
            }
            if (daylist.Count <= index)
            {
                return (float) daylist[daylist.Count - 1].Y;
            }

            if (0 < index)
            {
                var prev = daylist[index - 1];
                var now = daylist[index];
                float rate = (xvalue * 1.0f - prev.X) / (now.X - prev.X);
                return prev.Y * (1 - rate) + now.Y * rate;
            }
            else
            {
                return (float)daylist[0].Y;
            }

        }

        private void DrawAttackGraph(Graphics g, List<DateTime> datelist)
        {
            Brush[] brush =
            {
                Brushes.CornflowerBlue,
                Brushes.Aquamarine,
                Brushes.ForestGreen,
                Brushes.Gold,
                Brushes.OrangeRed,
                Brushes.Red,
            };
            Color[] colors =
            {
                Color.CornflowerBlue,
                Color.Aquamarine,
                Color.ForestGreen,
                Color.Gold,
                Color.OrangeRed,
                Color.Red,
            };

            var ox = 32;
            var y = 48;
            var start = datelist[0];
            var prevtime = new DateTime(start.Year, start.Month, start.Day, 5, 0, 0);
            var limittime = prevtime.AddDays(1);
            var starttime = prevtime;
            Font fnt = new Font("MS UI Gothic", 12);

            const int attackheight = 3;

            var dispday = start;
            for (int i = 0; i < 5; i++)
            {
                g.FillRectangle(brush[i], i * 48 + 32, 8, 48, 16);
                g.DrawString($"{dispday.Month}/{dispday.Day}", fnt, Brushes.Black, i * 48 + 40, 8);
                dispday = dispday.AddDays(1);
            }


            for (int i = 0; i < 25; i++)
            {
                int x = ox + i * 30;
                g.DrawLine(Pens.White, x, 32, x, 32 + 300);

                g.DrawString(((i + 5) % 24).ToString(), fnt, Brushes.Black, x, 32);
            }

            for (int i = 0; i < 10; i++)
            {
                int ly = y + i * attackheight * 10 + 16;
                g.DrawLine(Pens.White, 0, ly, 25 * 30+ 32, ly);

                g.DrawString(((9 - i) * 10).ToString(), fnt, Brushes.Black, 4, ly - 16);

            }

            var day = 0;
            foreach (var pointrawlist in DayData(datelist))
            {
                using (var pen = new Pen(colors[day], 2))
                {
                    var drawline = pointrawlist.Select(n => new Point(ox + n.X, y + 16 + (90 - n.Y) * attackheight)).ToArray();
                    g.DrawLines(pen, drawline);

//                    var liney = y + (90 - attacknum) * attackheight + 16;
//                    g.DrawLine(pen[day], ox + Px(prevtime, starttime), liney, ox + Px(dt, starttime), liney - attackheight);
                }

                day++;
            }
        }

        private void DrawAttackSpeedGraph(Graphics g, List<DateTime> datelist)
        {
            Brush[] brush =
            {
                Brushes.CornflowerBlue,
                Brushes.Aquamarine,
                Brushes.ForestGreen,
                Brushes.Gold,
                Brushes.OrangeRed,
                Brushes.Red,
            };
            Color[] colors =
            {
                Color.CornflowerBlue,
                Color.Aquamarine,
                Color.ForestGreen,
                Color.Gold,
                Color.OrangeRed,
                Color.Red,
            };

            var ox = 32;
            var y = 48;
            var start = datelist[0];
            var prevtime = new DateTime(start.Year, start.Month, start.Day, 5, 0, 0);
            var limittime = prevtime.AddDays(1);
            var starttime = prevtime;
            Font fnt = new Font("MS UI Gothic", 12);

            const int attackheight = 30;

            var dispday = start;
            for (int i = 0; i < 5; i++)
            {
                g.FillRectangle(brush[i], i * 48 + 32, 8, 48, 16);
                g.DrawString($"{dispday.Month}/{dispday.Day}", fnt, Brushes.Black, i * 48 + 40, 8);
                dispday = dispday.AddDays(1);
            }


            for (int i = 0; i < 25; i++)
            {
                int x = ox + i * 30;
                g.DrawLine(Pens.White, x, 32, x, 32 + 300);

                g.DrawString(((i + 5) % 24).ToString(), fnt, Brushes.Black, x, 32);
            }

            for (int i = 0; i < 10; i++)
            {
                int ly = y + i * attackheight + 16;
                g.DrawLine(Pens.White, 0, ly, 25 * 30 + 32, ly);

                g.DrawString(((9 - i)).ToString(), fnt, Brushes.Black, 4, ly - 16);

            }

            var day = 0;
            foreach (var pointrawlist in AttackSpeedList(datelist))
            {
                using (var pen = new Pen(colors[day], 2))
                {
                    var drawline = pointrawlist.Select(n => new Point((int) (ox + n.X), (int)( y + 16 + (9 - n.Y) * attackheight))).ToArray();
                    g.DrawLines(pen, drawline);
                }

                day++;
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
                switch (tabControl1.SelectedIndex)
                {
                    case 0:
                        defeatlist = LoadDateFile(fileName[0]);
                        break;
                    case 1:
                    case 2:
                        attacklist = LoadDateFile(fileName[0]);
                        break;
                    default:
                        break;
                }

                this.Refresh();
            }
        }

        private List<DateTime> LoadDateFile(string fileName)
        {
            List<DateTime> d = new List<DateTime>();
            using (var sm = new StreamReader(fileName))
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

            return d;
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

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.Refresh();
        }

        private void tabPage1_Paint(object sender, PaintEventArgs e)
        {
            var g = e.Graphics;

            if (defeatlist != null)
            {
                DrawBossGraph(g, defeatlist);
            }
            else
            {
                Font fnt = new Font("MS UI Gothic", 12);
                g.DrawString("defeatlogをドラッグ＆ドロップしてください", fnt, Brushes.Black, 0, 0);
            }
        }

        private void tabPage2_Paint(object sender, PaintEventArgs e)
        {
            var g = e.Graphics;
            if (attacklist != null)
            {
                DrawAttackGraph(g, attacklist);
            }
            else
            {
                Font fnt = new Font("MS UI Gothic", 12);
                g.DrawString("attacklogをドラッグ＆ドロップしてください", fnt, Brushes.Black, 0, 0);
            }
        }

        private void tabPage3_Paint(object sender, PaintEventArgs e)
        {
            var g = e.Graphics;
            if (attacklist != null)
            {
                DrawAttackSpeedGraph(g, attacklist);
            }
            else
            {
                Font fnt = new Font("MS UI Gothic", 12);
                g.DrawString("attacklogをドラッグ＆ドロップしてください", fnt, Brushes.Black, 0, 0);
            }

        }
    }
}
