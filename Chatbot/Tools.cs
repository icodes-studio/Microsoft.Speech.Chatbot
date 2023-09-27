using System;
using System.Xml;
using System.Net;
using System.Text;
using System.Collections.Generic;

namespace Chatbot
{
    public static class Tools
    {
        public static XmlDocument LoadXml(string uri)
        {
            using (var www = new WebClient())
            {
                var response = www.DownloadData(uri);
                var xml = Encoding.UTF8.GetString(response);
                var doc = new XmlDocument();
                doc.LoadXml(xml);
                return doc;
            }
        }

        private static XmlNode FindXmlNode(XmlNode root, string[] path, int depth)
        {
            if (depth >= path.Length)
                return root;

            foreach (XmlNode node in root)
            {
                if (string.Equals(path[depth], node.Name, StringComparison.OrdinalIgnoreCase))
                    return FindXmlNode(node, path, depth + 1);
            }

            return null;
        }

        private static List<XmlNode> FindXmlNodes(XmlNode root, string[] path, int depth)
        {
            if (depth == path.Length - 1)
            {
                var result = new List<XmlNode>();
                foreach (XmlNode node in root)
                {
                    if (string.Equals(path[depth], node.Name, StringComparison.OrdinalIgnoreCase))
                        result.Add(node);
                }
                return result;
            }
            else
            {
                foreach (XmlNode node in root)
                {
                    if (string.Equals(path[depth], node.Name, StringComparison.OrdinalIgnoreCase))
                        return FindXmlNodes(node, path, depth + 1);
                }
            }
            return null;
        }

        public static XmlNode FindXmlNode(XmlNode root, string path) =>
            FindXmlNode(root, path.Split('/'), 0);

        public static List<XmlNode> FindXmlNodes(XmlNode root, string path) =>
            FindXmlNodes(root, path.Split('/'), 0);

        public static string FullPath(string path) =>
            AppDomain.CurrentDomain.BaseDirectory + path;
    }
}
