using System.Text.Json;
using System.Net.Http;
using System.Windows.Forms;
using System.Security.Policy;
using System.Text.Json.Nodes;
using System.Text;

namespace SendPCImfo.App
{
    internal class Program
    {
        static void Main(string[] args)
        {
            PCInfo _PCInfo = null;
            string jsonRequest = "";
            string url = "http://localhost:4200/api/_pcInf/Post";

            try
            {
                _PCInfo = new PCInfo();
            }
            catch
            {
                MessageBox.Show("Произошла ошибка чтения данных компьютера");
                return;
            }

            jsonRequest = JsonSerializer.Serialize(_PCInfo);
            ////Console.WriteLine(jsonRequest);
            ////Console.ReadLine();

            HttpClient client = new HttpClient();

            //var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");
            //var result = client.PostAsync(url, content).Result;

            MessageBox.Show("Данные отправлены на сервер");

        }
        
    }
}
