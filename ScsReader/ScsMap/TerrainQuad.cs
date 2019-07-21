﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScsReader.ScsMap
{
    /// <summary>
    /// A single terrain quad.
    /// </summary>
    public class TerrainQuad : IBinarySerializable
    {
        /// <summary>
        /// Main terrain material of this quad.
        /// </summary>
        public byte MainMaterial;

        /// <summary>
        /// Additional material that will be drawn on top with the specified
        /// opacity value.
        /// </summary>
        public byte BlendMaterial;

        /// <summary>
        /// Opacity for the blend material.
        /// </summary>
        public byte Opacity;

        /// <summary>
        /// Texture color in the bottom left corner.
        /// </summary>
        public byte ColorBottomLeft;

        /// <summary>
        /// Texture color in the bottom right corner.
        /// </summary>
        public byte ColorBottomRight;

        /// <summary>
        /// Texture color in the top left corner.
        /// </summary>
        public byte ColorTopLeft;

        /// <summary>
        /// Texture color in the top right corner.
        /// </summary>
        public byte ColorTopRight;

        /// <summary>
        /// Vegetation setting for this quad.
        /// </summary>
        public QuadVegetation Vegetation;

        public TerrainQuad Clone()
        {
            return (TerrainQuad)MemberwiseClone();
        }

        public void ReadFromStream(BinaryReader r)
        {
            // nibble 1: main material
            // nibble 2: 2nd material to blend on top of main material
            var byte1 = r.ReadByte();
            MainMaterial = (byte)(byte1 & 0x0F);
            BlendMaterial = (byte)((byte1 & 0xF0) >> 4);

            // nibble 1: material blend opacity
            // nibble 2: bottom left color
            var byte2 = r.ReadByte();
            Opacity = (byte)(byte2 & 0x0F);
            ColorBottomLeft = (byte)((byte2 & 0xF0) >> 4);

            // nibble 1: bottom right color
            // nibble 2: top left color
            var byte3 = r.ReadByte();
            ColorBottomRight = (byte)(byte3 & 0x0F);
            ColorTopLeft = (byte)((byte3 & 0xF0) >> 4);

            // nibble 1: top right color
            // nibble 2: normal veg. / no detail veg. / no veg.
            var byte4 = r.ReadByte();
            ColorTopRight = (byte)(byte4 & 0x0F);
            Vegetation = (QuadVegetation)((byte4 & 0xF0) >> 4);
        }

        public void WriteToStream(BinaryWriter w)
        {
            byte byte1 = 0;
            byte1 |= MainMaterial;
            byte1 |= (byte)(BlendMaterial << 4);
            w.Write(byte1);

            byte byte2 = 0;
            byte2 |= Opacity;
            byte2 |= (byte)(ColorBottomLeft << 4);
            w.Write(byte2);

            byte byte3 = 0;
            byte3 |= ColorBottomRight;
            byte3 |= (byte)(ColorTopLeft << 4);
            w.Write(byte3);

            byte byte4 = 0;
            byte4 |= ColorTopRight;
            byte4 |= (byte)((byte)Vegetation << 4);
            w.Write(byte4);
        }
    }
}