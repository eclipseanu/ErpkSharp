using HtmlAgilityPack;

namespace Erpk.XPath
{
    public static class NodeBuilder
    {
        public static Node FromHtml(string html)
        {
            // https://stackoverflow.com/questions/13977243/how-can-i-parse-innertext-of-option-tag-with-htmlagilitypack
            HtmlNode.ElementsFlags.Remove("option");
            var doc = new HtmlDocument();
            doc.LoadHtml(html);
            return new Node(doc.DocumentNode);
        }
    }
}