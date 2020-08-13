using Npgsql;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace kitap_satis
{
    public partial class Form1 : Form
    {
        NpgsqlConnection connection = new NpgsqlConnection("Server=localhost; Port=5432; Database=kitap_satis; User id=postgres; Password=test");
        NpgsqlDataAdapter add;
        NpgsqlCommand dbcmd;
        public Form1()
        {
            try
            {
                connection.Open();
            }
            catch (Exception ex)
            {

                throw;
            }
            InitializeComponent();

            dataSetGetir("kitap");
            dataSetGetir("yayinci");
            dataSetGetir("yazar");
            dataSetGetir("uye");
            siparisleriListele();
            
            DataSet bolgeler = new DataSet();
            string sorgu = "SELECT bolge.id,bolge.ad as ad,sehir.ad as sehir FROM bolge INNER JOIN sehir on bolge.sehir_id=sehir.id";
            add = new NpgsqlDataAdapter(sorgu, connection);
            add.Fill(bolgeler);
            foreach (DataRow item in bolgeler.Tables[0].Rows)
            {
                comboBox7.Items.Add("" + item["id"] + "-" + item["ad"] + "-" + item["sehir"]);
            }
        }

        public void siparisleriListele()
        {
            DataSet siparisler = new DataSet();
            string sorgu = "SELECT odeme.odeme_turu,odeme.tarih,uye.ad,alt.fiyat,alt.ad as kitap_ad,siparis.id FROM siparis INNER JOIN odeme ON siparis.odeme_id=odeme.id INNER JOIN uye ON siparis.uye_id=uye.id INNER JOIN (select satistaki_kitaplar.id,satistaki_kitaplar.fiyat,kitap.ad FROM satistaki_kitaplar INNER JOIN kitap ON kitap.satis_id=satistaki_kitaplar.id) alt ON siparis.kitap_id=alt.id";
            add = new NpgsqlDataAdapter(sorgu, connection);
            add.Fill(siparisler);
            dataGridView4.DataSource = siparisler.Tables[0];
        }

        public void dataSetGetir(string tip)
        {
            string sorgu = "";
            if(tip == "kitap")
            {
                DataSet kitaplar = new DataSet();
                sorgu = "SELECT kitap.ad,yayinci.ad as yayinci_ad,yazar.ad as yazar_ad,satistaki_kitaplar.fiyat,satistaki_kitaplar.id FROM kitap INNER JOIN yayinci ON kitap.yayinci_id=yayinci.id INNER JOIN yazar ON kitap.yazar_id=yazar.id INNER JOIN satistaki_kitaplar ON kitap.satis_id = satistaki_kitaplar.id";
                add = new NpgsqlDataAdapter(sorgu, connection);
                add.Fill(kitaplar);
                dataGridView1.DataSource = kitaplar.Tables[0];
                comboBox4.Items.Clear();
                listBox1.Items.Clear();
                foreach (DataRow item in kitaplar.Tables[0].Rows)
                {
                    comboBox4.Items.Add("" + item["id"] + "-" + item["ad"]);
                    listBox1.Items.Add("" + item["id"] + "-" + item["ad"]);
                }
            }
            else if(tip == "yayinci")
            {
                DataSet kitaplar = new DataSet();
                sorgu = "SELECT ad,id FROM yayinci";
                add = new NpgsqlDataAdapter(sorgu, connection);
                add.Fill(kitaplar);
                dataGridView2.DataSource = kitaplar.Tables[0];
                comboBox9.Items.Clear();
                comboBox2.Items.Clear();
                foreach (DataRow item in kitaplar.Tables[0].Rows)
                {
                    comboBox9.Items.Add(item["id"] + "-" + item["ad"]);
                    comboBox2.Items.Add(item["id"] + "-" + item["ad"]);
                }
            }
            else if(tip == "yazar")
            {
                DataSet kitaplar = new DataSet();
                sorgu = "SELECT ad,id FROM yazar";
                add = new NpgsqlDataAdapter(sorgu, connection);
                add.Fill(kitaplar);
                dataGridView3.DataSource = kitaplar.Tables[0];
                comboBox8.Items.Clear();
                comboBox1.Items.Clear();
                foreach (DataRow item in kitaplar.Tables[0].Rows)
                {
                    comboBox8.Items.Add(item["id"] + "-" + item["ad"]);
                    comboBox1.Items.Add(item["id"]+"-"+item["ad"]);
                }
            }
            else if(tip == "uye")
            {
                DataSet kitaplar = new DataSet();
                sorgu = "SELECT uye.ad,iletisim.telefon,uye.id FROM uye INNER JOIN iletisim ON uye.iletisim_id=iletisim.id";
                add = new NpgsqlDataAdapter(sorgu, connection);
                add.Fill(kitaplar);
                dataGridView5.DataSource = kitaplar.Tables[0];
                comboBox5.Items.Clear();
                listBox2.Items.Clear();
                foreach (DataRow item in kitaplar.Tables[0].Rows)
                {
                    comboBox5.Items.Add("" + item["id"] + "-" + item["ad"]);
                    listBox2.Items.Add("" + item["id"] + "-" + item["ad"]);
                }
            }
        }

        private void kitap_ekle_Click(object sender, EventArgs e)
        {
            string sql = "INSERT INTO public.satistaki_kitaplar(fiyat)VALUES(" + textBox9.Text + "); ";
            dbcmd = connection.CreateCommand();
            dbcmd.CommandText = sql;
            dbcmd.ExecuteNonQuery();

            if (comboBox3.SelectedIndex == 0)   // TEST // _ad text,_yayinci_id int, _yazar_id int, _soru_sayisi int,_fiyat int
            {
                sql = "CALL test_kitabi('" + textBox1.Text + "',"+comboBox2.GetItemText(comboBox2.SelectedItem).Split('-')[0]+","+ comboBox1.GetItemText(comboBox1.SelectedItem).Split('-')[0] + ","+textBox2.Text+");";
            }
            else if (comboBox3.SelectedIndex == 1) // çocuk
            {
                sql = "CALL cocuk_kitabi('" + textBox1.Text + "'," + comboBox2.GetItemText(comboBox2.SelectedItem).Split('-')[0] + "," + comboBox1.GetItemText(comboBox1.SelectedItem).Split('-')[0] + "," + textBox2.Text + ");";
            }
            else // boya
            {
                sql = "CALL boyama_kitabi('" + textBox1.Text + "'," + comboBox2.GetItemText(comboBox2.SelectedItem).Split('-')[0] + "," + comboBox1.GetItemText(comboBox1.SelectedItem).Split('-')[0] + ",'" + textBox2.Text + "');";
            }
            dbcmd = connection.CreateCommand();
            dbcmd.CommandText = sql;
            dbcmd.ExecuteNonQuery();
            dataSetGetir("kitap");
            textBox1.Text = "";
            textBox2.Text = "";
            textBox9.Text = "";
        }

        private void button3_Click(object sender, EventArgs e)
        {
            string sql = "INSERT INTO public.yayinci(ad)VALUES('" + textBox4.Text + "'); ";
            dbcmd = connection.CreateCommand();
            dbcmd.CommandText = sql;
            dbcmd.ExecuteNonQuery();
            dataSetGetir("yayinci");
            textBox4.Text = "";
        }

        private void button4_Click(object sender, EventArgs e)
        {
            string sql = "INSERT INTO public.yazar(ad)VALUES('" + textBox3.Text + "'); ";
            dbcmd = connection.CreateCommand();
            dbcmd.CommandText = sql;
            dbcmd.ExecuteNonQuery();
            dataSetGetir("yazar");
            textBox3.Text = "";
        }

        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(comboBox3.SelectedIndex == 0)
            {
                label5.Text = "Soru Sayısı";
            }
            else if (comboBox3.SelectedIndex == 1)
            {
                label5.Text = "Tavsiye Edilen Yaş";
            }
            else if (comboBox3.SelectedIndex == 2)
            {
                label5.Text = "Tip";
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string bolge = comboBox7.GetItemText(comboBox7.SelectedItem).Split('-')[0];

            string sql = "INSERT INTO public.iletisim (adres, telefon, bolge_id) VALUES ('" + textBox7.Text + "','"+textBox5.Text+"', (SELECT id FROM public.bolge WHERE id='" + bolge + "'))";
            dbcmd = connection.CreateCommand();
            dbcmd.CommandText = sql;
            dbcmd.ExecuteNonQuery();

            sql = "INSERT INTO public.uye (ad, iletisim_id) VALUES ('" + textBox6.Text + "', (SELECT id FROM public.iletisim ORDER BY id DESC limit 1));";
            dbcmd.CommandText = sql;
            dbcmd.ExecuteNonQuery();

            dataSetGetir("uye");
            textBox5.Text = "";
            textBox6.Text = "";
            textBox7.Text = "";
        }

        private void button7_Click(object sender, EventArgs e)
        {
            string id = comboBox9.GetItemText(comboBox9.SelectedItem).Split('-')[0];
            string sql = "DELETE FROM public.yayinci WHERE id="+id;
            dbcmd = connection.CreateCommand();
            dbcmd.CommandText = sql;
            dbcmd.ExecuteNonQuery();
            dataSetGetir("yayinci");
        }

        private void button6_Click(object sender, EventArgs e)
        {
            string id = comboBox8.GetItemText(comboBox8.SelectedItem).Split('-')[0];
            string sql = "DELETE FROM public.yazar WHERE id=" + id;
            dbcmd = connection.CreateCommand();
            dbcmd.CommandText = sql;
            dbcmd.ExecuteNonQuery();
            dataSetGetir("yazar");
        }

        private void button5_Click(object sender, EventArgs e)
        {
            string id = comboBox5.GetItemText(comboBox5.SelectedItem).Split('-')[0];
            string sql = "DELETE FROM public.uye WHERE id=" + id;
            dbcmd = connection.CreateCommand();
            dbcmd.CommandText = sql;
            dbcmd.ExecuteNonQuery();
            dataSetGetir("uye");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string id = comboBox4.GetItemText(comboBox4.SelectedItem).Split('-')[0];
            string sql = "DELETE FROM public.kitap WHERE id=" + id;
            dbcmd = connection.CreateCommand();
            dbcmd.CommandText = sql;
            dbcmd.ExecuteNonQuery();
            dataSetGetir("kitap");
        }

        private void button8_Click(object sender, EventArgs e)
        {
            textBox8.Text = DateTime.Now.ToString().Substring(0, 10);
            string kitap = listBox1.GetItemText(listBox1.SelectedItem).Split('-')[0];
            string uye = listBox2.GetItemText(listBox2.SelectedItem).Split('-')[0];
            string odeme_turu = comboBox10.GetItemText(comboBox10.SelectedItem).Split('-')[0];
            string tarih = textBox8.Text;

            string sql = "CALL siparis_ver(" + uye + "," + kitap + ",'" + odeme_turu + "','" + tarih + "');";
            dbcmd = connection.CreateCommand();
            dbcmd.CommandText = sql;
            dbcmd.ExecuteNonQuery();
            siparisleriListele();
        }
    }
}
