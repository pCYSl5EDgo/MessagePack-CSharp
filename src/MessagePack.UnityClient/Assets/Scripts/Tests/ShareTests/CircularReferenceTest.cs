// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Xunit;

namespace MessagePack.Tests
{
    public class CircularReferenceTest
    {
        [Fact]
        public void NullTest()
        {
            var original = default(CircleExample);

            var binary = MessagePackSerializer.Serialize(original);
            var result = MessagePackSerializer.Deserialize<CircleExample>(binary);

            result.IsNull();
        }

        [Fact]
        public void NullTestTypeless()
        {
            var original = default(CircleExampleTypeless);

            var binary = MessagePackSerializer.Typeless.Serialize(original);
            var result = MessagePackSerializer.Typeless.Deserialize(binary);

            result.IsNull();
        }

        [Theory]
        [InlineData(0)]
        [InlineData(int.MaxValue)]
        [InlineData(int.MinValue)]
        public void FieldNullTest(int id)
        {
            var original = new CircleExample() { Id = id };

            var binary = MessagePackSerializer.Serialize(original);
            var result = MessagePackSerializer.Deserialize<CircleExample>(binary);

            result.IsNotNull();
            result.Parent.IsNull();
            result.Child0.IsNull();
            result.Child1.IsNull();
            result.Id.IsStructuralEqual(id);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(int.MaxValue)]
        [InlineData(int.MinValue)]
        public void FieldNullTestTypeless(int id)
        {
            var original = new CircleExampleTypeless() { Id = id };

            var binary = MessagePackSerializer.Typeless.Serialize(original);
            var result = (CircleExampleTypeless)MessagePackSerializer.Typeless.Deserialize(binary);

            result.IsNotNull();
            result.Parent.IsNull();
            result.Child0.IsNull();
            result.Child1.IsNull();
            result.Id.IsStructuralEqual(id);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(int.MaxValue)]
        [InlineData(int.MinValue)]
        public void SelfParentTest(int id)
        {
            var original = new CircleExample() { Id = id };
            original.Parent = original;

            var binary = MessagePackSerializer.Serialize(original);
            var result = MessagePackSerializer.Deserialize<CircleExample>(binary);

            result.IsNotNull();
            result.Parent.IsSameReferenceAs(result);
            result.Child0.IsNull();
            result.Child1.IsNull();
            result.Id.IsStructuralEqual(id);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(int.MaxValue)]
        [InlineData(int.MinValue)]
        public void SelfParentTestTypeless(int id)
        {
            var original = new CircleExampleTypeless() { Id = id };
            original.Parent = original;

            var binary = MessagePackSerializer.Typeless.Serialize(original);
            var result = (CircleExampleTypeless)MessagePackSerializer.Typeless.Deserialize(binary);

            result.IsNotNull();
            result.Parent.IsSameReferenceAs(result);
            result.Child0.IsNull();
            result.Child1.IsNull();
            result.Id.IsStructuralEqual(id);
        }

        [Theory]
        [InlineData(0, 1, 2)]
        [InlineData(int.MaxValue, int.MinValue, -1)]
        [InlineData(114514, 1919, -810931)]
        public void SimpleTreeTest(int id0, int id1, int id2)
        {
            var child0 = new CircleExample() { Id = id1 };
            var child1 = new CircleExample() { Id = id2 };
            var original = new CircleExample()
            {
                Id = id0,
                Child0 = child0,
                Child1 = child1,
            };
            child0.Parent = original;
            child1.Parent = original;

            var binary = MessagePackSerializer.Serialize(original);
            var result = MessagePackSerializer.Deserialize<CircleExample>(binary);

            Assert.NotNull(result);
            Assert.Null(result.Parent);
            Assert.NotNull(result.Child0);
            Assert.Same(result.Child0!.Parent, result);
            Assert.NotNull(result.Child1);
            Assert.Same(result.Child1!.Parent, result);
            Assert.Equal(result.Id, id0);
            Assert.Equal(result.Child0.Id, id1);
            Assert.Equal(result.Child1.Id, id2);
        }

