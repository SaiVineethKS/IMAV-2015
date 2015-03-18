using System;
using System.Collections.Generic;
using System.Linq;
using HighFlyers;

namespace UglyGCS.Protocol
{
    public class Proto
    {
        private byte[] value;

        public Proto()
        {
        }

        public Proto(byte[] data)
        {
            Command = (_FrameSrame)data[0];
            // endiana doesn't depend on a system, but on a protocol.
            // best practice is to add a bit to a frame, which describes
            // endiana. But we don't have a time for it.
            value = data.ToList().GetRange(1, 4).ToArray();
        }

        public _FrameSrame Command
        {
            get;
            set;
        }

        public ProtoBuffer ToBuffer()
        {
            var collector = new List<byte> {(byte) Command};
            collector.AddRange(value);

            return new ProtoBuffer(collector.ToArray());
        }

        public Int32 ValueInt32
        {
            get { return BitConverter.ToInt32(value, 0); }
            set { this.value = BitConverter.GetBytes(value); }
        }

        public float ValueFloat
        {
            get { return BitConverter.ToSingle(value, 0); }
            set { this.value = BitConverter.GetBytes(value); }
        }
    }

    public class ProtoEventArgs : EventArgs
    {
        public Proto Proto
        {
            get; private set;
        }

        public ProtoEventArgs(Proto proto)
        {
            Proto = proto;
        }
    }

    public delegate void ProtoEventHandler(object sender, ProtoEventArgs e);
}
