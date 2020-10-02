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

namespace Sgml
{
    /// <summary>
    /// This stack maintains a high water mark for allocated objects so the client
    /// can reuse the objects in the stack to reduce memory allocations, this is
    /// used to maintain current state of the parser for element stack, and attributes
    /// in each element.
    /// </summary>
    internal sealed class HWStack<T>
        where T: class
    {
        private T[] m_items;
        private int m_size;
        private int m_count;
        private int m_growth;

        /// <summary>
        /// Initialises a new instance of the HWStack class.
        /// </summary>
        /// <param name="growth">The amount to grow the stack space by, if more space is needed on the stack.</param>
        public HWStack(int growth)
        {
            this.m_growth = growth;
        }

        /// <summary>
        /// The number of items currently in the stack.
        /// </summary>
        public int Count
        {
            get => this.m_count;
            set => this.m_count = value;
        }

        /// <summary>
        /// The size (capacity) of the stack.
        /// </summary>
        [SuppressMessage("Microsoft.Performance", "CA1811", Justification = "Kept for potential future usage.")]
        public int Size => this.m_size;

        /// <summary>
        /// Returns the item at the requested index or null if index is out of bounds
        /// </summary>
        /// <param name="i">The index of the item to retrieve.</param>
        /// <returns>The item at the requested index or null if index is out of bounds.</returns>
        public T this[int i]
        {
            get => (i >= 0 && i < this.m_size) ? m_items[i] : null;
            set => this.m_items[i] = value;
        }

        /// <summary>
        /// Removes and returns the item at the top of the stack
        /// </summary>
        /// <returns>The item at the top of the stack.</returns>
        public T Pop()
        {
            this.m_count--;
            if (this.m_count > 0)
            {
                return m_items[this.m_count - 1];
            }

            return default(T);
        }

        /// <summary>
        /// Pushes a new slot at the top of the stack.
        /// </summary>
        /// <returns>The object at the top of the stack.</returns>
        /// <remarks>
        /// This method tries to reuse a slot, if it returns null then
        /// the user has to call the other Push method.
        /// </remarks>
        public T Push()
        {
            if (this.m_count == this.m_size)
            {
                int newsize = this.m_size + this.m_growth;
                T[] newarray = new T[newsize];
                if (this.m_items != null)
                    Array.Copy(this.m_items, newarray, this.m_size);

                this.m_size = newsize;
                this.m_items = newarray;
            }
            return m_items[this.m_count++];
        }

        /// <summary>
        /// Remove a specific item from the stack.
        /// </summary>
        /// <param name="i">The index of the item to remove.</param>
        [SuppressMessage("Microsoft.Performance", "CA1811", Justification = "Kept for potential future usage.")]
        public void RemoveAt(int i)
        {
            this.m_items[i] = default(T);
            Array.Copy(this.m_items, i + 1, this.m_items, i, this.m_count - i - 1);
            this.m_count--;
        }
    }
}