        [Theory]
        [InlineData(0, 1, 2)]
        [InlineData(int.MaxValue, int.MinValue, -1)]
        [InlineData(114514, 1919, -810931)]
        public void SimpleTreeTestTypeless(int id0, int id1, int id2)
        {
            var child0 = new CircleExampleTypeless() { Id = id1 };
            var child1 = new CircleExampleTypeless() { Id = id2 };
            var original = new CircleExampleTypeless()
            {
                Id = id0,
                Child0 = child0,
                Child1 = child1,
            };
            child0.Parent = original;
            child1.Parent = original;

            var binary = MessagePackSerializer.Typeless.Serialize(original);
            var result = (CircleExampleTypeless)MessagePackSerializer.Typeless.Deserialize(binary);

            Assert.NotNull(result);
            Assert.Null(result.Parent);
            Assert.NotNull(result.Child0);
            Assert.Same(result.Child0!.Parent, result);
            Assert.NotNull(result.Child1);
            Assert.Same(result.Child1!.Parent, result);
            Assert.Equal(result.Id, id0);
            Assert.Equal(result.Child0.Id, id1);
            Assert.Equal(result.Child1.Id, id2);
        }

        private static int FindParentIndex(CircleExample[] array, int index)
        {
            var item = array[index].Parent;
            for (var i = 0; i < array.Length; i++)
            {
                if (array[i] == item)
                {
                    return i;
                }
            }

            return -1;
        }

        private static int FindParentIndex(CircleExampleTypeless[] array, int index)
        {
            var item = array[index].Parent;
            for (var i = 0; i < array.Length; i++)
            {
                if (array[i] == item)
                {
                    return i;
                }
            }

            return -1;
        }

        private static int FindChild0Index(CircleExample[] array, int index)
        {
            var item = array[index].Child0;
            for (var i = 0; i < array.Length; i++)
            {
                if (array[i] == item)
                {
                    return i;
                }
            }

            return -1;
        }

        private static int FindChild0Index(CircleExampleTypeless[] array, int index)
        {
            var item = array[index].Child0;
            for (var i = 0; i < array.Length; i++)
            {
                if (array[i] == item)
                {
                    return i;
                }
            }

            return -1;
        }

        private static int FindChild1Index(CircleExample[] array, int index)
        {
            var item = array[index].Child1;
            for (var i = 0; i < array.Length; i++)
            {
                if (array[i] == item)
                {
                    return i;
                }
            }

            return -1;
        }

        private static int FindChild1Index(CircleExampleTypeless[] array, int index)
        {
            var item = array[index].Child1;
            for (var i = 0; i < array.Length; i++)
            {
                if (array[i] == item)
                {
                    return i;
                }
            }

            return -1;
        }

        [Theory]
        [InlineData(new int[] { 0, 1 })]
        [InlineData(new int[] { 114, 514, -1919, 810931 })]
        [InlineData(new int[] { 0, int.MinValue, int.MaxValue, -1, 33, -4 })]
        public void ArrayTest(int[] array)
        {
            var original = new CircleExample[array.Length];
            for (var i = 0; i < original.Length; i++)
            {
                original[i] = new CircleExample
                {
                    Id = array[i],
                };
            }

            original[0].Parent = original[original.Length - 1];
            original[1].Parent = original[0];
            for (var i = 2; i < original.Length; i++)
            {
                original[i].Parent = original[i - 1];
                original[i].Child0 = original[i - 2];
            }

            original[original.Length - 1].Child1 = original[0];
            for (var i = 0; i < original.Length - 1; i++)
            {
                original[i].Child1 = original[i + 1];
            }

            var binary = MessagePackSerializer.Serialize(original);
            var result = MessagePackSerializer.Deserialize<CircleExample[]>(binary);

            Assert.NotNull(result);
            Assert.Equal(array.Length, result.Length);

            for (var i = 0; i < array.Length; i++)
            {
                Assert.Equal(FindParentIndex(original, i), FindParentIndex(result, i));
                Assert.Equal(FindChild0Index(original, i), FindChild0Index(result, i));
                Assert.Equal(FindChild1Index(original, i), FindChild1Index(result, i));
            }
        }

        [Theory]
        [InlineData(new int[] { 0, 1 })]
        [InlineData(new int[] { 114, 514, -1919, 810931 })]
        [InlineData(new int[] { 0, int.MinValue, int.MaxValue, -1, 33, -4 })]
        public void ArrayTestTypeless(int[] array)
        {
            var original = new CircleExampleTypeless[array.Length];
            for (var i = 0; i < original.Length; i++)
            {
                original[i] = new CircleExampleTypeless
                {
                    Id = array[i],
                };
            }

            original[0].Parent = original[original.Length - 1];
            original[1].Parent = original[0];
            for (var i = 2; i < original.Length; i++)
            {
                original[i].Parent = original[i - 1];
                original[i].Child0 = original[i - 2];
            }

            original[original.Length - 1].Child1 = original[0];
            for (var i = 0; i < original.Length - 1; i++)
            {
                original[i].Child1 = original[i + 1];
            }

            var binary = MessagePackSerializer.Typeless.Serialize(original);
            var result = (CircleExampleTypeless[])MessagePackSerializer.Typeless.Deserialize(binary);

            Assert.NotNull(result);
            Assert.Equal(array.Length, result.Length);

            for (var i = 0; i < array.Length; i++)
            {
                Assert.Equal(FindParentIndex(original, i), FindParentIndex(result, i));
                Assert.Equal(FindChild0Index(original, i), FindChild0Index(result, i));
                Assert.Equal(FindChild1Index(original, i), FindChild1Index(result, i));
            }
        }
    }

