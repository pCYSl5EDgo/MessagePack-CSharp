// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace MessagePack
{
    /// <summary>
    /// The contract for overwrite-deserialization of some specific type.
    /// </summary>
    /// <typeparam name="T">The type to be overwrite-deserialized.</typeparam>
    public interface IOverwriteMessagePackFormatter<T>
#if CSHARP_8_OR_NEWER
#nullable enable
        where T : notnull
#nullable restore
#endif
    {
        void DeserializeTo(ref MessagePackReader reader, ref T value, MessagePackSerializerOptions options);
    }
}
