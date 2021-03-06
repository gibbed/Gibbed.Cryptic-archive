/* Copyright (c) 2012 Rick (rick 'at' gibbed 'dot' us)
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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Gibbed.Cryptic.FileFormats;
using ParserSchema = Gibbed.Cryptic.FileFormats.ParserSchema;

namespace Gibbed.Cryptic.GenerateSerializer
{
    public class ParserLoader
    {
        private readonly Dictionary<string, string> _Paths;
        private readonly Dictionary<string, ParserSchemaFile> _LoadedParsers;

        public IEnumerable<string> ParserNames
        {
            get { return this._Paths.Keys; }
        }

        public ParserLoader(string path)
        {
            this._Paths = new Dictionary<string, string>();
            this._LoadedParsers = new Dictionary<string, ParserSchemaFile>();

            foreach (var inputPath in Directory.GetFiles(path, "*.schema.xml", SearchOption.AllDirectories))
            {
                var name = ParserSchemaFile.GetNameFromFile(inputPath);
                if (name == "ReferenceEArrayTemplate")
                {
                    continue;
                }

                this._Paths.Add(name, inputPath);
            }
        }

        private void ResolveParser(ParserSchema.Table table)
        {
            foreach (var column in table.Columns
                                        .Where(c => c.Subtable != null))
            {
                ResolveParser(column.Subtable);
            }

            foreach (var column in table.Columns
                                        .Where(c => string.IsNullOrEmpty(c.SubtableExternalName) == false))
            {
                var external = this.LoadParser(column.SubtableExternalName);
                column.Subtable = external.Table;
            }
        }

        public ParserSchemaFile LoadParser(string name)
        {
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }

            if (this._Paths.ContainsKey(name) == false)
            {
                throw new ArgumentException("no such parser", "name");
            }

            if (this._LoadedParsers.ContainsKey(name) == true)
            {
                return this._LoadedParsers[name];
            }

            var parser = ParserSchemaFile.LoadFile(this._Paths[name]);
            this._LoadedParsers.Add(name, parser);

            ResolveParser(parser.Table);

            return parser;
        }
    }
}
