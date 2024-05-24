using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

            public long Duration { get; set; } = 0;
            public long DurationInMove { get; set; } = 0;

            public double Distance { get; set; } = 0;

            public float EleAscent { get; set; } = 0;

            public List<Point> Points { get; set; } = null;

            public void Clear()
            {
                Text = string.Empty;

                DateTimeStart = default;
                DateTimeFinish = default;

                Duration = 0;
                DurationInMove = 0;

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

                Duration = source.Duration;
                DurationInMove = source.DurationInMove;

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

                DurationInMove = 0;
                
                Distance = 0;

                var pointPrev = Points.First();

                long pointDuration;
                double pointSpeed;

                foreach (var point in Points)
                {
                    point.Distance = Geo.Haversine(pointPrev.Lat, pointPrev.Lng, point.Lat, point.Lng);

                    Distance += point.Distance;

                    pointDuration = (long)(point.DateTime - pointPrev.DateTime).TotalSeconds;
                    
                    if (point.Distance > 0 && pointDuration > 0)
                    {
                        pointSpeed = point.Distance / pointDuration;
                    }
                    else
                    {
                        pointSpeed = 0;
                    }

                    if (pointSpeed > 0.1 && pointDuration < 60)
                    {
                        DurationInMove += pointDuration;
                    }

                    if (pointSpeed > 0.1 && point.Ele > pointPrev.Ele)
                    {
                        EleAscent += point.Ele - pointPrev.Ele;
                    }

                    pointPrev = point;
                }

                DateTimeStart = Points.First().DateTime;
                DateTimeFinish = Points.Last().DateTime;

                Duration = (long)(DateTimeFinish - DateTimeStart).TotalSeconds;
            }
        }
    }
}