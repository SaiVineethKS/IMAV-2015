using System.Collections.Generic;

namespace HighFlyers.Various
{
    class GraphDataFrame
    {
        private readonly List<double> pppList = new List<double>();
        private readonly _FrameSrame frame;
        public System.Drawing.Color Color { get; set; }

        public GraphDataFrame(_FrameSrame frame)
        {
            this.frame = frame;
        }

        public _FrameSrame Frame
        {
            get { return frame; }
        }

        public void AddPoint(double v)
        {
            pppList.Add(v);
        }

        public int Iterator { get { return pppList.Count; } }

        public List<double> GetList()
        {
            return pppList;
        }
    }
}
