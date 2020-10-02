﻿/*
 * Original work Copyright (c) 2008 MindTouch. All rights reserved. 
 * Modified Work Copyright (c) 2016 Microsoft Corporation. All rights reserved.
 * 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * 
 *     http://www.apache.org/licenses/LICENSE-2.0
 */

using System;
using System.IO;
using System.Xml;
using log4net;
using NUnit.Framework;
using Sgml;

namespace SGMLTests {
    public partial class Tests {

        //--- Types ---
        private delegate void XmlReaderTestCallback(XmlReader reader, XmlWriter xmlWriter);

        private enum XmlRender {
            Doc,
            DocClone,
            Passthrough
        }

        //--- Class Fields ---
        private static ILog _log = LogManager.GetLogger(typeof(Tests));
        private static bool _debug = true;

        //--- Class Methods ---
        private static void Test(string name, XmlRender xmlRender, CaseFolding caseFolding, string doctype, bool format) {
            string source;
            string expected;
            ReadTest(name, out source, out expected);
            expected = expected.Trim().Replace("\r", "");
            string actual;

            // determine how the document should be written back
            XmlReaderTestCallback callback;
            switch(xmlRender) {
            case XmlRender.Doc:

                // test writing sgml reader using xml document load
                callback = (reader, writer) => {
                    var doc = new XmlDocument();
                    doc.Load(reader);
                    doc.WriteTo(writer);
                };
                break;
            case XmlRender.DocClone:

                // test writing sgml reader using xml document load and clone
                callback = (reader, writer) => {
                    var doc = new XmlDocument();
                    doc.Load(reader);
                    var clone = doc.Clone();
                    clone.WriteTo(writer);
                };
                break;
            case XmlRender.Passthrough:

                // test writing sgml reader directly to xml writer
                callback = (reader, writer) => {
                    reader.Read();
                    while(!reader.EOF) {
                        writer.WriteNode(reader, true);
                    }
                };
                break;
            default:
                throw new ArgumentException("unknown value", "xmlRender");
            }
            actual = RunTest(caseFolding, doctype, format, source, callback);
            Assert.AreEqual(expected, actual);
        }

        private static void ReadTest(string name, out string before, out string after) {
            var assembly = typeof(Tests).Assembly;
            if (assembly.FullName is null)
            {
                throw new NullReferenceException("Tests Assembly FullName is null (somehow?).");
            }
            var stream = assembly.GetManifestResourceStream(assembly.FullName.Split(',')[0] + ".Resources." + name);
            if(stream == null) {
                throw new FileNotFoundException("unable to load requested resource: " + name);
            }
            using(var sr = new StreamReader(stream)) {                
                var test = sr.ReadToEnd().Split('`');
                before = test[0];
                after = test[1];
            }
        }

        private static SgmlDtd LoadDtd(string docType, string name)
        {
            using (Stream stream = typeof(Tests).Assembly.GetManifestResourceStream("SgmlTests." + name))
            {
                SgmlDtd dtd = SgmlDtd.Parse(null, Path.GetFileNameWithoutExtension(name), new StreamReader(stream), "", new NameTable(), 
                    new DesktopEntityResolver());
                dtd.Name = docType;
                return dtd;
            }
        }

        private static string RunTest(CaseFolding caseFolding, string doctype, bool format, string source, XmlReaderTestCallback callback) {

            // initialize sgml reader
            XmlReader reader = new SgmlReader {
                CaseFolding = caseFolding,
                DocType = doctype,
                InputStream = new StringReader(source),
                WhitespaceHandling = format ? WhitespaceHandling.None : WhitespaceHandling.All
            };
            if (doctype == "OFX")
            {
                ((SgmlReader)reader).Dtd = LoadDtd("OFX", "ofx160.dtd");
            }


            // check if we need to use the LoggingXmlReader
            if (_debug) {
                reader = new LoggingXmlReader(reader, Console.Out);
            }

            // initialize xml writer
            var stringWriter = new StringWriter();
            var xmlTextWriter = new XmlTextWriter(stringWriter);
            if(format) {
                xmlTextWriter.Formatting = Formatting.Indented;
            }
            callback(reader, xmlTextWriter);
            xmlTextWriter.Close();

            // reproduce the parsed document
            string actual = stringWriter.ToString();

            // ensure that output can be parsed again
            try {
                using(var stringReader = new StringReader(actual)) {
                    var doc = new XmlDocument();
                    doc.Load(stringReader);
                }
            } catch(Exception) {
                Assert.Fail("unable to parse sgml reader output:\n{0}", actual);
            }
            return actual.Trim().Replace("\r", "");
        }
    }
}
