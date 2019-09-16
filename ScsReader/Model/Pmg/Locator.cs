﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using System.Text;

namespace ScsReader.Model.Pmg
{
    public class Locator : IBinarySerializable
    {
        public Token Name { get; set; }

        public Vector3 Position { get; set; }

        public float Scale { get; set; }

        public Quaternion Rotation { get; set; }

        public int HookupOffset { get; set; }

        public void ReadFromStream(BinaryReader r)
        {
            Name = r.ReadToken();
            Position = r.ReadVector3();
            Scale = r.ReadSingle();
            Rotation = r.ReadQuaternion();
            HookupOffset = r.ReadInt32();
        }

        public void WriteToStream(BinaryWriter w)
        {
            w.Write(Name);
            w.Write(Position);
            w.Write(Scale);
            w.Write(Rotation);
            w.Write(HookupOffset);
        }
    }
}
