using System;
using System.Globalization;
using System.IO.Ports;
using System.Threading.Tasks;

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

        public delegate void WeightReceivedEventHandler(object sender, WeightEventArgs e);

        public event WeightReceivedEventHandler WeightReceived;

        private TerminalType terminalType;

        private delegate int GetWeight(string line);
        private GetWeight getWeight;

        public ScaleTerminal()
        {
            //this.container = container;

            serialPort.DataReceived += SerialPortDataReceived;
        }

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

        private enum MassUnit
        {
            tn,
            kg
        }

        private int GetWeightFromLine(string line, int startIndex, int length, MassUnit massUnit)
        {
            try
            {
                var weight = line.Substring(startIndex, length);

                switch (massUnit)
                {
                    case MassUnit.tn:
                        return float.TryParse(weight, NumberStyles.Float | NumberStyles.AllowThousands,
                            CultureInfo.InvariantCulture, out float tn) ? (int)tn * 1000 : 0;
                    case MassUnit.kg:
                        return int.TryParse(weight, NumberStyles.Integer,
                            CultureInfo.InvariantCulture, out int kg) ? kg : 0;
                    default:
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

            return GetWeightFromLine(line, 2, 7, MassUnit.tn);
        }

        private int GetWeightMidlVda12(string line)
        {
            // Мидл ВДА/12Я
            // WW0000.00kg..
            // WW00004.0kg..
            // 57 57 30 30 30 30 2E 30 30 6B 67 0D 0A

            return GetWeightFromLine(line, 2, 7, MassUnit.kg);
        }

        private int GetWeightMicrosimM0601(string line)
        {
            // Микросим М0601
            // Б 172.60 B 
            // 81 20 20 31 37 32 2E 36 30 20 42 20 20 0D 0A

            return GetWeightFromLine(line, 2, 7, MassUnit.tn);
        }

        public string PortName => serialPort.PortName;

        public bool IsOpen => serialPort.IsOpen;

        public void Open()
        {
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
                LineReceived(serialPort.ReadLine());
            }
            catch (Exception)
            {
            }
        }

        private async void LineReceived(string line)
        {
            await Task.Run(() =>
             WeightReceived?.Invoke(this, new WeightEventArgs(getWeight(line))));
        }
    }
}