using System;
using System.Collections.Generic;
using System.Linq;

namespace HighFlyers.Communication
{
    enum BufferState
    {
        WaitingForFrame = 0, // because of different compilers, different implementations...
        GotStartFrame,
        GotSpecialSign
    };

    public class ProtoBuffer
    {
        const byte StartFrame = 0x20;
        const byte SpecialSign = 0x30;
        const byte FrameDataSize = 5;

        readonly List<byte> data = new List<byte>();
        BufferState currentState = BufferState.WaitingForFrame;

        public ProtoBuffer() { }

        public ProtoBuffer(IEnumerable<byte> data)
        {
            this.data = new List<byte>(data);
        }

        public byte CalculateCrc()
        {
            if (data.Count < 1)
                throw new Exception("pusty bufor przy obliczaniu crc");

            int crc = data.Aggregate(0, (current, t) => current + t);

            return (byte)(crc % 256);
        }

        private bool CheckEndFrame(byte crc)
        {
            if (data.Count == FrameDataSize)
            {
                if (crc == CalculateCrc())
                    return true;

                Reset();
            }

            return false;
        }

        public byte[] PrepareToSend()
        {
            var output = new List<byte> {StartFrame};

            foreach (byte b in data)
            {
                if (b == StartFrame || b == SpecialSign)
                {
                    output.Add(SpecialSign);
                }

                output.Add(b);
            }

            output.Add(CalculateCrc());

            return output.ToArray<byte>();
        }

        public void Append(byte b)
        {
            switch (currentState)
            {
                case BufferState.WaitingForFrame:
                    if (b == StartFrame)
                    {
                        data.Clear();
                        currentState = BufferState.GotStartFrame;
                    }
                    break;
                case BufferState.GotStartFrame:
                    if (b == SpecialSign)
                    {
                        currentState = BufferState.GotSpecialSign;
                    }
                    else
                    {
                        bool isEnd = CheckEndFrame(b);

                        if (!isEnd)
                            data.Add(b);
                        else
                        {
                            OnFrameReady(new ProtoEventArgs(ToProto()));
                            Reset();
                        }
                    }
                    break;
                case BufferState.GotSpecialSign:
                    if (b == SpecialSign || b == StartFrame)
                    {
                        bool isEnd = CheckEndFrame(b);

                        if (!isEnd)
                        {
                            data.Add(b);
                            currentState = BufferState.GotStartFrame;
                        }
                        else
                        {
                            OnFrameReady(new ProtoEventArgs(ToProto()));
                            Reset();
                        }
                    }
                    else //bad frame syntax
                    {
                        Reset();
                    }
                    break;
            }
        }

        public Proto ToProto()
        {
            if (data.Count < FrameDataSize)
                throw new Exception("Za mało danych żeby skonwertować.");

            return new Proto(data.ToArray());
        }

        private void Reset()
        {
            currentState = BufferState.WaitingForFrame;
            data.Clear();
        }

        public event ProtoEventHandler FrameReady;

        private void OnFrameReady(ProtoEventArgs e)
        {
            if (FrameReady != null)
                FrameReady(this, e);
        }
    }
}
