using System;

namespace Erpk.XPath
{
    public class NodeNotFoundException : Exception
    {
        public NodeNotFoundException(string msg) : base(msg)
        {
        }
    }
}