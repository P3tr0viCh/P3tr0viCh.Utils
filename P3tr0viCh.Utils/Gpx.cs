using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;

namespace P3tr0viCh.Utils
{
    public static class Gpx
    {
        public class Point
        {
            public int Num { get; set; } = 0;

            public DateTime DateTime { get; set; } = default;

            public double Lat { get; set; } = default;
            public double Lng { get; set; } = default;

            public double Ele { get; set; } = 0;

            public double Distance { get; set; } = 0;

            public void Clear()
            {
                Num = 0;

                DateTime = default;

                Lat = default;
                Lng = default;

                Ele = 0;

                Distance = 0;
            }

            public void Assign(Point source)
            {
                if (source == null)
                {
                    Clear();

                    return;
                }

                Num = source.Num;

                DateTime = source.DateTime;

                Lat = source.Lat;
                Lng = source.Lng;

                Ele = source.Ele;

                Distance = source.Distance;
            }
        }

        public class Track
        {
            public string Text { get; set; } = string.Empty;

            public DateTime DateTimeStart { get; set; } = default;
            public DateTime DateTimeFinish { get; set; } = default;

            public long Duration { get; set; } = 0;

            private long durationInMove = 0;
            public long DurationInMove
            {
                get => durationInMove;
                set
                {
                    durationInMove = value;

                    AverageSpeed = GetAverageSpeed();
                }
            }

            private double distance = 0;
            public double Distance
            {
                get => distance;
                set
                {
                    distance = value;

                    AverageSpeed = GetAverageSpeed();
                }
            }

            public double AverageSpeed { get; private set; } = 0;

            public double EleAscent { get; set; } = 0;
            public double EleDescent { get; set; } = 0;

            private bool calcEleAscent = true;
            private bool calcEleDescent = true;

            public List<Point> Points { get; set; } = null;

            private float GetAverageSpeed()
            {
                if (distance == 0 || durationInMove == 0)
                {
                    return 0;
                }

                return (float)(distance / durationInMove);
            }

            public void Clear()
            {
                Text = string.Empty;

                DateTimeStart = default;
                DateTimeFinish = default;

                Duration = 0;
                DurationInMove = 0;

                Distance = 0;

                AverageSpeed = 0;

                EleAscent = 0;
                EleDescent = 0;

                calcEleAscent = true;
                calcEleDescent = true;

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

                AverageSpeed = source.AverageSpeed;

                EleAscent = source.EleAscent;
                calcEleAscent = source.calcEleAscent;

                EleDescent = source.EleDescent;
                calcEleDescent = source.calcEleDescent;

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

            public const string ElementNameTrk = "trk";
            public const string ElementNameTrackName = "name";
            public const string ElementNameTrkPt = "trkpt";
            public const string ElementNameTrkPtLat = "lat";
            public const string ElementNameTrkPtLon = "lon";
            public const string ElementNameTrkPtTime = "time";
            public const string ElementNameTrkPtEle = "ele";
            public const string ElementNameMetadata = "metadata";
            public const string ElementNameExtensions = "extensions";
            public const string ElementNameEleAscent = "eleascent";
            public const string ElementNameEleDescent = "eledescent";

            public void OpenFromFile(string path)
            {
                var trackXml = new XmlDocument();

                trackXml.Load(path);

                var trkname = XmlGetText(trackXml.DocumentElement[ElementNameTrk]?[ElementNameTrackName]);

                if (trkname.IsEmpty())
                {
                    trkname = XmlGetText(trackXml.DocumentElement[ElementNameMetadata]?[ElementNameTrackName]);

                    if (trkname.IsEmpty())
                    {
                        trkname = Path.GetFileNameWithoutExtension(path);
                    }
                }

                Text = trkname;

                var eleAscentText = XmlGetText(trackXml.DocumentElement[ElementNameTrk]?[ElementNameExtensions]?[ElementNameEleAscent]);
                var eleDescentText = XmlGetText(trackXml.DocumentElement[ElementNameTrk]?[ElementNameExtensions]?[ElementNameEleDescent]);

                calcEleAscent = eleAscentText.IsEmpty();
                calcEleDescent = eleDescentText.IsEmpty();

                if (!calcEleAscent)
                {
                    EleAscent = Misc.FloatParseInvariant(eleAscentText);
                }
                if (!calcEleDescent)
                {
                    EleDescent = Misc.FloatParseInvariant(eleDescentText);
                }

                Points = new List<Point>();

                var trkptList = trackXml.GetElementsByTagName(ElementNameTrkPt);

                var num = 0;

                foreach (XmlNode trkpt in trkptList)
                {
                    if (trkpt.Attributes[ElementNameTrkPtLat] != null && trkpt.Attributes[ElementNameTrkPtLon] != null)
                    {
                        Points.Add(new Point()
                        {
                            Num = num++,

                            Lat = Misc.DoubleParseInvariant(trkpt.Attributes[ElementNameTrkPtLat].Value),
                            Lng = Misc.DoubleParseInvariant(trkpt.Attributes[ElementNameTrkPtLon].Value),

                            DateTime = Misc.DateTimeParse(XmlGetText(trkpt[ElementNameTrkPtTime])),

                            Ele = Misc.FloatParseInvariant(XmlGetText(trkpt[ElementNameTrkPtEle]))
                        });
                    }
                }

                NotifyPointsChanged();
            }

            public async Task OpenFromFileAsync(string path)
            {
                await Task.Factory.StartNew(() =>
                {
                    OpenFromFile(path);
                });
            }

            public void NotifyPointsChanged()
            {
                if (Points.Count < 2)
                {
                    throw new Exception("empty track");
                }

                DurationInMove = 0;

                Distance = 0;

                AverageSpeed = 0;

                if (calcEleAscent)
                {
                    EleAscent = 0;
                }
                if (calcEleDescent)
                {
                    EleDescent = 0;
                }

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

                    if (calcEleAscent)
                    {
                        if (pointSpeed > 0.1 && point.Ele > pointPrev.Ele)
                        {
                            EleAscent += point.Ele - pointPrev.Ele;
                        }
                    }
                    if (calcEleDescent)
                    {
                        if (pointSpeed > 0.1 && point.Ele < pointPrev.Ele)
                        {
                            EleDescent += pointPrev.Ele - point.Ele;
                        }
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