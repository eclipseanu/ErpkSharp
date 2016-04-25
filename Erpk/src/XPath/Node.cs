using System.Collections.Generic;
using System.Linq;
using HtmlAgilityPack;

namespace Erpk.XPath
{
    public class Node
    {
        private readonly HtmlNode _node;

        public Node(HtmlNode node)
        {
            _node = node;
        }

        public string OuterHtml => _node.OuterHtml;

        public Node Find(string query)
        {
            var rawNode = _node.SelectSingleNode(query);
            if (rawNode == null)
            {
                throw new NodeNotFoundException(query + " not found");
            }
            return new Node(rawNode);
        }

        public IEnumerable<Node> FindAll(string query)
        {
            return _node.SelectNodes(query)?.Select(raw => new Node(raw)) ?? Enumerable.Empty<Node>();
        }

        public bool FindAny(string query)
        {
            return _node.SelectSingleNode(query) != null;
        }

        public Node FindOneOrNull(string query)
        {
            var rawNode = _node.SelectSingleNode(query);
            return rawNode == null ? null : new Node(rawNode);
        }

        public string Extract()
        {
            return _node.InnerText;
        }

        public string GetAttribute(string attr)
        {
            return _node.GetAttributeValue(attr, "");
        }
    }
}