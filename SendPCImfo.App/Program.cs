using System.Text.Json;
using System.Net.Http;
using System.Windows.Forms;
using System.Text;
using System.Net;

namespace SendPCImfo.App
{
    internal class Program
    {
        static void Main(string[] args)
        {
            ServicePointManager.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true;
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
        
            PCInfo _PCInfo = null;
            string jsonObject = "";
            string url = "https://localhost:7032/api/_pcInf/Post";

            try
            {
                _PCInfo = new PCInfo();
            }
            catch
            {
                MessageBox.Show("Произошла ошибка чтения данных компьютера");
                return;
            }

            jsonObject = JsonSerializer.Serialize(_PCInfo);

            HttpClient httpClient = new HttpClient();

            try
            {
                var response = httpClient.PostAsJsonAsync(url, _PCInfo);
                MessageBox.Show("Данные успешно отправленны в хранилище на сервер!");
                MessageBox.Show("Произошла ошибка при отправке данных !");
                return;
            }
            catch (System.Exception)
            {
                MessageBox.Show("Произошла ошибка при отправке данных !");
            }
        }
        
    }
}
