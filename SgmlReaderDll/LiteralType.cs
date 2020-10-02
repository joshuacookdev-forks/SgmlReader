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

using System.Diagnostics.CodeAnalysis;
#if WINDOWS_DESKTOP
using System.Runtime.Serialization;
using System.Security.Permissions;
#endif

namespace Sgml
{
    /// <summary>
    /// The different types of literal text returned by the SgmlParser.
    /// </summary>
    public enum LiteralType
    {
        /// <summary>
        /// CDATA text literals.
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1705", Justification = "This capitalisation is appropriate since the value it represents has all upper-case capitalisation.")]
        CDATA,

        /// <summary>
        /// SDATA entities.
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1705", Justification = "This capitalisation is appropriate since the value it represents has all upper-case capitalisation.")]
        SDATA,

        /// <summary>
        /// The contents of a Processing Instruction.
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1705", Justification = "This capitalisation is appropriate since the value it represents has all upper-case capitalisation.")]
        PI
    };
}
