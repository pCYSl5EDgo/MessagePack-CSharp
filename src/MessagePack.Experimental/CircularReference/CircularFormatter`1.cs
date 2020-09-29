// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace MessagePack.Formatters
{
    public sealed class CircularFormatter<T> : IMessagePackFormatter<T?>
        where T : class, new()
    {
#pragma warning disable SA1401 // Fields should be private
        public readonly IMessagePackFormatter<T?> Formatter;
        public readonly IMessagePackDeserializeOverwriter<T> Overwriter;
#pragma warning restore SA1401 // Fields should be private

        public CircularFormatter(IMessagePackFormatter<T?> formatter, IMessagePackDeserializeOverwriter<T> overwriter)
        {
            Formatter = formatter;
            Overwriter = overwriter;
        }

        public void Serialize(ref MessagePackWriter writer, T? value, MessagePackSerializerOptions options)
        {
            if (value is null)
            {
                writer.WriteNil();
                return;
            }

            var cache = writer.Cache;
            var index = cache.FindIndex(value);
            if (index >= 0)
            {
                writer.Write((uint)index);
                return;
            }

            cache.Add(value);
            Formatter.Serialize(ref writer, value, options);
        }

        public T? Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
        {
            if (reader.TryReadNil())
            {
                return default;
            }

            var cache = reader.Cache;
            if (TryReadUInt32(ref reader, out var index))
            {
                return (T)cache.Span[(int)index];
            }

            var answer = new T();
            cache.Add(answer);
            Overwriter.DeserializeOverwrite(ref reader, options, ref answer);
            return answer;
        }

        private static bool TryReadUInt32(ref MessagePackReader reader, out uint value)
        {
            if (reader.NextMessagePackType == MessagePackType.Integer)
            {
                value = reader.ReadUInt32();
                return true;
            }

            value = default;
            return false;
        }
    }
}
