/* Copyright (c) 2015 Rick (rick 'at' gibbed 'dot' us)
 * 
 * This software is provided 'as-is', without any express or implied
 * warranty. In no event will the authors be held liable for any damages
 * arising from the use of this software.
 * 
 * Permission is granted to anyone to use this software for any purpose,
 * including commercial applications, and to alter it and redistribute it
 * freely, subject to the following restrictions:
 * 
 * 1. The origin of this software must not be misrepresented; you must not
 *    claim that you wrote the original software. If you use this software
 *    in a product, an acknowledgment in the product documentation would
 *    be appreciated but is not required.
 * 
 * 2. Altered source versions must be plainly marked as such, and must not
 *    be misrepresented as being the original software.
 * 
 * 3. This notice may not be removed or altered from any source
 *    distribution.
 */

using System;
using System.Globalization;
using System.Runtime.InteropServices;
using Parser = Gibbed.Cryptic.FileFormats.Parser;

namespace Gibbed.Cryptic.ExportSchemas.Natives
{
    [StructLayout(LayoutKind.Sequential)]
    public class ParserColumn
    {
        public uint NamePointer;
        public uint Unknown04;
        public ulong Type;
        public uint Offset;
        public uint Parameter0;
        public uint Parameter1;
        public uint Format;
        public uint FormatStringPointer;
        public uint Unknown24;

        public byte MinBits
        {
            get { return (byte)((this.Type >> 8) & 0xFF); }
        }

        public byte FloatRounding
        {
            get { return this.MinBits; }
        }

        public byte Token
        {
            get { return (byte)(this.Type & 0xFF); }

            set { this.Type = (this.Type & ~(UInt32)0xFF) | (byte)(value); }
        }

        public Parser.ColumnFlags Flags
        {
            get { return (Parser.ColumnFlags)(this.Type & ~0xFFFFUL); }

            set
            {
                this.Type &= 0xFFFFUL;
                this.Type |= ((ulong)value) & ~0xFFFFUL;
            }
        }

        public override string ToString()
        {
            return this.Type.ToString(CultureInfo.InvariantCulture);
        }
    }
}
