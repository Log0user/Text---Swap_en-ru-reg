using System.Reflection.Emit;
using System.Windows.Forms;

namespace Swap_en_ru
{

    public partial class Form1 : Form
    {
        private NotifyIcon notifyIcon;
        public Form1()
        {
            // ������� ������ NotifyIcon
            InitializeComponent();
            notifyIcon = new NotifyIcon();
            notifyIcon.Icon = SystemIcons.Application;
            notifyIcon.Visible = true;
            notifyIcon.DoubleClick += NotifyIcon_DoubleClick;
            notifyIcon.Text = "Swap en-ru";

            // ������������� �� ������� Move
            this.Move += Form1_Move;

            // ������������� �� ������� MouseUp
            // this.MouseUp += Form1_MouseUp;

            // ���������� ���������� ������� MouseClick
            notifyIcon.MouseClick += notifyIcon_MouseClick;
        }



        // ���������� ������� MouseClick ��� notifyIcon
        private void notifyIcon_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                ContextMenuStrip menu = new ContextMenuStrip();
                ToolStripMenuItem exitItem = new ToolStripMenuItem("�����");
                exitItem.Click += ExitItem_Click;
                menu.Items.Add(exitItem);
                notifyIcon.ContextMenuStrip = menu;
                menu.Show(Control.MousePosition);
            }
        }

        // ���������� ������� ��� ����� "�����" � ����������� ����
        private void ExitItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
        private void Form1_Resize(object sender, EventArgs e)
        {
            if (WindowState == FormWindowState.Minimized)
            {
                Hide(); //������ �����
                // notifyIcon.ShowBalloonTip(500, "���������", "����� ���������", ToolTipIcon.Info);
                //notifyIcon.ShowBalloonTip(500);
            }
        }

        private void NotifyIcon_DoubleClick(object sender, EventArgs e)
        {
            Show();
            this.Location = new Point(182, 182);
            WindowState = FormWindowState.Normal;
            if (Clipboard.ContainsText())
            {
                UsingCipboard(false);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //ItsNotButton1Press();
            bool select = true;
            bool check = true;

            if (radioButton5.Checked == true)
            {
                select = Auto(richTextBox1.Text);
            }

            if (radioButton1.Checked == true)
            {
                select = true;
            }

            if (radioButton2.Checked == true)
            {
                select = false;
            }

            if (checkBox1.Checked == true)
            {
                check = true;
            }
            else
            {
                check = false;
            }

            richTextBox2.Text = Swap(richTextBox1.Text.ToString(), select, check);
            UsingCipboard(true);
        }


        private string Swap(string a, bool s, bool c) // - ������ ��������� ���������
        {
            //string ru = "��������������������������������.�";
            //string en = "qwertyuiop[]asdfghjkl;'zxcvbnm,./`";
            // string ru = "��������������������������������.";          
            // string en = "qwertyuiop[]asdfghjkl;'zxcvbnm,./";

            string ru = "��������������������������������.!�;%:?*()_+/@";
            string en = "qwertyuiop[]asdfghjkl;'zxcvbnm,./!#$%^&*()_+|\"";

            string result = "";

            if (a.Length != 0)
            {
                int x = 0;
                if (s) // - ����� RU->EN
                {
                    for (int i = 0; i < a.Length; i++)
                    {
                        for (int j = 0; j < ru.Length; j++)
                        {
                            if (a[i] == ru[j])
                            {
                                result += en[j];
                                break;
                            }
                            else if (a[i] == ' ')
                            {
                                result += ' ';
                                break;
                            }

                            else if (int.TryParse(Convert.ToString(a[i]), out x))
                            {
                                result += x;
                                break;
                            }


                            //else if (j == ru.Length - 1 && a[i] != ru[j])
                            //{
                            //    for (int q = 0; q < ru.Length; q++)
                            //    {

                            //        if (a[i] == Up(ru[q]))
                            //        {
                            //            result += Up(en[q]);
                            //            break;
                            //        }



                            //    }
                            //}

                            else //------------------�������� - ������� � �������, � ��������� �����. ���� ����������
                            {
                                if (j == ru.Length - 1 && a[i] != ru[j])
                                {
                                    for (int q = 0; q < ru.Length; q++)
                                    {

                                        if (a[i] == Up(ru[q]))
                                        {
                                            result += Up(en[q]);
                                            break;
                                        }



                                    }
                                }
                                //else // --------------�������� � ���, ��� ����� ���������� �� ��������� ������. �������
                                //{
                                //    if (c)
                                //    {
                                //        if (Sig(a, true, i) != '0')
                                //        {
                                //            result += Sig(a, true, i);
                                //            break;
                                //        }
                                //    }
                                //}

                            }



                        }
                    }
                }
                else // - ����� EN-RU
                {
                    for (int i = 0; i < a.Length; i++)
                    {
                        for (int j = 0; j < ru.Length; j++)
                        {
                            if (a[i] == en[j])
                            {
                                result += ru[j];
                                break;
                            }

                            else if (a[i] == ' ')
                            {
                                result += ' ';
                                break;
                            }

                            else if (int.TryParse(Convert.ToString(a[i]), out x))
                            {
                                result += x;
                                break;
                            }

                            //else if (j == en.Length - 1 && a[i] != en[j])
                            //{
                            //    for (int q = 0; q < ru.Length; q++)
                            //    {

                            //        if (a[i] == Up(en[q]))
                            //        {
                            //            result += Up(ru[q]);
                            //            break;
                            //        }

                            //    }
                            //}

                            else
                            {
                                //if (char.ToUpper(Convert.ToChar(a[i])) == Up)
                                //{

                                //}
                                if (j == en.Length - 1 && a[i] != en[j])
                                {
                                    for (int q = 0; q < ru.Length; q++)
                                    {

                                        if (a[i] == Up(en[q]))
                                        {
                                            result += Up(ru[q]);
                                            break;
                                        }

                                    }
                                }

                                //else // --------------�������� � ���, ��� ����� ���������� �� ��������� ������. �������
                                //{

                                //    if (c)
                                //    {
                                //        if (Sig(a, false, i) != '0')
                                //        {
                                //            result += Sig(a, false, i);
                                //            break;
                                //        }

                                //    }
                                //}
                            }



                        }
                    }
                }

            }

            return result;
        }

        private char Up(char a) // - ������ ����� ���������
        {
            char result = char.ToUpper(a);
            return result;
        }

        private char Sig(string a, bool t, int s) // - �������� �� ����������� (������� �� ���� ����, ��� 2)
        {
            char result = '0';
            string ru = "!�;%:?*()_+/";
            string en = "!#$%^&*()_+|";

            if (t)
            {
                for (int i = s; i < a.Length; i++)
                {
                    for (int j = 0; j < ru.Length; j++) //--------RU
                    {
                        if (a[i] == ru[j])
                        {
                            result = en[j];
                            return result;
                        }
                        else if (a[i] == '"')
                        {
                            result = '@';
                            return result;
                        }
                    }
                }

            }
            else
            {
                for (int i = s; i < a.Length; i++)
                {
                    for (int j = 0; j < en.Length; j++) //--------EN
                    {
                        if (a[i] == en[j])
                        {
                            result = ru[j];
                            return result;
                        }
                        else if (a[i] == '@')
                        {
                            result = '"';
                            return result;
                        }
                    }
                }

            }


            return result;
        }

        private char Down(char a) // - ������ ����� ��������
        {
            char result = char.ToLower(a);
            return result;
        }

        private bool Auto(string a) // - �������������� ����������� ��������� �� ��������� ���� �����
        {
            string RUonle = "��������������������������������";
            string ENonle = "abcdefghijklmnopqrstuvwxyz";

            //string RUonle = "�, �, �, �, �, �, �, �, �, �, �, �, �, �, �, �, �, �, �, �, �, �, �, �, �, �, �, �, �, �, �, �, �";
            //string ENonle = "a, b, c, d, e, f, g, h, i, j, k, l, m, n, o, p, q, r, s, t, u, v, w, x, y, z";
            bool result = false;

            for (int i = 0; i < a.Length; i++)
            {

                for (int j = 0; j < RUonle.Length; j++) // - ��������� �� ��������� ��������� �������� ��������
                {
                    if (a[i] == RUonle[j])
                    {
                        return true;
                    }
                }

                for (int j = 0; j < RUonle.Length; j++) // - ��������� �����
                {
                    if (a[i] == Up(RUonle[j]))
                    {
                        return true;
                    }
                }


                for (int j = 0; j < ENonle.Length; j++) // - ��������� �� ��������� ��������� ����������� ��������
                {
                    if (a[i] == ENonle[j])
                    {
                        return false;
                    }
                }

                for (int j = 0; j < ENonle.Length; j++) // - ��������� �����
                {
                    if (a[i] == Up(ENonle[j]))
                    {
                        return false;
                    }
                }

            }

            return result;
        }
        private void UsingCipboard(bool a)// - ������ � ������� ������
        {

            if (checkBox2.Checked)
            {
                if (!a)//- ��������� �����, ��� ������ ������? false - �� �����, true - �� ������
                {
                    string clipboardText = Clipboard.GetText();
                    // ���������� ��������� ����������� ������
                    richTextBox1.Text = clipboardText;

                    button1.PerformClick();

                    //ItsNotButton1Press();
                    Clipboard.SetText(richTextBox2.Text);


                    IDataObject data = new DataObject();
                    data.SetData(DataFormats.UnicodeText, true, richTextBox2.Text);
                }
                else
                {
                    IDataObject data = new DataObject();
                    data.SetData(DataFormats.UnicodeText, true, richTextBox2.Text);
                }

            }

        }
        private void Form1_Load(object sender, EventArgs e)
        {
            radioButton5.Checked = true;
            radioButton6.Checked = true;
            //// �������� � �������� ������
            //Icon icon = new Icon("1.ico"); //�����-������ �����

            //// ��������� ������ ��� �����
            //this.Icon = icon;
            if (Clipboard.ContainsText())
            {
                UsingCipboard(false);
            }
        }

        private void radioButton1_Click(object sender, EventArgs e)
        {
            radioButton4.Checked = true;

            radioButton2.Checked = false;
            radioButton3.Checked = false;

            radioButton5.Checked = false;
            radioButton6.Checked = false;
        }

        private void radioButton2_Click(object sender, EventArgs e)
        {
            radioButton3.Checked = true;

            radioButton1.Checked = false;
            radioButton4.Checked = false;

            radioButton5.Checked = false;
            radioButton6.Checked = false;
        }

        private void radioButton3_Click(object sender, EventArgs e)
        {
            radioButton2.Checked = true;

            radioButton1.Checked = false;
            radioButton4.Checked = false;

            radioButton5.Checked = false;
            radioButton6.Checked = false;
        }

        private void radioButton4_Click(object sender, EventArgs e)
        {
            radioButton1.Checked = true;

            radioButton3.Checked = false;
            radioButton2.Checked = false;

            radioButton5.Checked = false;
            radioButton6.Checked = false;

        }

        private void radioButton6_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void radioButton5_Click(object sender, EventArgs e)
        {
            radioButton6.Checked = true;

            radioButton1.Checked = false;
            radioButton2.Checked = false;
            radioButton3.Checked = false;
            radioButton4.Checked = false;
        }

        private void radioButton6_Click(object sender, EventArgs e)
        {
            radioButton5.Checked = true;

            radioButton1.Checked = false;
            radioButton2.Checked = false;
            radioButton3.Checked = false;
            radioButton4.Checked = false;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string result = "";
            for (int i = 0; i < richTextBox1.Text.Length; i++)
            {
                result += Up(richTextBox1.Text[i]);
            }
            richTextBox2.Text = result;
            result = "";
        }

        private void button3_Click(object sender, EventArgs e)
        {
            string result = "";
            for (int i = 0; i < richTextBox1.Text.Length; i++)
            {
                result += Down(richTextBox1.Text[i]);
            }
            richTextBox2.Text = result;
            result = "";
        }

        private void button5_Click(object sender, EventArgs e)
        {
            richTextBox2.Text = richTextBox1.Text;
            richTextBox1.Clear();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            richTextBox1.Text = richTextBox2.Text;
            richTextBox2.Clear();
        }

        private void groupBox2_Enter(object sender, EventArgs e)
        {

        }



        private void Form1_DoubleClick(object sender, EventArgs e)
        {

        }



        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }

        private void Form1_Move(object sender, EventArgs e)
        {
            int x = this.Location.X;
            int y = this.Location.Y;
            label3.Text = $"����� ��������� �� ����������� ({x}, {y})";
            if (x > 1500 && y > 1000)
            {
                Hide();
                //Form1_Resize(this, EventArgs.Empty);
            }
        }

        //private void Form1_MouseUp(object sender, MouseEventArgs e)
        //{
        //    if (e.Button == MouseButtons.Left)
        //    {
        //        // ��������� ��������, ����� �������� ����� ������ ����
        //        MessageBox.Show("����� ������ ���� ���� ��������");

        //        button1.Text = "button1";
        //    }
        //}
    }

    //private void Form1_Resize(object sender, EventArgs e)
    //{
    //    if (WindowState == FormWindowState.Minimized)
    //    {
    //        Hide();
    //        notifyIcon.ShowBalloonTip(500);
    //    }
    //}
    //private void NotifyIcon_DoubleClick(object sender, EventArgs e)
    //{
    //    Show();
    //    WindowState = FormWindowState.Normal;
    //}


}