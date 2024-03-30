using System.Reflection.Emit;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;






namespace Swap_en_ru
{

    public partial class Form1 : Form
    {


        // ����������� ������� �� user32.dll ��� ������ � �������� ���������
        [DllImport("user32.dll")]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

        [DllImport("user32.dll")]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        // ����������� ������� �� user32.dll ��� ������ � �������� �����
        

        // ��������� � ������� ��� ������ � ������� ������
        private const int WM_GETTEXT = 0x000D;

        // ��������� ��� ������������� ������ (Alt, Ctrl, Shift, Win)
        private const uint MOD_NONE = 0x0000; // ��� �������������
        private const uint MOD_ALT = 0x0001; // Alt
        private const uint MOD_CONTROL = 0x0002; // Ctrl
        private const uint MOD_SHIFT = 0x0004; // Shift
        private const uint MOD_WIN = 0x0008; // Win

        // ��������� ��� ������
        private const uint VK_F9 = 0x78; // ��� ������� F9

        // ID ������� �������
        private const int HOTKEY_ID = 1;



        private NotifyIcon notifyIcon;
        public Form1()
        {
            // ������� ������ NotifyIcon
            InitializeComponent();

            // ����������� ������� ������� Ctrl+F9 ��� ������������� �����
            RegisterHotKey(this.Handle, HOTKEY_ID, MOD_CONTROL, VK_F9);

            // ����������� ������� ������� F9 ��� �������������
            RegisterHotKey(this.Handle, HOTKEY_ID + 1, MOD_NONE, VK_F9);

            notifyIcon = new NotifyIcon();
            notifyIcon.Icon = SystemIcons.Application;
            notifyIcon.Visible = true;
            notifyIcon.DoubleClick += NotifyIcon_DoubleClick;
            notifyIcon.Text = "Swap en-ru";

            // ������������� �� ������� Move
            this.Move += Form1_Move;

            // ���������� ���������� ������� MouseClick
            notifyIcon.MouseClick += notifyIcon_MouseClick;

            this.KeyPreview = true; // ��������� �������� KeyPreview � true ��� ��������� f9
        }


        // ����� ��� ��������� ��������� Windows
        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);

            // ��������� ��������� � ������� ������� �������
            if (m.Msg == 0x0312)
            {
                // ���������, ���� �� ������ ���������� Ctrl+F9
                if (m.WParam.ToInt32() == HOTKEY_ID)
                {
                    // ������� �� ������� ���������� Ctrl+F9
                   // MessageBox.Show("������� ������� Ctrl+F9 ������!");

                    // ����������� ����������� ������ � ����� ������
                    CopySelectedTextToClipboard();

                    NotifyIcon_DoubleClick(null, null); // ����� ������� - �������� �� ������� �����
                }
                // ���������, ���� �� ������ ������� F9 ��� �������������
                else if (m.WParam.ToInt32() == HOTKEY_ID + 1)
                {
                    // ������� �� ������� ������� F9 ��� �������������
                    //MessageBox.Show("������� ������� F9 ������!");
                    NotifyIcon_DoubleClick(null, null); //����� ������� - �������� �� ������� �����
                }
            }
        }





        // ����� ��� ����������� ����������� ������ �� ��������� ���� � ����� ������
        private void CopySelectedTextToClipboard()
        {
           //������ ����������� ����������� ������, �������
        }









        // ���������� ������� MouseClick ��� notifyIcon -���
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
            }
        }

        private void NotifyIcon_DoubleClick(object sender, EventArgs e) //�������� �� ������� �����
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

            string ru = "��������������������������������.!�;%:?*()_+/@{";   //�� �������� � "/", �������
            string en = "qwertyuiop[]asdfghjkl;'zxcvbnm,./!#$%^&*()_+|\"�";

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
                    // ��������� ������ �� ������ ������
                    string clipboardText = Clipboard.GetText();


                    // ��������, �� ���� �� ����� �� ������ ������
                    if (!string.IsNullOrEmpty(clipboardText))
                    {
                        // ���������� ��������� ����������� ������
                        // ��������� ������ �� ������ ������ � richTextBox1
                        richTextBox1.Text = clipboardText;

                        //��������� ������� ������ button1
                        button1.PerformClick();

                        // ��������� ������ �� richTextBox2 � ����� ������
                        Clipboard.SetText(richTextBox2.Text);

                        // �������� ������� ������ � ��������� ������ � ������� UnicodeText
                        IDataObject data = new DataObject();
                        data.SetData(DataFormats.UnicodeText, true, richTextBox2.Text);

                    }
                    else
                    {
                        // ��������� ��������, ����� ����� ������ ����
                        MessageBox.Show("����� ������ ����!");
                    }
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
            }
        }
       
    }

}