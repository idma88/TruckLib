﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Numerics;
using System.Text;

namespace TruckLib.Model
{
    public class Vertex
    {
        public Vector3 Position { get; set; }

        public Vector3 Normal { get; set; }

        public Vector4? Tangent { get; set; }

        public Color Color { get; set; }

        public Color? SecondaryColor { get; set; }

        public List<Vector2> TextureCoordinates { get; set; }

        public byte[] BoneIndexes { get; set; }

        public byte[] BoneWeights { get; set; }

        public Vertex(Vector3 position)
        {
            Position = position;
        }

        public Vertex(Vector3 position, Vector3 normal)
        {
            Position = position;
            Normal = normal;
        }
    }
}
