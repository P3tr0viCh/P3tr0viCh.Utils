using System;
using System.Globalization;
using System.IO.Ports;

namespace P3tr0viCh.Utils
{
    public class ScaleTerminal
    {
        public enum TerminalType
        {
            None,
            Newton42,
            MidlVda12,
            MicrosimM0601,
            TokvesSh50,
        }

        public enum MassUnit
        {
            kg,
            tn,
        }

        public class LineEventArgs : EventArgs
        {
            public LineEventArgs(string line)
            {
                Line = line;
            }

            public string Line { get; private set; } = string.Empty;
        }

        public class WeightEventArgs : EventArgs
        {
            public WeightEventArgs(int weight)
            {
                Weight = weight;
            }

            public int Weight { get; set; } = 0;
        }

        private readonly SerialPort serialPort = new SerialPort();
        public SerialPort SerialPort => serialPort;

        public delegate void LineReceivedEventHandler(object sender, LineEventArgs e);
        public event LineReceivedEventHandler LineReceived;

        public delegate void WeightReceivedEventHandler(object sender, WeightEventArgs e);
        public event WeightReceivedEventHandler WeightReceived;

        private delegate int GetWeight(string line);
        private GetWeight getWeight;

        public ScaleTerminal()
        {
            serialPort.DataReceived += SerialPortDataReceived;
        }


        private TerminalType terminalType = TerminalType.None;
        public TerminalType Type
        {
            get
            {
                return terminalType;
            }
            set
            {
                if (terminalType == value) return;

                var prevStateIsOpen = IsOpen;

                Close();

                terminalType = value;

                switch (terminalType)
                {
                    case TerminalType.Newton42:
                        serialPort.NewLine = "\x0D";

                        getWeight = new GetWeight(GetWeightNewton);

                        break;
                    case TerminalType.MidlVda12:
                        serialPort.NewLine = "\x0A";

                        getWeight = new GetWeight(GetWeightMidlVda12);

                        break;
                    case TerminalType.MicrosimM0601:
                        serialPort.NewLine = "\x0A";

                        getWeight = new GetWeight(GetWeightMicrosimM0601);

                        break;
                    case TerminalType.TokvesSh50:
                        serialPort.NewLine = "\x3D";

                        getWeight = new GetWeight(GetWeightTokvesSh50);

                        break;
                    default:
                        serialPort.NewLine = string.Empty;

                        getWeight = null;

                        return;
                }

                if (prevStateIsOpen)
                {
                    Open();
                }
            }
        }

        public MassUnit TerminalMassUnit { get; set; }

        private int GetWeightFromLine(string line, int startIndex, int length)
        {
            try
            {
                var weight = line.Substring(startIndex, length).Trim();

                if (weight.Length == 0) return 0;

                if (float.TryParse(weight, NumberStyles.Float | NumberStyles.AllowThousands,
                    CultureInfo.InvariantCulture, out float f))
                {
                    switch (TerminalMassUnit)
                    {
                        case MassUnit.tn:
                            return Convert.ToInt32(f * 1000);
                        case MassUnit.kg:
                            return Convert.ToInt32(f);
                        default:
                            return 0;
                    }
                }
                else
                {
                    return 0;
                }
            }
            catch (Exception)
            {
                return 0;
            }
        }

        private int GetWeightNewton(string line)
        {
            // Ньютон-42
            //     0.00 G .
            // 7F 20 20 20 20 30 2E 30 30 20 47 20 0D

            return GetWeightFromLine(line, 2, 7);
        }

        private int GetWeightMidlVda12(string line)
        {
            // Мидл ВДА/12Я
            // WW0000.00kg..
            // WW00004.0kg..
            // 57 57 30 30 30 30 2E 30 30 6B 67 0D 0A

            return GetWeightFromLine(line, 2, 7);
        }

        private int GetWeightMicrosimM0601(string line)
        {
            // Микросим М0601
            // Б_172.60_B 
            // 81 20 20 31 37 32 2E 36 30 20 42 20 20 0D 0A

            return GetWeightFromLine(line, 2, 7);
        }

        private int GetWeightTokvesSh50(string line)
        {
            // Токвес SH-50
            // =____2.85B0
            // 3D 20 20 20 20 32 2E 38 35 42 30 

            return GetWeightFromLine(line, 1, 7);
        }

        public string PortName => serialPort.PortName;

        public bool IsOpen => serialPort.IsOpen;

        public void Open()
        {
            if (serialPort.IsOpen) return;

            serialPort.Open();

            serialPort.DiscardInBuffer();
            serialPort.DiscardOutBuffer();
        }

        public void Close()
        {
            if (!serialPort.IsOpen) return;

            serialPort.DiscardInBuffer();
            serialPort.DiscardOutBuffer();

            serialPort.Close();
        }

        private void SerialPortDataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                DataReceived(serialPort.ReadLine());
            }
            catch (Exception)
            {
            }
        }

        private void DataReceived(string line)
        {
            LineReceived?.Invoke(this, new LineEventArgs(line));

            WeightReceived?.Invoke(this, new WeightEventArgs(getWeight(line)));
        }
    }
}