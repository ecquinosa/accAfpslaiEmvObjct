using System;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using accAfpslaiEmvObjct.CBS.Core.Message;

namespace accAfpslaiEmvObjct.CBS
{
    public class ConsolSys
    {
        static HttpClient client = new HttpClient();
        static string cbsUrl = "voyageruatcore.afpslai.com.ph:1010";
        static string tranCode = "4351";
        static string userName = "CMSUser";
        static string sequenceNo = "111";
        static string channel = "CMS";
        static string branchCode = "100";
        static string cif = "2012720001757";

        public void Main()
        {
            RunAsync().GetAwaiter().GetResult();
        }

        static async Task<Uri> PostInquiry(TransactionServiceMessageBase message)
        {
            HttpResponseMessage response = null;
            try
            {
                response = await client.PostAsJsonAsync("api/transaction/runtransaction/", message);
                var content = response.Content.ReadAsStringAsync().Result;
                Console.WriteLine(content);
                response.EnsureSuccessStatusCode();

            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine(ex.InnerException.ToString());
                Console.WriteLine(ex.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            // return URI of the created resource.
            return response?.Headers?.Location;
        }

        static async Task RunAsync()
        {
            try
            {
                var address = $"http://{cbsUrl}/";
                client.BaseAddress = new Uri(address);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.TryAddWithoutValidation("TranCode", tranCode ?? string.Empty);
                client.DefaultRequestHeaders.TryAddWithoutValidation("Username", userName ?? string.Empty);
                client.DefaultRequestHeaders.TryAddWithoutValidation("SeqNo", sequenceNo);
                client.DefaultRequestHeaders.TryAddWithoutValidation("TimeStamp", "1");
                client.DefaultRequestHeaders.TryAddWithoutValidation("Salt", GenerateSalt());
                client.DefaultRequestHeaders.TryAddWithoutValidation("Channel", channel ?? string.Empty);


                // Create a new host message ...
                var message = new CMSCIFDetailInquiryMessage
                {
                    Header = new TransactionMessageHeader
                    {
                        SourceId = "",
                        TranCode = "",
                        ReferenceID = new Guid().ToString(),
                        BusinessDate = DateTime.Now,
                        TransactionStatus = 0,
                        ProcessingMode = 0,
                        UserId = "",
                        BranchCode = branchCode ?? string.Empty,
                        SequenceNo = 100
                    },
                    CIF_NO = cif ?? string.Empty
                };

                var url = await PostInquiry(message);

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            Console.ReadLine();
        }

        static string GenerateSalt()
        {
            var salt =
            client.DefaultRequestHeaders.GetValues("Username").Single() + "84A47863-BDD5-4949-B364-DD2C993FBE08" + "SPICY" + client.DefaultRequestHeaders.GetValues("SeqNo").Single() + client.DefaultRequestHeaders.GetValues("TimeStamp").Single();

            var sha = System.Security.Cryptography.SHA256.Create();

            var hashed = sha.ComputeHash(Encoding.UTF8.GetBytes(salt));


            var binaryHashed = String.Empty;
            for (var x = 0; x < hashed.Length; x++)
            {
                binaryHashed = binaryHashed + String.Format("{0:x2}", hashed[x]);

            }

            return binaryHashed;
        }
    }
}
