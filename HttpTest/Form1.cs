using System.Diagnostics;

namespace HttpTest
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            QueryUrl("http://ektaliving.com/");

            var n1 = treeView1.Nodes.Add("key", "Text");
            n1.Nodes.Add("sub1");
            n1.Nodes.Add("sub2", "text2");
            var n2 = treeView1.Nodes.Add("key2", "Text2");
        }

        private void test() { 
            var handler = new HttpClientHandler()
            {
                AllowAutoRedirect = false  // Disables automatic redirect following
            };

            HttpClient client = new HttpClient(handler);

            var req = new HttpRequestMessage(HttpMethod.Get, "http://ektaliving.com/");
            var sw = Stopwatch.StartNew();
            var response = client.Send(req);
            sw.Stop();

            Debug.WriteLine($"StatusCode: {(int)response.StatusCode} {response.StatusCode} - took {sw.ElapsedMilliseconds} ms");
            if ((int)response.StatusCode >= 300 && (int)response.StatusCode < 400)
            {
                // Step 5: Get the Location header to find the redirect URL
                if (response.Headers.Location != null)
                {
                    Debug.WriteLine($"Redirect to: {response.Headers.Location}");
                }
                else
                {
                    Console.WriteLine("Redirect without a Location header.");
                }
            }
        }

        private void QueryUrl(string url)
        {
            Debug.WriteLine($"Querying url: '{url}' ...");
            var handler = new HttpClientHandler()
            {
                AllowAutoRedirect = false  // Disables automatic redirect following
            };

            HttpClient client = new HttpClient(handler);

            var req = new HttpRequestMessage(HttpMethod.Get, url);
            var sw = Stopwatch.StartNew();
            var response = client.Send(req);
            sw.Stop();

            Debug.WriteLine($"Got response: {(int)response.StatusCode} {response.StatusCode} - took {sw.ElapsedMilliseconds} ms, returned {response.Content.Headers.ContentLength/1024} kb");
            if ((int)response.StatusCode >= 300 && (int)response.StatusCode < 400)
            {
                // Step 5: Get the Location header to find the redirect URL
                if (response.Headers.Location != null)
                {
                    Debug.WriteLine($"Redirect to: {response.Headers.Location}");
                    QueryUrl(response.Headers.Location.ToString());
                }
                else
                {
                    Debug.WriteLine("Redirect without a Location header.");
                }
            }

        }
    }
}
