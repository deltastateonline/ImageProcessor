using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    [ProtoContract]
    public class ImageDefProto
    {
        [ProtoMember(1)]
        public required string Id { get; set; }

        [ProtoMember(2)]
        public required string Filename { get; set; }

        [ProtoMember(3)]
        public required string InputFolder { get; set; }

        [ProtoMember(4)]
        public required string OutputFolder { get; set; }

        [ProtoMember(5)]
        public decimal Resize { get; set; }
    }
}
