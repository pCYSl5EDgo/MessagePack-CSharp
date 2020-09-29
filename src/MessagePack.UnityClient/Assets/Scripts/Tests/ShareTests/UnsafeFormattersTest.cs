// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MessagePack.Formatters;
using Nerdbank.Streams;
using Xunit;

namespace MessagePack.Tests
{
    public class UnsafeFormattersTest
    {
        [Fact]
        public void GuidTest()
        {
            var guid = Guid.NewGuid();
            var sequence = new Sequence<byte>();
            var sequenceWriter = new MessagePackWriter(sequence);
            try
            {
                NativeGuidFormatter.Instance.Serialize(ref sequenceWriter, guid, null);
                sequenceWriter.Flush();
            }
            finally
            {
                sequenceWriter.Dispose();
            }

            sequence.Length.Is(18);

            Guid nguid;
            var sequenceReader = new MessagePackReader(sequence.AsReadOnlySequence);
            try
            {
                nguid = NativeGuidFormatter.Instance.Deserialize(ref sequenceReader, null);
                Assert.True(sequenceReader.End);
            }
            finally
            {
                sequenceReader.Dispose();
            }

            guid.Is(nguid);
        }

        [Fact]
        public void DecimalTest()
        {
            var d = new Decimal(1341, 53156, 61, true, 3);
            var sequence = new Sequence<byte>();
            var sequenceWriter = new MessagePackWriter(sequence);
            try
            {
                NativeDecimalFormatter.Instance.Serialize(ref sequenceWriter, d, null);
                sequenceWriter.Flush();
            }
            finally
            {
                sequenceWriter.Dispose();
            }

            sequence.Length.Is(18);

            decimal nd;
            var sequenceReader = new MessagePackReader(sequence.AsReadOnlySequence);
            try
            {
                nd = NativeDecimalFormatter.Instance.Deserialize(ref sequenceReader, null);
                Assert.True(sequenceReader.End);
            }
            finally
            {
                sequenceReader.Dispose();
            }

            d.Is(nd);
        }
    }
}
