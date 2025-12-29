using System;
using System.Data;
using System.IO;
using System.Text;
using System.Xml;
using static P3tr0viCh.Utils.Exceptions;

namespace P3tr0viCh.Utils
{
    public class DataTableFile
    {
        public const string DateTimeFormatXml = "yyyy-MM-ddTHH:mm:ssZ";
        public const string DateTimeFormatCsv = "yyyy-MM-dd HH:mm:ss";

        private const string ExcelXmlDateTimeFormatStyle = "yyyy/mm/dd\\ hh:mm:ss;@";
        private const string ExcelXmlDateTimeFormatStyleID = "s63";

        private const char CsvSeparatorChar = ';';

        public string FileName { get; set; }

        public DataTable Table { get; set; }

        public string Author { get; set; }

        private static string GetCellType(Type type)
        {
            //DebugWrite.Line(type.Name);

            if (type.Name == nameof(String))
            {
                return "String";
            }
            if (type.Name == nameof(Int32) ||
                type.Name == nameof(Single) ||
                type.Name == nameof(Double) ||
                type.Name == nameof(Int16) ||
                type.Name == nameof(Int64) ||
                type.Name == nameof(Decimal))
            {
                return "Number";
            }
            if (type.Name == nameof(Boolean))
            {
                return "Boolean";
            }
            if (type.Name == nameof(DateTime))
            {
                return "DateTime";
            }

            throw new NotImplementedException($"{type.Name}");
        }

        private static string GetCellStyleId(Type type)
        {
            if (type.Name == nameof(DateTime))
            {
                return ExcelXmlDateTimeFormatStyleID;
            }

            return string.Empty;
        }

        private static void XmlExcelWriteCell(XmlTextWriter xml, Type type, object value)
        {
            var typeName = GetCellType(type);

            var styleId = GetCellStyleId(type);

            xml.WriteStartElement("Cell");

            if (!styleId.IsEmpty())
            {
                xml.WriteAttributeString("ss:StyleID", styleId);
            }

            {
                xml.WriteStartElement("Data");

                xml.WriteAttributeString("ss:Type", typeName);

                if (value is DBNull)
                {
                    xml.WriteValue(string.Empty);
                }
                else
                {
                    if (value is bool b)
                    {
                        xml.WriteString(b ? "1" : "0");
                    }
                    else
                    {
                        xml.WriteValue(value);
                    }
                }

                xml.WriteEndElement();
            }

            xml.WriteEndElement();
        }

        public void WriteToExcelXml()
        {
            if (Table == null)
            {
                throw new NullReferenceException();
            }

            using (var xml = new XmlTextWriter(FileName, Encoding.UTF8))
            {
                xml.Formatting = Formatting.Indented;
                xml.Indentation = 2;

                var now = DateTime.UtcNow.ToString(DateTimeFormatXml);

                xml.WriteStartDocument();
                xml.WriteProcessingInstruction("mso-application", "progid=\"Excel.Sheet\"");

                xml.WriteStartElement("Workbook");
                {
                    xml.WriteAttributeString("xmlns", "urn:schemas-microsoft-com:office:spreadsheet");
                    xml.WriteAttributeString("xmlns:o", "urn:schemas-microsoft-com:office:office");
                    xml.WriteAttributeString("xmlns:x", "urn:schemas-microsoft-com:office:excel");
                    xml.WriteAttributeString("xmlns:ss", "urn:schemas-microsoft-com:office:spreadsheet");
                    xml.WriteAttributeString("xmlns:html", "http://www.w3.org/TR/REC-html40");
                }

                xml.WriteStartElement("DocumentProperties");
                {
                    xml.WriteAttributeString("xmlns", "urn:schemas-microsoft-com:office:office");

                    xml.WriteElementString("Author", Author);
                    xml.WriteElementString("Created", now);
                }
                xml.WriteEndElement();

                xml.WriteStartElement("Styles");
                {
                    xml.WriteStartElement("Style");
                    {
                        xml.WriteAttributeString("ss:ID", ExcelXmlDateTimeFormatStyleID);
                        xml.WriteStartElement("NumberFormat");
                        {
                            xml.WriteAttributeString("ss:Format", ExcelXmlDateTimeFormatStyle);
                        }
                        xml.WriteEndElement();
                    }
                    xml.WriteEndElement();
                }
                xml.WriteEndElement();

                xml.WriteStartElement("Worksheet");
                {
                    xml.WriteAttributeString("ss:Name", Table.TableName);
                }

                xml.WriteStartElement("Table");

                xml.WriteStartElement("Row");
                {
                    foreach (DataColumn column in Table.Columns)
                    {
                        XmlExcelWriteCell(xml, typeof(string), column.ColumnName);
                    }
                }
                xml.WriteEndElement();

                int col;

                foreach (DataRow row in Table.Rows)
                {
                    xml.WriteStartElement("Row");
                    {
                        col = 0;

                        foreach (var cell in row.ItemArray)
                        {
                            XmlExcelWriteCell(xml, Table.Columns[col].DataType, cell);

                            col++;
                        }
                    }
                    xml.WriteEndElement();
                }

                xml.WriteEndElement();

                xml.WriteEndElement();

                xml.WriteEndElement();

                xml.WriteEndDocument();

                xml.Close();
            }
        }

