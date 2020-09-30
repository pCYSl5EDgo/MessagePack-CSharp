// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using MessagePack.Formatters;
using MessagePack.Resolvers;
using Xunit;

namespace MessagePack.Tests.CircularReference
{
    public class VersionTest
    {
        private MessagePackSerializerOptions options;

        public VersionTest()
        {
            var resolver = CompositeResolver.Create(
                new IMessagePackFormatter[]
                {
                    new CircularFormatter<CircleExample>(StandardResolver.Instance.GetFormatterWithVerify<CircleExample>()!, CircleExampleOverwriter.Instance),
                },
                new[] { StandardResolver.Instance });
            options = MessagePackSerializerOptions.Standard.WithResolver(resolver);
        }

        [Fact]
        public void DifferentVersionTest()
        {
            var original = new CircleArrayOld
            {
                Array0 = new CircleExample[2]
                {
                    new CircleExample() { Id = 1 },
                    new CircleExample() { Id = 2 },
                },
                Array1 = new CircleExample[2]
                {
                    new CircleExample() { Id = -1 },
                    new CircleExample() { Id = -2 },
                },
            };

            original.Array0[0].Parent = original.Array1[0];
            original.Array0[1].Parent = original.Array1[1];
            original.Array1[0].Parent = original.Array0[0];
            original.Array1[1].Parent = original.Array0[1];

            var binary = MessagePackSerializer.Serialize(original, options);
            var resultOld = MessagePackSerializer.Deserialize<CircleArrayOld>(binary, options);

            Assert.NotNull(resultOld);
            Assert.NotNull(resultOld.Array0);
            Assert.NotNull(resultOld.Array1);
            Assert.Equal(original.Array0.Length, resultOld.Array0!.Length);
            Assert.Equal(original.Array1.Length, resultOld.Array1!.Length);
            Assert.Same(resultOld.Array0[0].Parent, resultOld.Array1[0]);
            Assert.Same(resultOld.Array0[1].Parent, resultOld.Array1[1]);
            Assert.Same(resultOld.Array1[0].Parent, resultOld.Array0[0]);
            Assert.Same(resultOld.Array1[1].Parent, resultOld.Array0[1]);

            Assert.Throws<MessagePackSerializationException>(() => MessagePackSerializer.Deserialize<CircleArrayNew>(binary, options));
        }
    }

    [MessagePackObject]
    public class CircleArrayOld
    {
#if CSHARP_8_OR_NEWER || NETCOREAPP3_1
        [Key(0)]
        public CircleExample[]? Array0 { get; set; }

        [Key(1)]
        public CircleExample[]? Array1 { get; set; }
#else
        [Key(0)]
        public CircleExample[] Array0 { get; set; }

        [Key(1)]
        public CircleExample[] Array1 { get; set; }
#endif
    }

    [MessagePackObject]
    public class CircleArrayNew
    {
#if CSHARP_8_OR_NEWER || NETCOREAPP3_1
        [IgnoreMember]
        public CircleExample[]? Array0 { get; set; }

        [Key(1)]
        public CircleExample[]? Array1 { get; set; }
#else
        [IgnoreMember]
        public CircleExample[] Array0 { get; set; }

        [Key(1)]
        public CircleExample[] Array1 { get; set; }
#endif
    }
}
