using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;
using Npgsql;

namespace KitapUygulamasi
{
    public partial class Form1 : Form
    {
        private NpgsqlConnection conn;

        public Form1()
        {
            InitializeComponent();
            conn = new NpgsqlConnection("Host=localhost;Port=5432;Username=postgres;Password=nergis;Database=kutuphane;");
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            RefreshKitapListesi();
        }

        private void RefreshKitapListesi()
        {
            try
            {
                conn.Open();
                string query = "SELECT id, kitap_adi FROM kitaplar WHERE mevcut = true";
                NpgsqlCommand cmd = new NpgsqlCommand(query, conn);
                NpgsqlDataReader reader = cmd.ExecuteReader();

                List<Tuple<int, string>> kitaplar = new List<Tuple<int, string>>();
                while (reader.Read())
                {
                    int id = Convert.ToInt32(reader["id"]);
                    string kitapAdi = reader["kitap_adi"].ToString();
                    kitaplar.Add(new Tuple<int, string>(id, kitapAdi));
                }

                listBoxKitaplar.DisplayMember = "Item2";
                listBoxKitaplar.ValueMember = "Item1";
                listBoxKitaplar.DataSource = kitaplar;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
            finally
            {
                conn.Close();
            }
        }

     


        private void listBoxKitaplar_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void btnEkle_Click_1(object sender, EventArgs e)
        {

            string kitapAdi = txtKitapAdi.Text;
            string yazar = txtYazar.Text;
            string isbn = txtISBN.Text;

            try
            {
                conn.Open();
                string query = "INSERT INTO kitaplar (kitap_adi, yazar, mevcut, \"ISBN\") VALUES (@kitapAdi, @yazar, true, @isbn)";
                NpgsqlCommand cmd = new NpgsqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@kitapAdi", kitapAdi);
                cmd.Parameters.AddWithValue("@yazar", yazar);
                cmd.Parameters.AddWithValue("@isbn", isbn);
                int rowsAffected = cmd.ExecuteNonQuery();
                if (rowsAffected > 0)
                {
                    MessageBox.Show("Kitap başarıyla eklendi.");
                    RefreshKitapListesi();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
            finally
            {
                conn.Close();
            }
        }

        private void btnSil_Click_1(object sender, EventArgs e)
        {
            if (listBoxKitaplar.SelectedIndex != -1)
            {
                int kitapId = (listBoxKitaplar.SelectedItem as Tuple<int, string>).Item1;
                try
                {
                    conn.Open();
                    string query = "DELETE FROM kitaplar WHERE id = @id";
                    NpgsqlCommand cmd = new NpgsqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@id", kitapId);
                    int rowsAffected = cmd.ExecuteNonQuery();
                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Kitap başarıyla silindi.");
                        RefreshKitapListesi();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
                finally
                {
                    conn.Close();
                }
            }
            else
            {
                MessageBox.Show("Lütfen silmek istediğiniz bir kitap seçin.");
            }
        }
    }
}
