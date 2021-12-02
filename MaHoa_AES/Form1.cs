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
using System.Text;
using System.Diagnostics;

namespace MaHoa_AES
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            txtPlainText.Text = "Nhom 24 ma hoa";
            txtKey.Text = "111111111111111111111111";
            string value = "Hello world";

            
        }
        public String[,] plainText = new String[4, 4];
        public String[,] khoa;
        public String[] khoa1;
        public String[,] khoaLap;
        public String[,] mangKhoa;
        public String[] mangKhoa1c;
        public Boolean print = true;
        
        /*public String[,] plainTextTest ={
            {"69","c4","e0","d8"},
            {"6a","7b","04","30"},
            {"d8","cd","b7","80"},
            {"70","b4","c5","5a"}
        };*/
        /*public String[,] plainTextTest ={
            {"69", "6a", "d8", "70"},
            {"c4", "7b", "cd", "b4"},
            {"e0", "04", "b7", "c5"},
            {"d8", "30", "80", "5a"}
        };
        public String[,] khoaTest ={
            {"00","04","08","0c"},
            {"01","05","09","0d"},
            {"02","06","0a","0e"},
            {"03","07","0b","0f"}
        };*/
        /*public String[,] plainTextTest ={
            {"00","44","88","cc"},
            {"11","55","99","dd"},
            {"22","66","aa","ee"},
            {"33","77","bb","ff"}
        };
        public String[,] khoaTest ={
            {"00","04","08","0c"},
            {"01","05","09","0d"},
            {"02","06","0a","0e"},
            {"03","07","0b","0f"}
        };*/
        public String[] khoaTest1 = {"00","01","02","03","04","05","06","07","08","09","0a","0b","0c","0d","0e","0f"};
        public Boolean pass=true;

        public int soVongLap;
        public int round;
        public int key_Length;

        private void get_print()
        {
            if (cb_print.Text == "Có in")
                print = true;
            else if (cb_print.Text == "Không in")
                print = false;
        }
        private void get_key_length()
        {
            if (cb_bitsize.Text == "AES - 128")
            {
                soVongLap = 10;
                key_Length=16;
                round = 4;
            }
            else if (cb_bitsize.Text == "AES - 192")
            {
                soVongLap = 12;
                key_Length = 24;
                round = 6;
            }
            else if (cb_bitsize.Text == "AES - 256")
            {
                soVongLap = 14;
                key_Length = 32;
                round = 8;
            }
            khoa1 = new string[key_Length];
            khoa = new string[4, round];
            khoaLap = new string[4, round];
            mangKhoa1c = new string[key_Length * (soVongLap+1)];
        }

        private Boolean dinhDangKhoa()
        {
            get_key_length();
            char[] key = txtKey.Text.ToCharArray();
            int doDaiKhoa = key.Length;
            if (doDaiKhoa != key_Length)
            {
                MessageBox.Show("Độ dài khóa không hợp lệ", "Lỗi đầu vào", MessageBoxButtons.OK);
                return false;
            }
            int aa = 0;
            int bb = 0;
            int a = 0;
            foreach (char word in key)
            {
                int value = Convert.ToInt32(word);
                khoa1[a] = $"{value:X}".ToLower();
                khoa[aa,bb]= $"{value:X}".ToLower();
                aa++;
                a++;
                if (aa > 3 && bb != round-1)
                {
                    aa = 0;
                    bb++;
                }
            }
            return true;
        }

        private void btnMahoa_Click(object sender, EventArgs e)
        {
            get_print();
            Stopwatch st = new Stopwatch();
            st.Start();
            if (dinhDangKhoa() == false)
                return;
            //Chuyen hóa chuỗi đầu vào
            string[,] state = new string[4, 4];
            string plainTextStr = txtPlainText.Text;
            int doDai = plainTextStr.Length;
            int du = doDai % 16;
            if (16 - du >= 0)
            {
                for (int bienChay = 0; bienChay < 16 - du; bienChay++)
                {
                    plainTextStr = String.Concat(plainTextStr, " ");
                }
            }
            if(du != 0)
                doDai = doDai + 16 - du;
            int soCum = doDai / 16;
            char[] plainTextArr = plainTextStr.ToCharArray();
            
            int pts = 0;
            for(int cumSo = 0; cumSo < soCum; cumSo++)
            {
                
                char[] plainTextArr_child = new char[16];
                for(int stt = 0; stt < 16; stt++)
                {
                    plainTextArr_child[stt] = plainTextArr[pts];
                    pts++;
                }
                int a = 0;
                int b = 0;
                foreach (char word in plainTextArr_child)
                {
                    int value = Convert.ToInt32(word);
                    plainText[a, b] = $"{value:X}".ToLower();
                    a++;
                    if (a > 3 && b != 3)
                    {
                        a = 0;
                        b++;
                    }

                }
                if (print)
                {
                    rtbKq.Text += "Bản mã : \n";
                    xuatmatran(plainText);
                    rtbKq.Text += "Khóa : \n";
                    xuatmatran(khoa);
                    taoMangKhoa();
                    state = AddRoundKeyvonglap(plainText, 0);
                    rtbKq.Text += "Hàm AddRoundKey : \n";
                    xuatmatran(state);
                    int z = 1;
                    for (int i = 0; i < soVongLap - 1; i++)
                    {
                        rtbKq.Text += "Vòng lặp thứ " + z + " : " + "\n";
                        rtbKq.Text += "Hàm SubByte : \n";
                        state = SubBytes(state);
                        xuatmatran(state);
                        rtbKq.Text += "Hàm ShiftRow : \n";
                        state = ShiftRows(state);
                        xuatmatran(state);
                        rtbKq.Text += "Hàm MixColumn : \n";
                        state = MixColumns(state);
                        xuatmatran(state);
                        rtbKq.Text += "Hàm AddRoundKey : \n";
                        state = AddRoundKeyvonglap(state, i + 1);
                        xuatmatran(state);
                        z++;
                    }
                    rtbKq.Text += "Bước tạo ngõ ra : " + "\n";
                    rtbKq.Text += "Hàm SubByte : \n";
                    state = SubBytes(state);
                    xuatmatran(state);
                    rtbKq.Text += "Hàm ShiftRow : \n";
                    state = ShiftRows(state);
                    xuatmatran(state);
                    rtbKq.Text += "Bản mã :\n";
                    state = AddRoundKeyvonglap(state, soVongLap);
                    xuatmatran(state);
                    for (int i = 0; i <= 3; i++)
                        for (int j = 0; j <= 3; j++)
                        {
                            txtcypher.Text += state[j, i];
                        }
                }
                else
                {
                    taoMangKhoa();
                    state = AddRoundKeyvonglap(plainText, 0);
                    int z = 1;
                    for (int i = 0; i < soVongLap - 1; i++)
                    {
                        state = SubBytes(state);
                        state = ShiftRows(state);
                        state = MixColumns(state);
                        state = AddRoundKeyvonglap(state, i + 1);
                        z++;
                    }
                    state = SubBytes(state);
                    state = ShiftRows(state);
                    state = AddRoundKeyvonglap(state, soVongLap);
                    for (int i = 0; i <= 3; i++)
                        for (int j = 0; j <= 3; j++)
                        {
                            txtcypher.Text += state[j, i];
                        }
                }
                
            }
            st.Stop();
            txt_time.Text = st.Elapsed.TotalSeconds.ToString() + "s";
        }

        private void btnGiaiMa_Click(object sender, EventArgs e)
        {
            get_print();
            Stopwatch st = new Stopwatch();
            st.Start();
            if (dinhDangKhoa() == false)
                return;
            //Chuyen hóa chuỗi đầu vào
            string[,] state = new string[4, 4];
            string plainTextStr = txtPlainText.Text;
            int doDai = plainTextStr.Length;
            int soCum = doDai / 32;
            char[] plainTextArr = plainTextStr.ToCharArray();

            int pts = 0;
            for (int cumSo = 0; cumSo < soCum; cumSo++)
            {
                string[] plainTextArr_child = new string[16];
                for (int stt = 0; stt < 16; stt++)
                { 
                    string str = "" + plainTextStr[pts] + plainTextStr[pts + 1];
                    plainTextArr_child[stt] = str;
                    pts += 2;
                }
                int a = 0;
                int b = 0;
                foreach (string word in plainTextArr_child)
                {
                    plainText[a, b] = word;
                    a++;
                    if (a > 3 && b != 3)
                    {
                        a = 0;
                        b++;
                    }
                }
                if (print)
                {
                    rtbKq.Text += "Bản mã : \n";
                    xuatmatran(plainText);
                    rtbKq.Text += "Khóa : \n";
                    xuatmatran(khoa);
                    taoMangKhoa();
                    state = AddRoundKeyvonglap(plainText, soVongLap);
                    rtbKq.Text += "Hàm AddRoundKey : \n";
                    xuatmatran(state);
                    int z = 1;
                    for (int i = soVongLap - 1; i > 0; i--)
                    {
                        rtbKq.Text += "Vòng lặp thứ " + z + " : " + "\n";
                        
                        state = inv_ShiftRows(state);
                        rtbKq.Text += "Hàm Inv_ShiftRow : \n";
                        xuatmatran(state);
                        state = inv_SubBytes(state);
                        rtbKq.Text += "Hàm Inv_SubByte : \n";
                        xuatmatran(state);
                        state = AddRoundKeyvonglap(state, i);
                        rtbKq.Text += "Hàm AddRoundKey : \n";
                        xuatmatran(state);
                        state = inv_MixColumns(state);
                        rtbKq.Text += "Hàm Inv_MixColumn : \n";
                        xuatmatran(state);
                        z++;
                    }
                    rtbKq.Text += "Bước tạo ngõ ra : " + "\n";
                    state = inv_ShiftRows(state);
                    rtbKq.Text += "Hàm Inv_ShiftRow : \n";
                    xuatmatran(state);
                    state = inv_SubBytes(state);
                    rtbKq.Text += "Hàm Inv_SubByte : \n";
                    xuatmatran(state);
                    state = AddRoundKeyvonglap(state, 0);
                    rtbKq.Text += "Bản mã :\n";
                    xuatmatran(state);
                    for (int i = 0; i <= 3; i++)
                        for (int j = 0; j <= 3; j++)
                        {
                            int value = Convert.ToInt32(state[j, i].ToUpper(), 16);
                            char stringValue = (char)value;
                            txtcypher.Text += stringValue.ToString();
                        }
                }
                else
                {
                    taoMangKhoa();
                    state = AddRoundKeyvonglap(plainText, soVongLap);
                    int z = 1;
                    for (int i = soVongLap - 1; i > 0; i--)
                    {
                        state = inv_ShiftRows(state);
                        state = inv_SubBytes(state);
                        state = AddRoundKeyvonglap(state, i);
                        state = inv_MixColumns(state);
                        z++;
                    }
                    state = inv_ShiftRows(state);
                    state = inv_SubBytes(state);
                    state = AddRoundKeyvonglap(state, 0);
                    for (int i = 0; i <= 3; i++)
                        for (int j = 0; j <= 3; j++)
                        {
                            int value = Convert.ToInt32(state[j, i].ToUpper(), 16);
                            char stringValue = (char)value;
                            txtcypher.Text += stringValue.ToString();
                        }
                }
            }
            st.Stop();
            txt_time.Text = st.Elapsed.TotalSeconds.ToString() + "s";
        }

        public void xuatmatran(String[,] matran)
        {
            for (int i = 0; i <= 3; i++)
                for (int j = 0; j <= 3; j++)
                {

                    if (j == 3) rtbKq.Text += "  " + matran[i, j] + "\n";
                    else rtbKq.Text += "  " + matran[i, j] + " ";

                }
        }

        /*private string[,] AddRoundKey(string[,] stateIn)
        {
            string[,] stateOut = new string[4, 4];
            rtbKq.Text += "AddRoundKey : \n";
            for (int i = 0; i <= 3; i++)
                for (int j = 0; j <= 3; j++)
                {
                    stateOut[i, j] = AES.XOR16(plainText[i, j], khoa[i, j]);
                }
            xuatmatran(stateOut);
            return stateOut;
        }*/

        private string[,] AddRoundKeyvonglap(String[,] stateIn,int stt)
        {
            //taoKhoa(stt,khoamoi);
            string[,] khoamoi = new string[4, 4];
            string[,] stateOut = new string[4, 4];
            /*int a = 0;
            int b = 0;*/
            for (int i = 0; i < 4; i++)
            {
                for(int j = 0; j < 4; j++)
                {
                    khoamoi[i,j ] = mangKhoa1c[stt * 16 +4*j+i];
                }
            }
            if (print)
            {
                rtbKq.Text += "Khóa dùng tại vòng này:\n";
                xuatmatran(khoamoi);
                rtbKq.Text += "AddRoundKey : \n";
            }
            for (int i = 0; i <= 3; i++)
                for (int j = 0; j <= 3; j++)
                { 
                    stateOut[i, j] = AES.XOR16(stateIn[i, j], khoamoi[i, j]);
                }
            return stateOut;
        }

        private string[,] SubBytes(string[,] stateIn)
        {
            string[,] stateOut = new string[4, 4];
            for (int i = 0; i <= 3; i++)
                for (int j = 0; j <= 3; j++)
                {
                    String ark = stateIn[i, j];
                    stateOut[i, j] = AES.sbox[AES.chuyen16sangso(ark[0]), AES.chuyen16sangso(ark[1])];
                }
            return stateOut;
        }

        private  string[,] inv_SubBytes(string [,] stateIn)
        {
            string[,] stateOut = new string[4, 4];
            for (int i = 0; i <= 3; i++)
                for (int j = 0; j <= 3; j++)
                {
                    String ark = stateIn[i, j];
                    stateOut[i, j] = AES.inv_sbox[AES.chuyen16sangso(ark[0]), AES.chuyen16sangso(ark[1])];
                }
            return stateOut;
        }

        public string [,] ShiftRows(string[,] stateIn)
        {
            string[,] stateOut = new string[4, 4];
            String teap;
            teap = stateIn[1, 0];
            stateIn[1, 0] = stateIn[1, 3];
            stateIn[1, 3] = teap;
            teap = stateIn[1, 0];
            stateIn[1, 0] = stateIn[1, 2];
            stateIn[1, 2] = teap;
            teap = stateIn[1, 0];
            stateIn[1, 0] = stateIn[1, 1];
            stateIn[1, 1] = teap;

            teap = stateIn[2, 0];
            stateIn[2, 0] = stateIn[2, 2];
            stateIn[2, 2] = teap;
            teap = stateIn[2, 1];
            stateIn[2, 1] = stateIn[2, 3];
            stateIn[2, 3] = teap;

            teap = stateIn[3, 0];
            stateIn[3, 0] = stateIn[3, 3];
            stateIn[3, 3] = teap;
            teap = stateIn[3, 1];
            stateIn[3, 1] = stateIn[3, 3];
            stateIn[3, 3] = teap;
            teap = stateIn[3, 2];
            stateIn[3, 2] = stateIn[3, 3];
            stateIn[3, 3] = teap;

            stateOut = stateIn;
            return stateOut;

        }

        public string[,] inv_ShiftRows(string[,]stateIn)
        {
            string[,] stateOut = new string[4, 4];
            String teap;
            teap = stateIn[3, 0];
            stateIn[3, 0] = stateIn[3, 3];
            stateIn[3, 3] = teap;
            teap = stateIn[3, 0];
            stateIn[3, 0] = stateIn[3, 2];
            stateIn[3, 2] = teap;
            teap = stateIn[3, 0];
            stateIn[3, 0] = stateIn[3, 1];
            stateIn[3, 1] = teap;

            teap = stateIn[2, 0];
            stateIn[2, 0] = stateIn[2, 2];
            stateIn[2, 2] = teap;
            teap = stateIn[2, 1];
            stateIn[2, 1] = stateIn[2, 3];
            stateIn[2, 3] = teap;

            teap = stateIn[1, 0];
            stateIn[1, 0] = stateIn[1, 3];
            stateIn[1, 3] = teap;
            teap = stateIn[1, 1];
            stateIn[1, 1] = stateIn[1, 3];
            stateIn[1, 3] = teap;
            teap = stateIn[1, 2];
            stateIn[1, 2] = stateIn[1, 3];
            stateIn[1, 3] = teap;

            stateOut = stateIn;
            return stateOut;

        }

        public string [,]  MixColumns(string [,]stateIn)
        {
            string[,] stateOut = new string[4, 4];
            for (int j = 0; j <= 3; j++)
            {
                String[] lay1cot = new String[4];
                for (int i = 0; i <= 3; i++)
                {
                    lay1cot[i] = stateIn[i, j];
                }
                for (int k = 0; k <= 3; k++)
                {
                    int[] kqsaubangbd = new int[4];
                    for (int q = 0; q <= 3; q++)
                    {
                        kqsaubangbd[q] = AES.nhanbangbd1(lay1cot[q], AES.matranbd[k, q]);
                        
                    }
                    stateOut[k, j] = AES.XOR16voi4kytu_inv(kqsaubangbd[0], kqsaubangbd[1], kqsaubangbd[2], kqsaubangbd[3]);
                }
            }
            return stateOut;
        }

        public string[,] inv_MixColumns(string[,] stateIn)
        {
            string[,] stateOut = new string[4, 4];
            for (int j = 0; j <= 3; j++)
            {
                String[] lay1cot = new String[4];
                for (int i = 0; i <= 3; i++)
                {
                    lay1cot[i] = stateIn[i, j];
                }
                for (int k = 0; k <= 3; k++)
                {
                    int[] kqsaubangbd = new int[4];
                    for (int q = 0; q <= 3; q++)
                    {
                        kqsaubangbd[q] = AES.inv_nhanbangbd(lay1cot[q], AES.inv_matranbd[k, q]);
                    }
                    stateOut[k, j] = AES.XOR16voi4kytu_inv(kqsaubangbd[0], kqsaubangbd[1], kqsaubangbd[2], kqsaubangbd[3]);
                }
            }
            return stateOut;
        }

        public void taoKhoa(int stt,String[,] Khoa){
            String [] R_con = new string[4];
            R_con=AES.layR_con(stt);
            khoa=AES.tinhkhoa(R_con,Khoa);
        }

        /*public void taoMangKhoa()
        {
            mangKhoa = new string[soVongLap + 1, key_Length];
            int j = 0;
            for (int i = 0;i< key_Length; i++)
            {
                mangKhoa[j, i] = khoa1[i];
            }
            j++;
            for(int i = 0; i < soVongLap; i++)
            {
                String[] R_con = new string[4];
                R_con = AES.layR_con(i);
                khoa = AES.tinhkhoa(R_con, khoa);
                int a = 0;
                int b = 0;
                for (int z = 0; z < key_Length; z++)
                {
                    mangKhoa[j, z] = khoa[a,b];
                    a++;
                    if (a > 3 && b != round-1)
                    {
                        a = 0;
                        b++;
                    }
                }
                j++;
            }
            for(int i =0; i <=soVongLap; i++)
            {
                rtbKq.Text += "Khóa " + i + " : ";
                for(int x = 0;x < key_Length; x++)
                {
                    rtbKq.Text += mangKhoa[i, x] + "-";
                }
                rtbKq.Text += "\n";

            }
                
        }*/
        public void taoMangKhoa()
        {
            mangKhoa = new string[soVongLap + 1, key_Length];
            int j = 0;
            for (int i = 0; i < key_Length; i++)
            {
                mangKhoa[j, i] = khoa1[i];
            }
            
            String[] R_con = new string[4];
            String[] temp_array = new String[4];
            for (int i = 1; i <=soVongLap; i++)
            {
                int kiHieu = 0;
                for (int z = 0; z < 4; z++)
                    temp_array[z] = mangKhoa[j, key_Length - (4- z)];
                temp_array = AES.bdcotcuoicuakhoa(temp_array,i);
                /*for (int z = 0; z < 4; z++)
                    temp_array1[z] = mangKhoa[j, z];*/
                R_con = AES.layR_con(i);
                for (int z = 0; z < 4; z++)
                    temp_array[z] = AES.XOR16( temp_array[z], R_con[z]);
                for(int z = 0; z < round; z++)
                {
                    if (kiHieu == 16 && round >6)
                        for (int x = 0; x < 4; ++x)
                            temp_array[x] = AES.sbox[AES.chuyen16sangso(temp_array[x][0]), AES.chuyen16sangso(temp_array[x][1])];
                    for(int x = 0; x < 4; x++)
                    {
                        temp_array[x] = AES.XOR16(temp_array[x], mangKhoa[j, kiHieu]);
                        mangKhoa[j+1, kiHieu] = temp_array[x];
                        kiHieu++;
                    }
                }
                j++;           
                /*R_con = AES.layR_con(i);
                khoa = AES.tinhkhoa(R_con, khoa);
                int a = 0;
                int b = 0;
                for (int z = 0; z < key_Length; z++)
                {
                    mangKhoa[j, z] = khoa[a, b];
                    a++;
                    if (a > 3 && b != round - 1)
                    {
                        a = 0;
                        b++;
                    }
                }
                j++;*/
            }
            if (print)
            {
                int z1 = 0;
                for (int n = 0; n <= soVongLap; n++)
                {
                    for (int m = 0; m < key_Length; m++)
                    {
                        mangKhoa1c[z1] = mangKhoa[n, m];
                        rtbKq.Text += mangKhoa1c[z1] + ",";
                        z1++;
                    }
                }
                rtbKq.Text += "\n";
                for (int i = 0; i <= soVongLap; i++)
                {
                    rtbKq.Text += "Khóa " + i + " : ";
                    for (int x = 0; x < key_Length; x++)
                    {
                        rtbKq.Text += mangKhoa[i, x] + "-";
                    }
                    rtbKq.Text += "\n";

                }
                rtbKq.Text += "\n";
            }
            else
            {
                int z1 = 0;
                for (int n = 0; n <= soVongLap; n++)
                {
                    for (int m = 0; m < key_Length; m++)
                    {
                        mangKhoa1c[z1] = mangKhoa[n, m];
                        z1++;
                    }
                }
            }

        }

        private void btn_file(object sender, EventArgs e)
        {
            if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                txt_file_path.Text = openFileDialog.FileName;
        }
        
        private void read_file_EN(object sender, EventArgs e)
        {
            string path = txt_file_path.Text;
            string content = File.ReadAllText(path);
            txtPlainText.Text = content;
        }

        private void clear(object sender, EventArgs e)
        {
            txtPlainText.Text = "";
            txtKey.Text = "";
            txtcypher.Text = "";
            rtbKq.Text = "";
            txt_file_path.Text = "";
            txt_time.Text = "";
        }

        private void read_file_key(object sender, EventArgs e)
        {
            string path = txt_file_path.Text;
            string content = File.ReadAllText(path);
            txtKey.Text = content;
        }

        private void read_file_DE(object sender, EventArgs e)
        {
            string path = txt_file_path.Text;
            string content = File.ReadAllText(path);
            txtcypher.Text = content;
        }

        private void btn_thoat(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void btnChange_click(object sender, EventArgs e)
        {
            txtPlainText.Text = txtcypher.Text;
            txtcypher.Text = "";
            txt_time.Text = "";
            rtbKq.Text = "";
        }
    }
}