    public class TwinTest
    {
        [Fact]
        public void NullTest0()
        {
            var original = default(TwinExample0);

            var binary = MessagePackSerializer.Serialize(original);
            var result = MessagePackSerializer.Deserialize<TwinExample0>(binary);

            Assert.Null(result);
        }

        [Fact]
        public void NullTest0Typeless()
        {
            var original = default(TwinExample0Typeless);

            var binary = MessagePackSerializer.Typeless.Serialize(original);
            var result = MessagePackSerializer.Typeless.Deserialize(binary);

            Assert.Null(result);
        }

        [Fact]
        public void NullTest1()
        {
            var original = default(TwinExample1);

            var binary = MessagePackSerializer.Serialize(original);
            var result = MessagePackSerializer.Deserialize<TwinExample1>(binary);

            Assert.Null(result);
        }

        [Fact]
        public void NullTest1Typeless()
        {
            var original = default(TwinExample1Typeless);

            var binary = MessagePackSerializer.Typeless.Serialize(original);
            var result = MessagePackSerializer.Typeless.Deserialize(binary);

            Assert.Null(result);
        }

        [Theory]
        [InlineData("")]
        [InlineData("名無しの権兵衛")]
        public void FieldNullTest0(string name)
        {
            var original = new TwinExample0() { Name = name };

            var binary = MessagePackSerializer.Serialize(original);
            var result = MessagePackSerializer.Deserialize<TwinExample0>(binary);

            Assert.NotNull(result);
            Assert.Null(result.Partner);
            Assert.Equal(result.Name, name);
        }

        [Theory]
        [InlineData("")]
        [InlineData("名無しの権兵衛")]
        public void FieldNullTest0Typeless(string name)
        {
            var original = new TwinExample0Typeless() { Name = name };

            var binary = MessagePackSerializer.Typeless.Serialize(original);
            var result = (TwinExample0Typeless)MessagePackSerializer.Typeless.Deserialize(binary);

            Assert.NotNull(result);
            Assert.Null(result.Partner);
            Assert.Equal(result.Name, name);
        }

        [Theory]
        [InlineData("")]
        [InlineData("名無しの権兵衛")]
        public void FieldNullTest1(string name)
        {
            var original = new TwinExample1() { Name = name };

            var binary = MessagePackSerializer.Serialize(original);
            var result = MessagePackSerializer.Deserialize<TwinExample1>(binary);

            Assert.NotNull(result);
            Assert.Null(result.Partner);
            Assert.Equal(result.Name, name);
        }

        [Theory]
        [InlineData("")]
        [InlineData("名無しの権兵衛")]
        public void FieldNullTest1Typeless(string name)
        {
            var original = new TwinExample1Typeless() { Name = name };

            var binary = MessagePackSerializer.Typeless.Serialize(original);
            var result = (TwinExample1Typeless)MessagePackSerializer.Typeless.Deserialize(binary);

            Assert.NotNull(result);
            Assert.Null(result.Partner);
            Assert.Equal(result.Name, name);
        }

        [Theory]
        [InlineData("", "empty")]
        [InlineData("名無しの権兵衛", "じゅげむじゅげむごこうのすりきれ")]
        public void MirrorTest(string name0, string name1)
        {
            var original1 = new TwinExample1() { Name = name0 };
            var original0 = new TwinExample0()
            {
                Name = name1,
                Partner = original1,
            };
            original1.Partner = original0;

            var binary = MessagePackSerializer.Serialize(original1);
            var result = MessagePackSerializer.Deserialize<TwinExample1>(binary);

            Assert.NotNull(result);
            Assert.NotNull(result.Partner);
            Assert.Equal(result.Name, name0);
            Assert.Equal(result.Partner!.Name, name1);
            Assert.Same(result.Partner.Partner, result);
        }

