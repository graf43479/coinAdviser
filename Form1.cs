using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net.Http;
using System.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text.RegularExpressions;
using System.IO;
using System.Net.Http.Headers;
using System.Web;

namespace Bitcoin
{
    public partial class Form1 : Form
    {
        IList<Currency> curList = new List<Currency>();

        public Form1()
        {
            InitializeComponent();          
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                if ((!String.IsNullOrEmpty(textBoxChoosenCourse.Text)) || (!String.IsNullOrWhiteSpace(textBoxChoosenCourse.Text)))
                {
                    decimal pCourse = Decimal.Parse(textBoxChoosenCourse.Text);
                    decimal p1 = Convert.ToDecimal(textBox1.Text);
                    decimal p2 = Convert.ToDecimal(textBox2.Text);
                    decimal p3 = Convert.ToDecimal(textBox3.Text);
                    decimal p4 = Convert.ToDecimal(textBox4.Text);
                    decimal p5 = Convert.ToDecimal(textBox5.Text);

                    decimal p6 = p1 / pCourse * (1 - p3 / 100);
                    decimal p7 = pCourse * (p2 / 100 + 1);
                    decimal p8 = pCourse * (p4 / 100 + 1);
                    decimal p9 = pCourse * (p4 / 100 * p5 + 1);

                    decimal p10 = p6 * p7 * (1 - p3 / 100);
                    decimal p11 = (p10 - p1) * Convert.ToDecimal(textBoxBTCDol.Text);
                    decimal p12 = (p10 - p1) * Convert.ToDecimal(textBoxBtcRub.Text);
                    textBox6.Text = ((float)p6).ToString();
                    textBox7.Text = p7.ToString();
                    textBox8.Text = p8.ToString();
                    textBox9.Text = p9.ToString();
                    textBox10.Text = ((float)p10).ToString();
                    textBox11.Text = ((float)p11).ToString();
                    textBox12.Text = ((float)p12).ToString();
                }
                else
                {
                    MessageBox.Show("Необходимо выбрать валюту");
                }
            }
            catch(Exception ex) {
                MessageBox.Show("Что-то пошло не так! Проверьте правильность введнных данных. Ошибка: \n" + ex.Message);
            }
        }

        public async Task FastRequest()
        {
            Uri url = new Uri("https://api.cryptonator.com/api/currencies");

            
            using (var client = new HttpClient())
            {
                var response2 = await client.GetStringAsync(url);
                JObject json2 = JObject.Parse(response2);
                var response = await client.GetAsync(url);
                var result = await response.Content.ReadAsStringAsync();

                JObject json = JObject.Parse(result);
                IList<JToken> results = json["rows"].Children().ToList(); 
                foreach (JToken res in results)
                {
                    Currency searchResult = res.ToObject<Currency>();
                    curList.Add(searchResult);
                }

                foreach (Currency cur in curList)
                {
                    dataGridView1.Rows.Add(cur.Code, cur.Name);
                }
            }         
        }

        public async Task FastRequest2()
        {
            Uri url = new Uri("https://www.cryptonator.com/api/currencies");


            DataSet ds = null;
            HttpClient client = new HttpClient();
            client.BaseAddress = url;
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            HttpResponseMessage response = client.GetAsync(string.Format("api/currencies")).Result;
            var p = response;
            var p2 = p;
        }

        private async void Form1_Load(object sender, EventArgs e)
        {
            ToolTip t = new ToolTip();
            t.SetToolTip(label1, "Курс биткойна к доллару");
            t.SetToolTip(label2, "Курс биткойна к рублю");
            t.SetToolTip(label3, "Выбранная валюта");
            t.SetToolTip(label4, "Курс выбранной валюты к биткойну");
            t.SetToolTip(label5, "Сколько биткойнов тратится, в целых числах");
            t.SetToolTip(label6, "Собственный процент");
            t.SetToolTip(label7, "Комиссия, %");
            t.SetToolTip(label8, "Для STOP/LIMIT Срабатывает в случае \n увеличения цены на % от цены покупки");
            t.SetToolTip(label9, "Для STOP/LIMIT Коэфициент уменьшения \n между STOP и LIMIT");
            t.SetToolTip(label10, "Количество продаваемых/покупаемых монет(в целых числах)");
            t.SetToolTip(label11, "По какой цене нужно продать (комиссия НЕ включена)");
            t.SetToolTip(label12, "STOP");
            t.SetToolTip(label13, "LIMIT");
            t.SetToolTip(label14, "Выручка в биткойнах (чистыми без комисии)");
            t.SetToolTip(label15, "Выручка в долларах");
            t.SetToolTip(label16, "Выручка в рублях");
            //сколько BTC тратим  в целых числах

            loadSettings();

            try
            {

                await FastRequest();
                await FastBtcDolCourseRequest("btc", "rub", 2);
                await FastBtcDolCourseRequest("btc", "usd", 1);
            }
            catch {
                MessageBox.Show("Возникла проблема с подключением к списку курса валют");
            }
        }

