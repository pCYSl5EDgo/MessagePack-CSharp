// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

//using MessagePack.Formatters;
//using MessagePack.Resolvers;
//using Xunit;

//namespace MessagePack.Tests.CircularReference
//{
//    public class TwinTest
//    {
//        private MessagePackSerializerOptions options;

//        public TwinTest()
//        {
//            var resolver = CompositeResolver.Create(
//                new IMessagePackFormatter[]
//                {
//                    new CircularFormatter<TwinExample0>(StandardResolver.Instance.GetFormatterWithVerify<TwinExample0>()!, TwinExample0Overwriter.Instance),
//                    new CircularFormatter<TwinExample1>(StandardResolver.Instance.GetFormatterWithVerify<TwinExample1>()!, TwinExample1Overwriter.Instance),
//                },
//                new[] { StandardResolver.Instance });
//            options = MessagePackSerializerOptions.Standard.WithResolver(resolver);
//        }

//        [Fact]
//        public void NullTest0()
//        {
//            var original = default(TwinExample0);

//            var binary = MessagePackSerializer.Serialize(original, options);
//            var result = MessagePackSerializer.Deserialize<TwinExample0>(binary, options);

//            Assert.Null(result);
//        }

//        [Fact]
//        public void NullTest1()
//        {
//            var original = default(TwinExample1);

//            var binary = MessagePackSerializer.Serialize(original, options);
//            var result = MessagePackSerializer.Deserialize<TwinExample1>(binary, options);

//            Assert.Null(result);
//        }

//        [Theory]
//        [InlineData("")]
//        [InlineData("名無しの権兵衛")]
//        public void FieldNullTest0(string name)
//        {
//            var original = new TwinExample0() { Name = name };

//            var binary = MessagePackSerializer.Serialize(original, options);
//            var result = MessagePackSerializer.Deserialize<TwinExample0>(binary, options);

//            Assert.NotNull(result);
//            Assert.Null(result.Partner);
//            Assert.Equal(result.Name, name);
//        }

//        [Theory]
//        [InlineData("")]
//        [InlineData("名無しの権兵衛")]
//        public void FieldNullTest1(string name)
//        {
//            var original = new TwinExample1() { Name = name };

//            var binary = MessagePackSerializer.Serialize(original, options);
//            var result = MessagePackSerializer.Deserialize<TwinExample1>(binary, options);

//            Assert.NotNull(result);
//            Assert.Null(result.Partner);
//            Assert.Equal(result.Name, name);
//        }

//        [Theory]
//        [InlineData("", "empty")]
//        [InlineData("名無しの権兵衛", "じゅげむじゅげむごこうのすりきれ")]
//        public void MirrorTest(string name0, string name1)
//        {
//            var original0 = new TwinExample1() { Name = name0 };
//            var original1 = new TwinExample0()
//            {
//                Name = name1,
//                Partner = original0,
//            };
//            original0.Partner = original1;

//            var binary = MessagePackSerializer.Serialize(original0, options);
//            var result = MessagePackSerializer.Deserialize<TwinExample0>(binary, options);

//            Assert.NotNull(result);
//            Assert.NotNull(result.Partner);
//            Assert.Equal(result.Name, name0);
//            Assert.Equal(result.Partner!.Name, name1);
//            Assert.Same(result.Partner.Partner, result);
//        }
//    }
//}
