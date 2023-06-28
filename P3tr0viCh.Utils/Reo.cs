using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace P3tr0viCh.Utils
{
#pragma warning disable IDE1006
    public static class Reo
    {
        public class WeightControl
        {
            public string id { get; set; }
            public string dateBefore { get; set; }
            public string dateAfter { get; set; }
            public string registrationNumber { get; set; }
            public string garbageTruckType { get; set; }
            public string garbageTruckBrand { get; set; }
            public string garbageTruckModel { get; set; }
            public string companyName { get; set; }
            public string companyInn { get; set; }
            public string companyKpp { get; set; }
            public int weightBefore { get; set; }
            public int weightAfter { get; set; }
            public int weightDriver { get; set; }
            public int coefficient { get; set; }
            public int garbageWeight { get; set; }
            public string garbageType { get; set; }
        }

        public class Data
        {
            public Data()
            {
                weightControls = new List<WeightControl>();
            }

            public string objectId { get; set; }
            public string accessKey { get; set; }

            public List<WeightControl> weightControls { get; }
        }
#pragma warning restore IDE1006

        public class Send
        {
            public Data Data { get; set; }

            public string Url { get; set; }

            public async Task SendAsync()
            {
                var fileNameData = Files.TempFileName("data2reo.json");
                var fileNameResponse = fileNameData + ".response.log";
                var fileNameResponseContent = fileNameData + ".response.content.log";

                File.Delete(fileNameData);
                File.Delete(fileNameResponse);
                File.Delete(fileNameResponseContent);

                File.WriteAllText(fileNameData, JsonConvert.SerializeObject(Data, Formatting.Indented));

                using (var client = new HttpClient())
                using (var multipartFormContent = new MultipartFormDataContent())
                using (var fileStreamContent = new StreamContent(File.OpenRead(fileNameData)))
                {
                    var assemblyDecorator = new Misc.AssemblyDecorator();

                    var header = new ProductHeaderValue(Files.ExecutableName(), assemblyDecorator.Version.ToString());

                    client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue(header));

                    if (assemblyDecorator.IsDebug)
                    {
                        client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("(debug build)"));
                    }

                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("*/*"));

                    multipartFormContent.Add(fileStreamContent, name: "file", fileName: "data.json");

                    using (var response = await client.PostAsync(Url, multipartFormContent))
                    {
                        File.WriteAllText(fileNameResponse, response.ToString());

                        var responseContent = await response.Content.ReadAsStringAsync();

                        File.WriteAllText(fileNameResponseContent, responseContent);

                        if (response.IsSuccessStatusCode)
                        {
                            if (!assemblyDecorator.IsDebug)
                            {
                                File.Delete(fileNameData);
                                File.Delete(fileNameResponse);
                                File.Delete(fileNameResponseContent);
                            }
                        }
                        else
                        {
                            throw new InvalidDataException(response.StatusCode.ToString());
                        }
                    }
                }
            }
        }
    }
}