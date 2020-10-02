/*
 * Copyright (c) 2020 Microsoft Corporation. All rights reserved.
 * Modified work Copyright (c) 2008 MindTouch. All rights reserved. 
 * 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * 
 *     http://www.apache.org/licenses/LICENSE-2.0
 */

using System;
using System.Collections.Generic;
#if WINDOWS_DESKTOP
using System.Runtime.Serialization;
using System.Security.Permissions;
#endif

namespace Sgml
{
    /// <summary>
    /// An element declaration in a DTD.
    /// </summary>
    public class ElementDecl
    {
        private readonly string m_name;
        private readonly bool m_startTagOptional;
        private readonly bool m_endTagOptional;
        private readonly ContentModel m_contentModel;
        private readonly string[] m_inclusions;
        private readonly string[] m_exclusions;
        private Dictionary<string, AttDef> m_attList;

        /// <summary>
        /// Initialises a new element declaration instance.
        /// </summary>
        /// <param name="name">The name of the element.</param>
        /// <param name="sto">Whether the start tag is optional.</param>
        /// <param name="eto">Whether the end tag is optional.</param>
        /// <param name="cm">The <see cref="ContentModel"/> of the element.</param>
        /// <param name="inclusions"></param>
        /// <param name="exclusions"></param>
        public ElementDecl(string name, bool sto, bool eto, ContentModel cm, string[] inclusions, string[] exclusions)
        {
            m_name = name;
            m_startTagOptional = sto;
            m_endTagOptional = eto;
            m_contentModel = cm;
            m_inclusions = inclusions;
            m_exclusions = exclusions;
        }

        /// <summary>
        /// The element name.
        /// </summary>
        public string Name => m_name;

        /// <summary>
        /// The <see cref="Sgml.ContentModel"/> of the element declaration.
        /// </summary>
        public ContentModel ContentModel => m_contentModel;

        /// <summary>
        /// Whether the end tag of the element is optional.
        /// </summary>
        /// <value>true if the end tag of the element is optional, otherwise false.</value>
        public bool EndTagOptional => m_endTagOptional;

        /// <summary>
        /// Whether the start tag of the element is optional.
        /// </summary>
        /// <value>true if the start tag of the element is optional, otherwise false.</value>
        public bool StartTagOptional => m_startTagOptional;

        /// <summary>
        /// Finds the attribute definition with the specified name.
        /// </summary>
        /// <param name="name">The name of the <see cref="AttDef"/> to find.</param>
        /// <returns>The <see cref="AttDef"/> with the specified name.</returns>
        /// <exception cref="InvalidOperationException">If the attribute list has not yet been initialised.</exception>
        public AttDef FindAttribute(string name)
        {
            if (m_attList is null)
                throw new InvalidOperationException("The attribute list for the element declaration has not been initialised.");

            m_attList.TryGetValue(name.ToUpperInvariant(), out AttDef a);
            return a;
        }

        /// <summary>
        /// Adds attribute definitions to the element declaration.
        /// </summary>
        /// <param name="list">The list of attribute definitions to add.</param>
        public void AddAttDefs(Dictionary<string, AttDef> list)
        {
            if (list is null)
                throw new ArgumentNullException(nameof(list));

            if (m_attList is null) 
            {
                m_attList = list;
            } 
            else 
            {
                foreach (AttDef a in list.Values) 
                {
                    if (!m_attList.ContainsKey(a.Name)) 
                    {
                        m_attList.Add(a.Name, a);
                    }
                }
            }
        }

        /// <summary>
        /// Tests whether this element can contain another specified element.
        /// </summary>
        /// <param name="name">The name of the element to check for.</param>
        /// <param name="dtd">The DTD to use to do the check.</param>
        /// <returns>True if the specified element can be contained by this element.</returns>
        public bool CanContain(string name, SgmlDtd dtd)
        {            
            // return true if this element is allowed to contain the given element.
            if (m_exclusions != null) 
            {
                foreach (string s in m_exclusions) 
                {
                    if (string.Equals(s, name, StringComparison.OrdinalIgnoreCase))
                        return false;
                }
            }

            if (m_inclusions != null) 
            {
                foreach (string s in m_inclusions) 
                {
                    if (string.Equals(s, name, StringComparison.OrdinalIgnoreCase))
                        return true;
                }
            }
            return m_contentModel.CanContain(name, dtd);
        }
    }
}