        private object GetValue(int col, string value)
        {
            var type = Table.Columns[col].DataType;

            if (type.Name == nameof(String))
            {
                return value;
            }
            if (type.Name == nameof(Int32))
            {
                return int.Parse(value);
            }
            if (type.Name == nameof(Single))
            {
                return Misc.FloatParseInvariant(value.Replace(',', '.'));
            }
            if (type.Name == nameof(Double))
            {
                return Misc.DoubleParseInvariant(value.Replace(',', '.'));
            }
            if (type.Name == nameof(Boolean))
            {
                return value == "1";
            }
            if (type.Name == nameof(DateTime))
            {
                return DateTime.Parse(value);
            }

            throw new NotImplementedException($"{type.Name}: {value}");
        }

        public void ReadFromExcelXml()
        {
            if (Table == null)
            {
                throw new NullReferenceException();
            }

            int col = 0;

            DataRow row = null;

            bool isData = false;

            bool isValue = false;

            using (var xml = new XmlTextReader(FileName))
            {
                while (xml.Read())
                {
                    switch (xml.NodeType)
                    {
                        case XmlNodeType.Element when xml.Name == "Row" && isData:
                            col = 0;

                            row = Table.NewRow();

                            break;
                        case XmlNodeType.EndElement when xml.Name == "Row" && !isData:
                            isData = true;

                            break;
                        case XmlNodeType.EndElement when xml.Name == "Row" && isData:
                            Table.Rows.Add(row);

                            break;
                        case XmlNodeType.EndElement when xml.Name == "Cell":
                            col++;

                            break;
                        case XmlNodeType.Element when xml.Name == "Data":
                            isValue = true;

                            break;
                        case XmlNodeType.EndElement when xml.Name == "Data":
                            isValue = false;

                            break;
                        case XmlNodeType.Text when isValue && isData:
                            row[col] = GetValue(col, xml.Value);

                            break;
                    }
                }
            }
        }

        public class CsvFileWrongHeaderException : FileBadFormatException
        {
        }

        public void ReadFromCsv()
        {
            if (Table == null)
            {
                throw new NullReferenceException();
            }

            int col;

            using (var reader = new StreamReader(FileName))
            {
                var header = reader.ReadLine();

                var columns = header.Split(';');

                if (columns.Length != Table.Columns.Count)
                {
                    throw new CsvFileWrongHeaderException();
                }

                for (var i = 0; i < columns.Length; i++)
                {
                    if (columns[i] != Table.Columns[i].ColumnName)
                    {
                        throw new CsvFileWrongHeaderException();
                    }
                }

                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine()?.TrimText();

                    if (string.IsNullOrEmpty(line)) continue;

                    col = 0;

                    var parts = line.SplitCsv(CsvSeparatorChar);

                    var row = Table.NewRow();

                    foreach (var part in parts)
                    {
                        row[col] = GetValue(col, part);
                        
                        col++;
                    }

                    Table.Rows.Add(row);
                }
            }
        }
    }
}