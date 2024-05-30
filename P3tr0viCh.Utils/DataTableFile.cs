using System.Data;
using System.Xml;
using System.Text;
using System;

namespace P3tr0viCh.Utils
{
    public class DataTableFile
    {
        public const string DATETIME_FORMAT_XML = "yyyy-MM-ddTHH:mm:ssZ";

        private const string EXCEL_XML_DATETIME_FORMAT_STYLE = "yyyy/mm/dd\\ hh:mm:ss;@";
        private const string EXCEL_XML_DATETIME_FORMAT_STYLE_ID = "s63";

        public string FileName { get; set; }

        public DataTable Table { get; set; }

        public string Author { get; set; }

        private static void XmlExcelWriteCell(XmlTextWriter xml, object value)
        {
            string type;

            string styleId = string.Empty;

            if (value is int || value is long || value is float || value is double || value is decimal)
            {
                type = "Number";
            }
            else
            {
                if (value is bool)
                {
                    type = "Boolean";
                }
                else
                {
                    if (value is DateTime)
                    {
                        type = "DateTime";
                        styleId = EXCEL_XML_DATETIME_FORMAT_STYLE_ID;
                    }
                    else
                    {
                        type = "String";
                    }
                }
            }

            xml.WriteStartElement("Cell");

            if (!styleId.IsEmpty())
            {
                xml.WriteAttributeString("ss:StyleID", styleId);
            }

            {
                xml.WriteStartElement("Data");

                xml.WriteAttributeString("ss:Type", type);

                if (value is bool b)
                {
                    xml.WriteString(b ? "1" : "0");
                }
                else
                {
                    xml.WriteValue(value);
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

                var now = DateTime.UtcNow.ToString(DATETIME_FORMAT_XML);

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
                        xml.WriteAttributeString("ss:ID", EXCEL_XML_DATETIME_FORMAT_STYLE_ID);
                        xml.WriteStartElement("NumberFormat");
                        {
                            xml.WriteAttributeString("ss:Format", EXCEL_XML_DATETIME_FORMAT_STYLE);
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
                    foreach (DataColumn col in Table.Columns)
                    {
                        XmlExcelWriteCell(xml, col.ColumnName);
                    }
                }
                xml.WriteEndElement();

                foreach (DataRow row in Table.Rows)
                {
                    xml.WriteStartElement("Row");
                    {
                        foreach (var cell in row.ItemArray)
                        {
                            XmlExcelWriteCell(xml, cell);
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
    }
}