        private void dataGridView1_RowPrePaint(object sender, DataGridViewRowPrePaintEventArgs e)
        {
            int index = e.RowIndex;
            string indexStr = (index + 1).ToString();
            object header = this.dataGridView1.Rows[index].HeaderCell.Value;
            if (header == null || !header.Equals(indexStr))
                this.dataGridView1.Rows[index].HeaderCell.Value = indexStr; 
        }


        public async Task FastBtcDolCourseRequest(string from, string to, int id)
        {
            var client = new HttpClient(); // Add: using System.Net.Http;

            var response = await client.GetAsync(new Uri("https://api.cryptonator.com/api/ticker/"+from+"-"+to));
            var result = await response.Content.ReadAsStringAsync();

          
            JObject json = JObject.Parse(result);

            JToken results = json["ticker"]; //.Children().ToList();

            Ticker search = new Ticker();

            search = results.ToObject<Ticker>();
            if (id == 2)
            {
                textBoxBtcRub.Text = ((float)search.price).ToString();
            }
            else if (id == 1)
            {
                textBoxBTCDol.Text = ((float)search.price).ToString();
            }
            else {
                textBoxChoosenCourse.Text = ((decimal)search.price).ToString();
                
            }
        }
        
        private void textBox11_TextChanged(object sender, EventArgs e)
        {
        }

        private void textBox13_TextChanged(object sender, EventArgs e)
        {
            dataGridView1.Rows.Clear();
            IList<Currency> filtered = new List<Currency>();
             filtered = curList.Where(x => x.Name.ToUpper().Contains(textBox13.Text.ToUpper()) || x.Code.ToUpper().Contains(textBox13.Text.ToUpper())).ToList();

            foreach (Currency cur in filtered)
            {
                dataGridView1.Rows.Add(cur.Code, cur.Name);
                // richTextBox1.Text = richTextBox1.Text + "\n" +cur.Name;
            }          
        }

        private async void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                textBoxChoosenCurrency.Text = "";
                textBoxChoosenCourse.Text = "";
                textBoxBTCDol.Text = "";
                textBoxBtcRub.Text = "";
                string s1 = dataGridView1.CurrentRow.Cells[1].Value.ToString();
                string s2 = dataGridView1.CurrentRow.Cells[0].Value.ToString();
                textBoxChoosenCurrency.Text = s1;

                if (s1 != "")
                {
                    await FastBtcDolCourseRequest(s2, "btc", 3);
                    await FastBtcDolCourseRequest("btc", "rub", 2);
                    await FastBtcDolCourseRequest("btc", "usd", 1);
                }
            }
            catch { }
        }

        private void loadSettings()
        {
            string path = Application.StartupPath + "/";
            string path2 = path+@"settings.json";
            if (!System.IO.File.Exists(path2))
            {
                Presettings settings = new Presettings()
                {
                    p1 = "0,063",
                    p2 = "2",
                    p3 = "0,25",
                    p4 = "0,5",
                    p5 = "0,7"
                };

                textBox1.Text = settings.p1;
                textBox2.Text = settings.p2;
                textBox3.Text = settings.p3;
                textBox4.Text = settings.p4;
                textBox5.Text = settings.p5;

                string json = JsonConvert.SerializeObject(settings, Formatting.Indented);
                System.IO.File.WriteAllText(path2, json);

            }
            else
            {
                using (StreamReader r = new StreamReader(path2))
                {
                    string json = r.ReadToEnd();
                    Presettings settings = JsonConvert.DeserializeObject<Presettings>(json);

                    textBox1.Text = settings.p1;
                    textBox2.Text = settings.p2;
                    textBox3.Text = settings.p3;
                    textBox4.Text = settings.p4;
                    textBox5.Text = settings.p5;
                }
            }
        }

        private void loadFinalSettings()
        {
            string path = Application.StartupPath + "/";
            string path2 = path + @"settings.json";

            Presettings settings = new Presettings()
            {
                p1 = textBox1.Text,
                p2 = textBox2.Text,
                p3 = textBox3.Text,
                p4 = textBox4.Text,
                p5 = textBox5.Text
            };

            string json = JsonConvert.SerializeObject(settings, Formatting.Indented);
            System.IO.File.WriteAllText(path2, json);

        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            loadFinalSettings();
        }
    }
}