        [Theory]
        [InlineData("", "empty")]
        [InlineData("名無しの権兵衛", "じゅげむじゅげむごこうのすりきれ")]
        public void MirrorTestTypeless(string name0, string name1)
        {
            var original1 = new TwinExample1Typeless() { Name = name0 };
            var original0 = new TwinExample0Typeless()
            {
                Name = name1,
                Partner = original1,
            };
            original1.Partner = original0;

            var binary = MessagePackSerializer.Typeless.Serialize(original1);
            var result = (TwinExample1Typeless)MessagePackSerializer.Typeless.Deserialize(binary);

            Assert.NotNull(result);
            Assert.NotNull(result.Partner);
            Assert.Equal(result.Name, name0);
            Assert.Equal(result.Partner!.Name, name1);
            Assert.Same(result.Partner.Partner, result);
        }
    }

    public class VersionTest
    {
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

            var binary = MessagePackSerializer.Serialize(original);
            var resultOld = MessagePackSerializer.Deserialize<CircleArrayOld>(binary);

            Assert.NotNull(resultOld);
            Assert.NotNull(resultOld.Array0);
            Assert.NotNull(resultOld.Array1);
            Assert.Equal(original.Array0.Length, resultOld.Array0!.Length);
            Assert.Equal(original.Array1.Length, resultOld.Array1!.Length);
            Assert.Same(resultOld.Array0[0].Parent, resultOld.Array1[0]);
            Assert.Same(resultOld.Array0[1].Parent, resultOld.Array1[1]);
            Assert.Same(resultOld.Array1[0].Parent, resultOld.Array0[0]);
            Assert.Same(resultOld.Array1[1].Parent, resultOld.Array0[1]);

            Assert.Throws<MessagePackSerializationException>(() => MessagePackSerializer.Deserialize<CircleArrayNew>(binary));
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

    [MessagePackObject, TrackReference]
    public sealed class CircleExample
    {
#if CSHARP_8_OR_NEWER || NETCOREAPP3_1
        [Key(0)]
        public CircleExample? Parent { get; set; }

        [Key(1)]
        public CircleExample? Child0 { get; set; }

        [Key(2)]
        public CircleExample? Child1 { get; set; }
#else
        [Key(0)]
        public CircleExample Parent { get; set; }

        [Key(1)]
        public CircleExample Child0 { get; set; }

        [Key(2)]
        public CircleExample Child1 { get; set; }
#endif

        [Key(3)]
        public int Id { get; set; }
    }

    public sealed class CircleExampleTypeless
    {
#if CSHARP_8_OR_NEWER || NETCOREAPP3_1
        public CircleExampleTypeless? Parent { get; set; }

        public CircleExampleTypeless? Child0 { get; set; }

        public CircleExampleTypeless? Child1 { get; set; }
#else
        public CircleExampleTypeless Parent { get; set; }

        public CircleExampleTypeless Child0 { get; set; }

        public CircleExampleTypeless Child1 { get; set; }
#endif

        [Key(3)]
        public int Id { get; set; }
    }

    [MessagePackObject, TrackReference]
    public class TwinExample0
    {
#if CSHARP_8_OR_NEWER || NETCOREAPP3_1
        [Key(0)]
        public string? Name { get; set; }

        [Key(1)]
        public TwinExample1? Partner { get; set; }
#else
        [Key(0)]
        public string Name { get; set; }

        [Key(1)]
        public TwinExample1 Partner { get; set; }
#endif
    }

    public class TwinExample0Typeless
    {
#if CSHARP_8_OR_NEWER || NETCOREAPP3_1
        public string? Name { get; set; }

        public TwinExample1Typeless? Partner { get; set; }
#else
        public string Name { get; set; }

        public TwinExample1Typeless Partner { get; set; }
#endif
    }

    [MessagePackObject, TrackReference]
    public class TwinExample1
    {
#if CSHARP_8_OR_NEWER || NETCOREAPP3_1
        [Key(0)]
        public string? Name { get; set; }

        [Key(1)]
        public TwinExample0? Partner { get; set; }
#else
        [Key(0)]
        public string Name { get; set; }

        [Key(1)]
        public TwinExample0 Partner { get; set; }
#endif
    }

    public class TwinExample1Typeless
    {
#if CSHARP_8_OR_NEWER || NETCOREAPP3_1
        public string? Name { get; set; }

        public TwinExample0Typeless? Partner { get; set; }
#else
        public string Name { get; set; }

        public TwinExample0Typeless Partner { get; set; }
#endif
    }
}
