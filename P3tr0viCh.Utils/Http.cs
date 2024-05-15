using System.Net.Http.Headers;
using System.Net.Http;
using System;

namespace P3tr0viCh.Utils
{
    public static class Http
    {
        public static void SetClientBaseAddress(HttpClient client, string address)
        {
            client.BaseAddress = new Uri(address);
        }

        public static void SetClientHeader(HttpClient client)
        {
            var assemblyDecorator = new Misc.AssemblyDecorator();

            var header = new ProductHeaderValue(Files.ExecutableName(), assemblyDecorator.Version.ToString());

            client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue(header));

            if (assemblyDecorator.IsDebug)
            {
                client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("(debug build)"));
            }
        }

        public static void SetClientMediaType(HttpClient client, string mediaType)
        {
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(mediaType));
        }
    }
}