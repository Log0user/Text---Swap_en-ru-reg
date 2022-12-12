namespace Swap_en_ru
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {           
            bool select = true;

            if (radioButton5.Checked == true)
            {
                select = Auto(textBox1.Text);
            }

            if (radioButton1.Checked == true)
            {
                select = true;
            }

            if (radioButton2.Checked == true)
            {
                select = false;
            }


            textBox2.Text = Swap(textBox1.Text.ToString(), select);
        }

        private string Swap(string a, bool s)
        {
            string ru = "יצףךוםדרשחץתפûגאןנמכהז‎ÿקסלטעüב‏.";
            string en = "qwertyuiop[]asdfghjkl;'zxcvbnm,./";
            //int temp = 0;
            //bool rg = false;
            string result = "";

            if (a.Length != 0)
            {
                if (s)
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

                            else if (j == ru.Length - 1 && a[i] != ru[j])
                            {
                                for (int q = 0; q < ru.Length; q++)
                                {

                                    if (a[i] == Up(ru[q]))
                                    {
                                        result += Up(en[q]);
                                        break;
                                    }
                                }
                                // break;
                            }
                        }
                    }
                }
                else
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

                            else if (j == en.Length - 1 && a[i] != en[j])
                            {
                                for (int q = 0; q < ru.Length; q++)
                                {

                                    if (a[i] == Up(en[q]))
                                    {
                                        result += Up(ru[q]);
                                        break;
                                    }
                                }
                                // break;
                            }
                        }
                    }
                }

            }

            return result;
        }
        private char Up(char a)
        {
            char result = char.ToUpper(a);
            return result;
        }

        private bool Auto(string a)
        {
            string RUonle = "א, ב, ג, ד, ה, ו, ¸, ז, ח, ט, י, ך, כ, ל, ם, מ, ן, נ, ס, ע, ף, פ, ץ, צ, ק, ר, ש, ת, û, ü, ‎, ‏, ÿ";
            string ENonle = "a, b, c, d, e, f, g, h, i, j, k, l, m, n, o, p, q, r, s, t, u, v, w, x, y, z";
            bool result = false;
            //bool temp = false;

            for (int i = 0; i < a.Length; i++)
            {
                
                for (int j = 0; j < RUonle.Length; j++) // - 1
                {
                    if (a[i] == RUonle[j])
                    {
                        return true;
                    }
                }

                for (int j = 0; j < RUonle.Length; j++)
                {
                    if (a[i] == Up(RUonle[j]))
                    {
                        return true;
                    }
                }


                for (int j = 0; j < ENonle.Length; j++) // - 2
                {
                    if (a[i] == ENonle[j])
                    {
                        return false;
                    }
                }

                for (int j = 0; j < ENonle.Length; j++)
                {
                    if (a[i] == Up(ENonle[j]))
                    {
                        return false;
                    }
                }

            }



            return result;
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            radioButton5.Checked = true;
            radioButton6.Checked = true;
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
    }
}