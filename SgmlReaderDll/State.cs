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
    internal enum State
    {
        Initial,    // The initial state (Read has not been called yet)
        Markup,     // Expecting text or markup
        EndTag,     // Positioned on an end tag
        Attr,       // Positioned on an attribute
        AttrValue,  // Positioned in an attribute value
        Text,       // Positioned on a Text node.
        PartialTag, // Positioned on a text node, and we have hit a start tag
        AutoClose,  // We are auto-closing tags (this is like State.EndTag), but end tag was generated
        CData,      // We are on a CDATA type node, eg. <scipt> where we have special parsing rules.
        PartialText,
        PseudoStartTag, // we pushed a pseudo-start tag, need to continue with previous start tag.
        Eof
    }
}