using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography.Xml;
using System.Security.Cryptography;
using System.Linq;
using System.Xml.Linq;

namespace XMLComparisonTests
{
    [TestClass]
    public class UnitTests
    {
        [TestMethod]
        public void XMLSemanticCorrectnessTest()
        {
            var hash = new List<byte[]>();
            var inputs = new string[]
            {
                "<test><sample value=\"1\" putthisanywhere=\"abcdef\" /></test>",
                "<test><sample putthisanywhere=\"abcdef\" value=\"1\" /></test>",
                "<test><sample value=\"1\" broken=\"true\" /></test>"
            };

            foreach (var input in inputs)
            {
                using (var stream = new MemoryStream())
                {
                    using (var writer = new StreamWriter(stream))
                    {
                        writer.Write(input);
                        writer.Flush();

                        stream.Position = 0;

                        var xfrm = new XmlDsigC14NTransform(false);
                        xfrm.LoadInput(stream);

                        hash.Add(xfrm.GetDigestedOutput(new SHA1Managed()));
                    }
                }
            }

            CollectionAssert.AreEqual(hash[0], hash[1]);
            CollectionAssert.AreNotEqual(hash[0], hash[2]);
            CollectionAssert.AreNotEqual(hash[1], hash[2]);
        }

        [TestMethod]
        public void XMLInlineTest()
        {
            var doc = new XDocument();

            var root = new XElement("root", new XElement("test", "lol"));

            if (root.Elements("test").FirstOrDefault() != null)
                root.Element("test").SetValue("lol-updated");
            else
                root.Add(new XElement("test", "lol"));

            Assert.IsTrue(true);
        }
    }
}
