// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

//using MessagePack.Formatters;
//using MessagePack.Resolvers;
//using Xunit;

//namespace MessagePack.Tests.CircularReference
//{
//    public class CircularReferenceTest
//    {
//        private MessagePackSerializerOptions options;

//        public CircularReferenceTest()
//        {
//            var resolver = CompositeResolver.Create(
//                new IMessagePackFormatter[]
//                {
//                    new CircularFormatter<CircleExample>(StandardResolver.Instance.GetFormatterWithVerify<CircleExample>()!, CircleExampleOverwriter.Instance),
//                },
//                new[] { StandardResolver.Instance });
//            options = MessagePackSerializerOptions.Standard.WithResolver(resolver);
//        }

//        [Fact]
//        public void NullTest()
//        {
//            var original = default(CircleExample);

//            var binary = MessagePackSerializer.Serialize(original, options);
//            var result = MessagePackSerializer.Deserialize<CircleExample>(binary, options);

//            result.IsNull();
//        }

//        [Theory]
//        [InlineData(0)]
//        [InlineData(int.MaxValue)]
//        [InlineData(int.MinValue)]
//        public void FieldNullTest(int id)
//        {
//            var original = new CircleExample() { Id = id };

//            var binary = MessagePackSerializer.Serialize(original, options);
//            var result = MessagePackSerializer.Deserialize<CircleExample>(binary, options);

//            result.IsNotNull();
//            result.Parent.IsNull();
//            result.Child0.IsNull();
//            result.Child1.IsNull();
//            result.Id.IsStructuralEqual(id);
//        }

//        [Theory]
//        [InlineData(0)]
//        [InlineData(int.MaxValue)]
//        [InlineData(int.MinValue)]
//        public void SelfParentTest(int id)
//        {
//            var original = new CircleExample() { Id = id };
//            original.Parent = original;

//            var binary = MessagePackSerializer.Serialize(original, options);
//            var result = MessagePackSerializer.Deserialize<CircleExample>(binary, options);

//            result.IsNotNull();
//            result.Parent.IsSameReferenceAs(result);
//            result.Child0.IsNull();
//            result.Child1.IsNull();
//            result.Id.IsStructuralEqual(id);
//        }

//        [Theory]
//        [InlineData(0, 1, 2)]
//        [InlineData(int.MaxValue, int.MinValue, -1)]
//        [InlineData(114514, 1919, -810931)]
//        public void SimpleTreeTest(int id0, int id1, int id2)
//        {
//            var child0 = new CircleExample() { Id = id1 };
//            var child1 = new CircleExample() { Id = id2 };
//            var original = new CircleExample()
//            {
//                Id = id0,
//                Child0 = child0,
//                Child1 = child1,
//            };
//            child0.Parent = original;
//            child1.Parent = original;

//            var binary = MessagePackSerializer.Serialize(original, options);
//            var result = MessagePackSerializer.Deserialize<CircleExample>(binary, options);

//            Assert.NotNull(result);
//            Assert.Null(result.Parent);
//            Assert.NotNull(result.Child0);
//            Assert.Same(result.Child0!.Parent, result);
//            Assert.NotNull(result.Child1);
//            Assert.Same(result.Child1!.Parent, result);
//            Assert.Equal(result.Id, id0);
//            Assert.Equal(result.Child0.Id, id1);
//            Assert.Equal(result.Child1.Id, id2);
//        }

//        [Theory]
//        [InlineData(new int[] { 0, 1 })]
//        [InlineData(new int[] { 114, 514, -1919, 810931 })]
//        [InlineData(new int[] { 0, int.MinValue, int.MaxValue, -1, 33, -4 })]
//        public void ArrayTest(int[] array)
//        {
//            var original = new CircleExample[array.Length];
//            for (var i = 0; i < original.Length; i++)
//            {
//                original[i] = new CircleExample
//                {
//                    Id = array[i],
//                };
//            }

//            original[0].Parent = original[original.Length - 1];
//            original[1].Parent = original[0];
//            for (var i = 2; i < original.Length; i++)
//            {
//                original[i].Parent = original[i - 1];
//                original[i].Child0 = original[i - 2];
//            }

//            original[original.Length - 1].Child1 = original[0];
//            for (var i = 0; i < original.Length - 1; i++)
//            {
//                original[i].Child1 = original[i + 1];
//            }

//            static int FindParentIndex(CircleExample[] array, int index)
//            {
//                var item = array[index].Parent;
//                for (var i = 0; i < array.Length; i++)
//                {
//                    if (array[i] == item)
//                    {
//                        return i;
//                    }
//                }

//                return -1;
//            }

//            static int FindChild0Index(CircleExample[] array, int index)
//            {
//                var item = array[index].Child0;
//                for (var i = 0; i < array.Length; i++)
//                {
//                    if (array[i] == item)
//                    {
//                        return i;
//                    }
//                }

//                return -1;
//            }

//            static int FindChild1Index(CircleExample[] array, int index)
//            {
//                var item = array[index].Child1;
//                for (var i = 0; i < array.Length; i++)
//                {
//                    if (array[i] == item)
//                    {
//                        return i;
//                    }
//                }

//                return -1;
//            }

//            var binary = MessagePackSerializer.Serialize(original, options);
//            var result = MessagePackSerializer.Deserialize<CircleExample[]>(binary, options);

//            Assert.NotNull(result);
//            Assert.Equal(array.Length, result.Length);

//            for (var i = 0; i < array.Length; i++)
//            {
//                Assert.Equal(FindParentIndex(original, i), FindParentIndex(result, i));
//                Assert.Equal(FindChild0Index(original, i), FindChild0Index(result, i));
//                Assert.Equal(FindChild1Index(original, i), FindChild1Index(result, i));
//            }
//        }
//    }
//}
