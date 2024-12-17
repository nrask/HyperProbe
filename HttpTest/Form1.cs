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
            // todo: "canonical name check" funktionalitet
            // todo: hvad nu hvis den timer ud. lav bedre fejl håndtering

            treeView1.Nodes.Clear();
            foreach (var _url in textBox1.Lines)
            {
                var url = _url;
                if (!url.StartsWith("http"))
                {
                    url = "http://" + url;
                }
                //string url = "http://ektaliving.com/";
                var node = treeView1.Nodes.Add("url", url);
                QueryUrl(url, node);
                node.Expand();
            }

            //var n1 = treeView1.Nodes.Add("key", "Text");
            //n1.Nodes.Add("sub1");
            //n1.Nodes.Add("sub2", "text2");
            //var n2 = treeView1.Nodes.Add("key2", "Text2");
        }

        private void test()
        {
            var handler = new HttpClientHandler()
            {
                AllowAutoRedirect = false  // Disables automatic redirect following
         
            };

            HttpClient client = new HttpClient(handler);

            var req = new HttpRequestMessage(HttpMethod.Get, "http://ektaliving.com/");
            //req.Headers.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:132.0) Gecko/20100101 Firefox/132.0";
            req.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:132.0) Gecko/20100101 Firefox/132.0");
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

        private void QueryUrl(string url, TreeNode node)
        {
            Debug.WriteLine($"Querying url: '{url}' ...");
            var handler = new HttpClientHandler()
            {
                AllowAutoRedirect = false  // Disables automatic redirect following
            };

            HttpClient client = new HttpClient(handler);

            var x = node.Nodes.Add($"Url: {url}");

            //user-agents: https://developer.mozilla.org/en-US/docs/Web/HTTP/Headers/User-Agent
            //https://www.whatismybrowser.com/detect/what-is-my-user-agent/
            var req = new HttpRequestMessage(HttpMethod.Get, url);
            req.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:132.0) Gecko/20100101 Firefox/132.0");// reddit.com giver 403 hvis vi ikke ændrer ua fra default
            var sw = Stopwatch.StartNew();
            var response = client.Send(req);
            sw.Stop();
            x.Text += $" -> ({(int)response.StatusCode})";

            x.Nodes.Add($"Response code: {(int)response.StatusCode} {response.StatusCode}");

            Debug.WriteLine($"Got response: {(int)response.StatusCode} {response.StatusCode} - took {sw.ElapsedMilliseconds} ms, returned {response.Content.Headers.ContentLength / 1024} kb");
            if ((int)response.StatusCode >= 300 && (int)response.StatusCode < 400)
            {
                // Step 5: Get the Location header to find the redirect URL
                if (response.Headers.Location != null)
                {
                    x.Nodes.Add($"Destination: {response.Headers.Location}"); 
                    Debug.WriteLine($"Redirect to: {response.Headers.Location}");
                    //NOTE: reddit.com kan finde på at give os en relativ location. det skal vi kunne håndtere
                    QueryUrl(response.Headers.Location.ToString(), node);
                }
                else
                {
                    Debug.WriteLine("Redirect without a Location header.");
                }
            }
            x.Nodes.Add($"Time taken: {sw.ElapsedMilliseconds} ms");
            x.Nodes.Add($"Response length: {response.Content.Headers.ContentLength / 1024} kb");
            var h = x.Nodes.Add("Headers");
            foreach (var item in response.Headers)
            {
                h.Nodes.Add($"{item.Key}: {string.Join(",", item.Value.Select(x => $"'{x}'"))}");
            }

        }
    }
}
