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

#if !PORTABLE
#endif

namespace Sgml
{
    /// <summary>
    /// This class represents an attribute.  The AttDef is assigned
    /// from a validation process, and is used to provide default values.
    /// </summary>
    internal class Attribute
    {
        internal string Name;    // the atomized name.
        internal AttDef DtdType; // the AttDef of the attribute from the SGML DTD.
        internal char QuoteChar; // the quote character used for the attribute value.
        private string m_literalValue; // the attribute value

        /// <summary>
        /// Attribute objects are reused during parsing to reduce memory allocations, 
        /// hence the Reset method.
        /// </summary>
        public void Reset(string name, string value, char quote)
        {
            this.Name = name;
            this.m_literalValue = value;
            this.QuoteChar = quote;
            this.DtdType = null;
        }

        public string Value
        {
            get
            {
                if (this.m_literalValue != null) 
                    return this.m_literalValue;
                if (this.DtdType != null) 
                    return this.DtdType.Default;
                return null;
            }
/*            set
            {
                this.m_literalValue = value;
            }*/
        }

        public bool IsDefault => this.m_literalValue is null;
    }    
}