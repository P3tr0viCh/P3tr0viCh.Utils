using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
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
            [Required()]
            [DisplayName("Идентификатор выгрузки ТС")]
            public string id { get; set; }

            [Required()]
            [DisplayName("Дата и время заезда ТС")]
            public DateTime? dateBefore { get; set; }

            [Required()]
            [DisplayName("Дата и время выезда ТС")]
            public DateTime? dateAfter { get; set; }

            [Required()]
            [DisplayName("Государственный регистрационный номер ТС")]
            public string registrationNumber { get; set; }

            [DisplayName("Вид ТС")]
            public string garbageTruckType { get; set; }

            [DisplayName("Марка ТС")]
            public string garbageTruckBrand { get; set; }

            [DisplayName("Модель/кузов ТС")]
            public string garbageTruckModel { get; set; }

            [Required()]
            [DisplayName("Наименование транспортирующей организации")]
            public string companyName { get; set; }

            [Required()]
            [DisplayName("ИНН организации")]
            public string companyInn { get; set; }

            [DisplayName("КПП организации")]
            public string companyKpp { get; set; }

            [Required()]
            [DisplayName("Вес ТС на въезде, кг")]
            public int weightBefore { get; set; }

            [Required()]
            [DisplayName("Вес ТС на выезде, кг ")]
            public int weightAfter { get; set; }

            [DisplayName("Вес водителя, кг")]
            public int weightDriver { get; set; }

            [Required()]
            [DisplayName("Коэффициент уплотнения мусора")]
            public float coefficient { get; set; } = 1;

            [Required()]
            [DisplayName("Вес мусора, кг")]
            public int garbageWeight { get; set; }

            [DisplayName("Вид мусора")]
            public string garbageType { get; set; }
        }

        public class Data
        {
            [Required()]
            [DisplayName("Идентификатор объекта, на котором расположен данный пункт весового контроля")]
            public string objectId { get; set; }

            [Required()]
            [DisplayName("Ключ доступа для передачи информации")]
            public string accessKey { get; set; }

            [Required()]
            [DisplayName("Информация с пункта весового контроля")]
            public List<WeightControl> weightControls { get; } = new List<WeightControl>();

            public string ToJson()
            {
                return JsonConvert.SerializeObject(this, Formatting.Indented,
                    new IsoDateTimeConverter() { DateTimeFormat = "yyyy-MM-ddTHH:mm" });
            }
        }
#pragma warning restore IDE1006

        public class CheckData
        {
            private void CheckString(string value, string field)
            {
                if (string.IsNullOrEmpty(value)) throw new ArgumentNullException(field);
            }

            private void CheckInt(int value, string field)
            {
                if (value <= 0) throw new ArgumentOutOfRangeException(field);
            }

            private void CheckFloat(float value, string field)
            {
                if (value <= 0) throw new ArgumentOutOfRangeException(field);
            }

            private void CheckDateTime(DateTime? value, string field)
            {
                if (value == null) throw new ArgumentNullException(field);
            }

            public void Check(Data data)
            {
                if (data == null) throw new ArgumentNullException();

                CheckString(data.objectId, nameof(data.objectId));
                CheckString(data.accessKey, nameof(data.accessKey));

                if (data.weightControls == null) throw new ArgumentNullException(nameof(data.weightControls));

                foreach (var weightControl in data.weightControls)
                {
                    CheckString(weightControl.id, nameof(weightControl.id));

                    CheckDateTime(weightControl.dateBefore, nameof(weightControl.dateBefore));
                    CheckDateTime(weightControl.dateAfter, nameof(weightControl.dateAfter));

                    CheckString(weightControl.registrationNumber, nameof(weightControl.registrationNumber));

                    CheckString(weightControl.companyName, nameof(weightControl.companyName));
                    CheckString(weightControl.companyInn, nameof(weightControl.companyInn));

                    CheckInt(weightControl.weightBefore, nameof(weightControl.weightBefore));
                    CheckInt(weightControl.weightAfter, nameof(weightControl.weightAfter));
                    CheckInt(weightControl.garbageWeight, nameof(weightControl.garbageWeight));

                    CheckFloat(weightControl.coefficient, nameof(weightControl.coefficient));

                    if (weightControl.weightBefore <= weightControl.weightAfter)
                        throw new ArgumentOutOfRangeException("weightBefore <= weightAfter");

                    if (weightControl.dateBefore > weightControl.dateAfter)
                        throw new ArgumentOutOfRangeException("dateBefore > dateAfter");
                }
            }
        }

        public class SendData
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

                File.WriteAllText(fileNameData, Data.ToJson());

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