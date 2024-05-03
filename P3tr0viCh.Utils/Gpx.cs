using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;

namespace P3tr0viCh.Utils
{
    public static class Gpx
    {
        public const string DATETIME_FORMAT_GPX = "yyyy-MM-ddTHH:mm:ssZ";

        public class Point
        {
            public int Num { get; set; }

            public DateTime DateTime { get; set; }

            public double Lat { get; set; }
            public double Lng { get; set; }

            public float Ele { get; set; }

            public double Distance { get; set; }
        }

        public class Track
        {
            public string Text { get; set; } = string.Empty;

            public DateTime DateTimeStart { get; set; } = default;
            public DateTime DateTimeFinish { get; set; } = default;

            public string DurationAsString => (DateTimeFinish - DateTimeStart).ToHoursMinutesString();

            public double Distance { get; set; } = 0;
            
            public float EleAscent { get; set; } = 0;

            public List<Point> Points { get; set; } = null;

            public void Clear()
            {
                Text = string.Empty;

                DateTimeStart = default;
                DateTimeFinish = default;

                Distance = 0;

                EleAscent = 0;

                Points = null;
            }

            public void Assign(Track source)
            {
                if (source == null)
                {
                    Clear();

                    return;
                }

                Text = source.Text;

                DateTimeStart = source.DateTimeStart;
                DateTimeFinish = source.DateTimeFinish;

                Distance = source.Distance;

                EleAscent = source.EleAscent;

                if (source.Points == null)
                {
                    Points = null;
                }
                else
                {
                    if (Points == null)
                    {
                        Points = new List<Point>();
                    }
                    else
                    {
                        Points.Clear();
                    }

                    Points.AddRange(source.Points);
                }
            }

            private static string XmlGetText(XmlNode node)
            {
                return node != null ? node.InnerText : string.Empty;
            }

            public void OpenFromFile(string path)
            {
                var trackXml = new XmlDocument();

                trackXml.Load(path);

                var trkname = XmlGetText(trackXml.DocumentElement["trk"]?["name"]);

                if (string.IsNullOrWhiteSpace(trkname))
                {
                    trkname = XmlGetText(trackXml.DocumentElement["metadata"]?["name"]);

                    if (string.IsNullOrWhiteSpace(trkname))
                    {
                        trkname = Path.GetFileNameWithoutExtension(path);
                    }
                }

                Text = trkname;

                Points = new List<Point>();

                var trkptList = trackXml.GetElementsByTagName("trkpt");

                var num = 0;

                foreach (XmlNode trkpt in trkptList)
                {
                    if (trkpt.Attributes["lat"] != null && trkpt.Attributes["lon"] != null)
                    {
                        Points.Add(new Point()
                        {
                            Num = num++,

                            Lat = Misc.DoubleParseInvariant(trkpt.Attributes["lat"].Value),
                            Lng = Misc.DoubleParseInvariant(trkpt.Attributes["lon"].Value),

                            DateTime = Misc.DateTimeParse(XmlGetText(trkpt["time"]), DATETIME_FORMAT_GPX),

                            Ele = Misc.FloatParseInvariant(XmlGetText(trkpt["ele"]))
                        });
                    }
                }

                if (Points.Count < 2)
                {
                    throw new Exception("empty track");
                }

                Distance = 0;

                var pointPrev = Points.First();

                float ele1 = pointPrev.Ele;
                float ele2;

                foreach (var point in Points)
                {
                    point.Distance = Geo.Haversine(pointPrev.Lat, pointPrev.Lng, point.Lat, point.Lng);

                    Distance += point.Distance;

                    pointPrev = point;

                    ele2 = point.Ele;

                    if (ele2 > ele1)
                    {
                        EleAscent += ele2 - ele1;
                    }

                    ele1 = ele2;
                }

                DateTimeStart = Points.First().DateTime;
                DateTimeFinish = Points.Last().DateTime;

                if (DateTimeStart == default)
                {
                    DateTimeStart = Misc.DateTimeParse(
                        XmlGetText(trackXml.DocumentElement["metadata"]?["time"]), DATETIME_FORMAT_GPX, DateTime.Now);
                }
                if (DateTimeFinish == default)
                {
                    DateTimeFinish = DateTimeStart;
                }
            }
        }
    }
}