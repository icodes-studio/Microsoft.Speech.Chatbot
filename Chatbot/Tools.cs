using System;
using System.IO;
using System.Xml;
using System.Net;
using System.Text;
using System.Globalization;
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

        public static XmlNode FindXmlNode(XmlNode root, string[] path, int depth = 0)
        {
            if (depth >= path.Length)
                return root;

            foreach (XmlNode node in root)
            {
                if (path[depth] == node.Name)
                    return FindXmlNode(node, path, depth + 1);
            }

            return null;
        }

        public static List<XmlNode> FindXmlNodes(XmlNode root, string[] path, int depth = 0)
        {
            if (depth == path.Length - 1)
            {
                var result = new List<XmlNode>();
                foreach (XmlNode node in root)
                {
                    if (path[depth] == node.Name)
                        result.Add(node);
                }
                return result;
            }
            else
            {
                foreach (XmlNode node in root)
                {
                    if (path[depth] == node.Name)
                        return FindXmlNodes(node, path, depth + 1);
                }
            }
            return null;
        }

        public static string FullPath(string path)
        {
            return AppDomain.CurrentDomain.BaseDirectory + path;
        }
    }
}
