using System;
using System.Data;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Newtonsoft.Json;
using System.Windows.Controls;

namespace CurrencyConverter_API
{
    public partial class MainWindow : Window
    {
        double ConvertedCurrency;
        Root val = new Root();
        public MainWindow()
        {
            InitializeComponent();
            GetValue();
        }

        private async void GetValue ()
        {
            val = await GetData<Root>("https://openexchangerates.org/api/latest.json?app_id=0d417ffd6abf4c1abc3bb9c45ea1f1d9");
            BindCurrency();
        }
        public static async Task<Root> GetData<T>(string url)
        {
            Root myRoot = new Root();
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.Timeout = TimeSpan.FromMinutes(1);
                    HttpResponseMessage response = await client.GetAsync(url);
                    if (response.StatusCode == System.Net.HttpStatusCode.OK) 
                    { 
                        var ResponceString = await response.Content.ReadAsStringAsync();
                        var ResponceObject = JsonConvert.DeserializeObject<Root>(ResponceString);

                        //MessageBox.Show("TimeStamp: " +  ResponceObject.timestamp, "Information", MessageBoxButton.OK, MessageBoxImage.Information);

                        return ResponceObject;
                    }
                    return myRoot;
                }
            }
            catch 
            {
                return myRoot;
            }
        }

        private void BindCurrency()
        {
            DataTable dt = new DataTable();

            dt.Columns.Add("Text");
            dt.Columns.Add("Value");

            dt.Rows.Add("Select", 0);
            dt.Rows.Add("USD", val.rates.USD);
            dt.Rows.Add("EUR", val.rates.EUR);
            dt.Rows.Add("AZN", val.rates.AZN);
            dt.Rows.Add("TRY", val.rates.TRY);
            dt.Rows.Add("KRW", val.rates.KRW);


            cmb1.ItemsSource = dt.DefaultView;
            cmb1.DisplayMemberPath = "Text";
            cmb1.SelectedValuePath = "Value";
            cmb1.SelectedIndex = 0;

            cmb2.ItemsSource = dt.DefaultView;
            cmb2.DisplayMemberPath = "Text";
            cmb2.SelectedValuePath = "Value";
            cmb2.SelectedIndex = 0;
        }

        private void textbox1_TextChanged(object sender, TextChangedEventArgs e)
        {
            Convert(textbox1, textbox2, cmb1, cmb2);
        }

        private void textbox2_TextChanged(object sender, TextChangedEventArgs e)
        {
            Convert(textbox2, textbox1, cmb2, cmb1);
        }

        private void Convert(TextBox textbox1, TextBox textbox2, ComboBox cmb1, ComboBox cmb2)
        {
            if (cmb1.SelectedIndex != 0 && cmb2.SelectedIndex != 0)
            {
                if (double.TryParse(textbox1.Text, out double result))
                {
                    if (result == 0 && textbox1.IsFocused)
                    {
                        textbox2.Text = "0";
                    }
                    else if (textbox1.IsFocused)
                    {
                        ConvertedCurrency = result * double.Parse(cmb2.SelectedValue.ToString()) / double.Parse(cmb1.SelectedValue.ToString());
                        textbox2.Text = ConvertedCurrency.ToString("N2");
                    }
                }
                else if (textbox1.IsFocused)
                {
                    textbox2.Text = string.Empty;
                }
            }
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex(@"^*[0-9\.]+$");
            e.Handled = !regex.IsMatch(e.Text);
        }
    }
}
