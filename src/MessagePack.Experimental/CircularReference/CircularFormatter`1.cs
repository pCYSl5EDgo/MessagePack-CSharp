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

            writer.WriteMapHeader(1);
            var cache = writer.Cache;
            var index = cache.FindIndex(value);
            if (index >= 0)
            {
                writer.WriteNil();
                writer.Write((uint)index);
                return;
            }

            index = cache.Add(value);
            writer.Write((uint)index);
            Formatter.Serialize(ref writer, value, options);
        }

        public T? Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
        {
            if (reader.TryReadNil())
            {
                return default;
            }

            var count = reader.ReadMapHeader();
            if (count != 1)
            {
                throw new MessagePackSerializationException($"type {typeof(T).FullName} should be encoded as length 1 map. actual: {count}");
            }

            var cache = reader.Cache;
            if (reader.TryReadNil())
            {
                var index = reader.ReadUInt32();
                return (T)cache.Span[(int)index];
            }
            else
            {
                var index = (int)reader.ReadUInt32();
                var answer = new T();
                var addedIndex = cache.Add(answer);
                if (index != addedIndex)
                {
                    throw new MessagePackSerializationException($"Object reference cache index mismatch! expected: {index}, actual: {addedIndex}");
                }

                Overwriter.DeserializeOverwrite(ref reader, options, ref answer);
                return answer;
            }
        }
    }
}
