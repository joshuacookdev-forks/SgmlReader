/*
 * Copyright (c) 2020 Microsoft Corporation. All rights reserved.
 * Modified work Copyright (c) 2008 MindTouch. All rights reserved. 
 * s
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * 
 *     http://www.apache.org/licenses/LICENSE-2.0
 */

using System;
using System.Diagnostics.CodeAnalysis;
#if !PORTABLE
#endif
using System.Xml;

namespace Sgml
{
    /// <summary>
    /// This class models an XML node, an array of elements in scope is maintained while parsing
    /// for validation purposes, and these Node objects are reused to reduce object allocation,
    /// hence the reset method.  
    /// </summary>
    internal sealed class Node
    {
        internal XmlNodeType NodeType;
        internal string Value;
        internal XmlSpace Space;
        internal string XmlLang;
        internal bool IsEmpty;        
        internal string Name;
        internal ElementDecl DtdType; // the DTD type found via validation
        internal State CurrentState;
        internal bool Simulated; // tag was injected into result stream.
        readonly HWStack<Attribute> attributes = new HWStack<Attribute>(10);

        /// <summary>
        /// Attribute objects are reused during parsing to reduce memory allocations, 
        /// hence the Reset method. 
        /// </summary>
        public void Reset(string name, XmlNodeType nt, string value) {           
            this.Value = value;
            this.Name = name;
            this.NodeType = nt;
            this.Space = XmlSpace.None;
            this.XmlLang= null;
            this.IsEmpty = true;
            this.attributes.Count = 0;
            this.DtdType = null;
        }

        public Attribute AddAttribute(string name, string value, char quotechar, bool caseInsensitive) {
            Attribute a;
            // check for duplicates!
            for (int i = 0, n = this.attributes.Count; i < n; i++) {
                a = this.attributes[i];
                if (string.Equals(a.Name, name, caseInsensitive ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal))
                {
                    return null;
                }
            }
            // This code makes use of the high water mark for attribute objects,
            // and reuses exisint Attribute objects to avoid memory allocation.
            a = this.attributes.Push();
            if (a is null) {
                a = new Attribute();
                this.attributes[this.attributes.Count-1] = a;
            }
            a.Reset(name, value, quotechar);
            return a;
        }

        [SuppressMessage("Microsoft.Performance", "CA1811", Justification = "Kept for potential future usage.")]
        public void RemoveAttribute(string name)
        {
            for (int i = 0, n = this.attributes.Count; i < n; i++)
            {
                Attribute a  = this.attributes[i];
                if (string.Equals(a.Name, name, StringComparison.OrdinalIgnoreCase))
                {
                    this.attributes.RemoveAt(i);
                    return;
                }
            }
        }
        public void CopyAttributes(Node n) {
            for (int i = 0, len = n.attributes.Count; i < len; i++) {
                Attribute a = n.attributes[i];
                Attribute na = this.AddAttribute(a.Name, a.Value, a.QuoteChar, false);
                na.DtdType = a.DtdType;
            }
        }

        public int AttributeCount => this.attributes.Count;

        public int GetAttribute(string name) {
            for (int i = 0, n = this.attributes.Count; i < n; i++) {
                Attribute a = this.attributes[i];
                if (string.Equals(a.Name, name, StringComparison.OrdinalIgnoreCase)) {
                    return i;
                }
            }
            return -1;
        }

        public Attribute GetAttribute(int i) {
            if (i>=0 && i<this.attributes.Count) {
                Attribute a = this.attributes[i];
                return a;
            }
            return null;
        }
    }
}