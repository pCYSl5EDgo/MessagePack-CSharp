// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Buffers;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using Nerdbank.Streams;
using Xunit;
using Xunit.Abstractions;

namespace MessagePack.Tests
{
    public class MessagePackWriterTests
    {
        private readonly ITestOutputHelper logger;

#if !UNITY_2018_3_OR_NEWER

        public MessagePackWriterTests(ITestOutputHelper logger)
        {
            this.logger = logger;
        }

#else
        public MessagePackWriterTests()
        {
            this.logger = new NullTestOutputHelper();
        }

#endif

        /// <summary>
        /// Verifies that <see cref="MessagePackWriter.WriteRaw(ReadOnlySpan{byte})"/>
        /// accepts a span that came from stackalloc.
        /// </summary>
        [Fact]
        public unsafe void WriteRaw_StackAllocatedSpan()
        {
            var sequence = new Sequence<byte>();
            var writer = new MessagePackWriter(sequence);
            try
            {
                Span<byte> bytes = stackalloc byte[8];
                bytes[0] = 1;
                bytes[7] = 2;
                fixed (byte* pBytes = bytes)
                {
                    var flexSpan = new Span<byte>(pBytes, bytes.Length);
                    writer.WriteRaw(flexSpan);
                }

                writer.Flush();
            }
            finally
            {
                writer.Dispose();
            }

            var written = sequence.AsReadOnlySequence.ToArray();
            Assert.Equal(1, written[0]);
            Assert.Equal(2, written[7]);
        }

        [Fact]
        public void Write_ByteArray_null()
        {
            var sequence = new Sequence<byte>();
            var writer = new MessagePackWriter(sequence);
            try
            {
                writer.Write((byte[])null);
                writer.Flush();
            }
            finally
            {
                writer.Dispose();
            }

            var reader = new MessagePackReader(sequence.AsReadOnlySequence);
            try
            {
                Assert.True(reader.TryReadNil());
            }
            finally
            {
                reader.Dispose();
            }
        }

        [Fact]
        public void Write_ByteArray()
        {
            var sequence = new Sequence<byte>();
            var buffer = new byte[] { 1, 2, 3 };
            var writer = new MessagePackWriter(sequence);
            try
            {
                writer.Write(buffer);
                writer.Flush();
            }
            finally
            {
                writer.Dispose();
            }

            var reader = new MessagePackReader(sequence.AsReadOnlySequence);
            try
            {
                Assert.Equal(buffer, reader.ReadBytes().Value.ToArray());
            }
            finally
            {
                reader.Dispose();
            }
        }

        [Fact]
        public void Write_String_null()
        {
            var sequence = new Sequence<byte>();
            var writer = new MessagePackWriter(sequence);
            try
            {
                writer.Write((string)null);
                writer.Flush();
            }
            finally
            {
                writer.Dispose();
            }

            var reader = new MessagePackReader(sequence.AsReadOnlySequence);
            try
            {
                Assert.True(reader.TryReadNil());
            }
            finally
            {
                reader.Dispose();
            }
        }

        [Fact]
        public void Write_String()
        {
            var sequence = new Sequence<byte>();
            const string expected = "hello";
            var writer = new MessagePackWriter(sequence);
            try
            {
                writer.Write(expected);
                writer.Flush();
            }
            finally
            {
                writer.Dispose();
            }

            var reader = new MessagePackReader(sequence.AsReadOnlySequence);
            try
            {
                Assert.Equal(expected, reader.ReadString());
            }
            finally
            {
                reader.Dispose();
            }
        }

        [Fact]
        public void Write_String_MultibyteChars()
        {
            var sequence = new Sequence<byte>();
            var writer = new MessagePackWriter(sequence);
            try
            {
                writer.Write(TestConstants.MultibyteCharString);
                writer.Flush();
            }
            finally
            {
                writer.Dispose();
            }

            this.logger.WriteLine("Written bytes: [{0}]", string.Join(", ", sequence.AsReadOnlySequence.ToArray().Select(b => string.Format(CultureInfo.InvariantCulture, "0x{0:x2}", b))));
            Assert.Equal(TestConstants.MsgPackEncodedMultibyteCharString.ToArray(), sequence.AsReadOnlySequence.ToArray());
        }

        [Fact]
        public void WriteStringHeader()
        {
            var sequence = new Sequence<byte>();
            byte[] strBytes = Encoding.UTF8.GetBytes("hello");
            var writer = new MessagePackWriter(sequence);
            try
            {
                writer.WriteStringHeader(strBytes.Length);
                writer.WriteRaw(strBytes);
                writer.Flush();
            }
            finally
            {
                writer.Dispose();
            }

            var reader = new MessagePackReader(sequence);
            try
            {
                Assert.Equal("hello", reader.ReadString());
            }
            finally
            {
                reader.Dispose();
            }
        }

        [Fact]
        public void WriteBinHeader()
        {
            var sequence = new Sequence<byte>();
            var writer = new MessagePackWriter(sequence);
            try
            {
                writer.WriteBinHeader(5);
                writer.WriteRaw(new byte[] { 1, 2, 3, 4, 5 });
                writer.Flush();
            }
            finally
            {
                writer.Dispose();
            }

            var reader = new MessagePackReader(sequence);
            try
            {
                Assert.Equal(new byte[] { 1, 2, 3, 4, 5 }, reader.ReadBytes().Value.ToArray());
            }
            finally
            {
                reader.Dispose();
            }
        }

        [Fact]
        public void WriteExtensionFormatHeader_NegativeExtension()
        {
            var sequence = new Sequence<byte>();
            var header = new ExtensionHeader(-1, 10);
            var writer = new MessagePackWriter(sequence);
            try
            {
                writer.WriteExtensionFormatHeader(header);
                writer.WriteRaw(new byte[10]);
                writer.Flush();
            }
            finally
            {
                writer.Dispose();
            }

            var written = sequence.AsReadOnlySequence;
            var reader = new MessagePackReader(written);
            try
            {
                var readHeader = reader.ReadExtensionFormatHeader();

                Assert.Equal(header.TypeCode, readHeader.TypeCode);
                Assert.Equal(header.Length, readHeader.Length);
            }
            finally
            {
                reader.Dispose();
            }
        }

        [Fact]
        public void CancellationToken()
        {
            var sequence = new Sequence<byte>();
            var writer = new MessagePackWriter(sequence);
            try
            {
                Assert.False(writer.CancellationToken.CanBeCanceled);

                var cts = new CancellationTokenSource();
                writer.CancellationToken = cts.Token;
                Assert.Equal(cts.Token, writer.CancellationToken);
            }
            finally
            {
                writer.Dispose();
            }
        }

        [Fact]
        public void TryWriteWithBuggyWriter()
        {
            Assert.Throws<InvalidOperationException>(() =>
            {
                var writer = new MessagePackWriter(new BuggyBufferWriter());
                try
                {
                    writer.WriteRaw(new byte[10]);
                }
                finally
                {
                    writer.Dispose();
                }
            });
        }

        /// <summary>
        /// Besides being effectively a no-op, this <see cref="IBufferWriter{T}"/>
        /// is buggy because it can return empty arrays, which should never happen.
        /// A sizeHint=0 means give me whatever memory is available, but should never be empty.
        /// </summary>
        private class BuggyBufferWriter : IBufferWriter<byte>
        {
            public void Advance(int count)
            {
            }

            public Memory<byte> GetMemory(int sizeHint = 0) => new byte[sizeHint];

            public Span<byte> GetSpan(int sizeHint = 0) => new byte[sizeHint];
        }
    }
}
