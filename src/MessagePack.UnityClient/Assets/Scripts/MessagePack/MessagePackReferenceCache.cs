// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Buffers;
using System.Collections.Generic;

namespace MessagePack
{
#if MESSAGEPACK_INTERNAL
    internal
#else
    public
#endif
    sealed class MessagePackReferenceCache : IDisposable
    {
        private const uint MinimumLength = 256;

        private object[] array;
        private int count;

        public Span<object> Span => array.AsSpan(0, count);

        public int Capacity
        {
            get => array.Length;
            set
            {
                if (value <= array.Length)
                {
                    return;
                }

                var pool = ArrayPool<object>.Shared;
                var tempArray = pool.Rent(value);

                if (count != 0)
                {
                    Array.Copy(array, tempArray, count);
                }

                if (array.Length != 0)
                {
                    pool.Return(array);
                }

                array = tempArray;
            }
        }

        private MessagePackReferenceCache(uint minimumLength)
        {
            count = 0;
            array = ArrayPool<object>.Shared.Rent((int)minimumLength);
        }

        private static readonly Stack<MessagePackReferenceCache> NotUsed = new Stack<MessagePackReferenceCache>();
        private static readonly List<MessagePackReferenceCache> Used = new List<MessagePackReferenceCache>();

        public static MessagePackReferenceCache Rent(uint minimumLength = MinimumLength)
        {
            lock (NotUsed)
            {
                MessagePackReferenceCache answer;
                if (NotUsed.Count != 0)
                {
                    answer = NotUsed.Pop();
                    answer.Capacity = (int)minimumLength;
                }
                else
                {
                    answer = new MessagePackReferenceCache(minimumLength);
                }

                for (var i = 0; i < Used.Count; i++)
                {
                    if (Used[i] == null)
                    {
                        Used[i] = answer;
                        return answer;
                    }
                }

                Used.Add(answer);
                return answer;
            }
        }

        public static void Return(MessagePackReferenceCache cache)
        {
            cache.Dispose();
            lock (NotUsed)
            {
                for (var i = 0; i < Used.Count; i++)
                {
                    if (Used[i] != cache)
                    {
                        continue;
                    }

                    NotUsed.Push(cache);
                    Used[i] = default;
                    return;
                }

                throw new ArgumentException("List should contain argument!");
            }
        }

        public int Add(object reference)
        {
            if (count == array.Length)
            {
                var pool = ArrayPool<object>.Shared;
                if (count == 0)
                {
                    array = pool.Rent((int)MinimumLength);
                }
                else
                {
                    var tempArray = pool.Rent(count << 1);
                    Array.Copy(array, tempArray, array.Length);
                    pool.Return(array);
                    array = tempArray;
                }
            }

            array[count] = reference;
            return count++;
        }

        public int FindIndex(object reference)
        {
            var span = Span;
            for (var i = 0; i < span.Length; i++)
            {
                if (reference == span[i])
                {
                    return i;
                }
            }

            return -1;
        }

        public void Dispose()
        {
            count = 0;
            if (array.Length != 0)
            {
                Array.Clear(array, 0, array.Length);
                ArrayPool<object>.Shared.Return(array);
                array = Array.Empty<object>();
            }
        }

        public MessagePackReferenceCache Clone()
        {
            var answer = Rent((uint)count);
            if (count != 0)
            {
                answer.count = count;
                Array.Copy(array, answer.array, count);
            }

            return answer;
        }
    }
}
