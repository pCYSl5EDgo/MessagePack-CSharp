// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

//#if CSHARP_8_OR_NEWER || NETCOREAPP3_1
//#nullable enable
//#endif

//namespace MessagePack.Tests.CircularReference
//{
//    [MessagePackObject]
//    public sealed class CircleExample
//    {
//#if CSHARP_8_OR_NEWER || NETCOREAPP3_1
//        [Key(0)]
//        public CircleExample? Parent { get; set; }

//        [Key(1)]
//        public CircleExample? Child0 { get; set; }

//        [Key(2)]
//        public CircleExample? Child1 { get; set; }
//#else
//        [Key(0)]
//        public CircleExample Parent { get; set; }

//        [Key(1)]
//        public CircleExample Child0 { get; set; }

//        [Key(2)]
//        public CircleExample Child1 { get; set; }
//#endif

//        [Key(3)]
//        public int Id { get; set; }
//    }

//    public sealed class CircleExampleOverwriter : IOverwriteMessagePackFormatter<CircleExample>
//    {
//        public static readonly CircleExampleOverwriter Instance = new CircleExampleOverwriter();

//        private CircleExampleOverwriter()
//        {
//        }

//        public void DeserializeTo(ref MessagePackReader reader, ref CircleExample value, MessagePackSerializerOptions options)
//        {
//            var length = reader.ReadArrayHeader();
//            if (length == 0)
//            {
//                return;
//            }

//            var selfFormatter = options.Resolver.GetFormatterWithVerify<CircleExample>();
//            for (var i = 0; i < length; i++)
//            {
//                switch (i)
//                {
//                    default:
//                        reader.Skip();
//                        break;
//                    case 0:
//                        value.Parent = selfFormatter.Deserialize(ref reader, options);
//                        break;
//                    case 1:
//                        value.Child0 = selfFormatter.Deserialize(ref reader, options);
//                        break;
//                    case 2:
//                        value.Child1 = selfFormatter.Deserialize(ref reader, options);
//                        break;
//                    case 3:
//                        value.Id = reader.ReadInt32();
//                        break;
//                }
//            }
//        }
//    }
//}
