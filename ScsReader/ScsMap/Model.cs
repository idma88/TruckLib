﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace ScsReader.ScsMap
{
    /// <summary>
    /// A scenery model.
    /// </summary>
    public class Model : SingleNodeItem
    {
        public override ItemType ItemType => ItemType.Model;

        public override ItemFile DefaultItemFile => ItemFile.Aux;

        protected override ushort DefaultViewDistance => KdopItem.ViewDistanceClose;

        public new ushort ViewDistance
        {
            get => base.ViewDistance;
            set => base.ViewDistance = value;
        }

        /// <summary>
        /// The unit name of the model.
        /// </summary>
        public Token ModelName;

        /// <summary>
        /// The model look.
        /// </summary>
        public Token Look;

        /// <summary>
        /// The model variant.
        /// </summary>
        public Token Variant;

        private int colorVariantPos = 8;
        private int colorVariantLength = 4;
        /// <summary>
        /// The color variant.
        /// </summary>
        public byte ColorVariant
        {
            get => (byte)Flags.GetBitString(colorVariantPos, colorVariantLength);
            set
            {
                if (value > 15) throw new ArgumentOutOfRangeException("ColorVariant must be between 0 and 15.");
                Flags.SetBitString(value, colorVariantPos, colorVariantLength);
            }
        }

        /// <summary>
        /// The relative scale of the model.
        /// </summary>
        public Vector3 Scale = new Vector3(1f, 1f, 1f);

        /// <summary>
        /// Unit names of additional parts used by the model.
        /// </summary>
        public List<Token> AdditionalParts = new List<Token>();

        public Token TerrainMaterial;

        public Color TerrainColor = Color.FromArgb(0x00ffffff);

        private int staticLodPos = 4;
        private int staticLodLength = 2;
        public StaticLod StaticLod
        {
            get => (StaticLod)Flags.GetBitString(staticLodPos, staticLodLength);
            set => Flags.SetBitString((uint)value, staticLodPos, staticLodLength);
        }

        public bool ModelHookups
        {
            get => !Flags[3];
            set => Flags[3] = !value;
        }

        /// <summary>
        /// Determines if this item will render behind a cut plane.
        /// </summary>
        public bool IgnoreCutPlanes
        {
            get => Flags[2];
            set => Flags[2] = value;
        }

        /// <summary>
        /// Determines if the item is reflected in water.
        /// </summary>
        public bool WaterReflection
        {
            get => Flags[1];
            set => Flags[1] = value;
        }

        /// <summary>
        /// Determines if the item uses left hand traffic.
        /// </summary>
        public bool LeftHandTraffic
        { 
            get => Flags[0];
            set => Flags[0] = value;
        }

        /// <summary>
        /// Determines if detail vegetation (small clumps of grass etc.) is drawn.
        /// </summary>
        public bool DetailVegetation
        {
            get => Flags[12];
            set => Flags[12] = value;
        }

        /// <summary>
        /// Determines if the player can collide with this item.
        /// </summary>
        public bool Collision
        {
            get => !Flags[13];
            set => Flags[13] = !value;
        }

        /// <summary>
        /// Determines if this item casts shadows.
        /// </summary>
        public bool Shadows
        {
            get => !Flags[14];
            set => Flags[14] = !value;
        }

        /// <summary>
        /// Determines if this item is visible in mirrors.
        /// </summary>
        public bool MirrorReflection
        {
            get => !Flags[15];
            set => Flags[15] = !value;
        }

        /// <summary>
        /// Adds a new model to the map. This method will also create a node for the model.
        /// </summary>
        /// <param name="map">The map the model will be added to.</param>
        /// <param name="name">The name of the model.</param>
        /// <param name="variant">The variant of the model.</param>
        /// <param name="look">The look of the model.</param>
        /// <param name="position">The position of the model.</param>
        /// <returns>The newly created model.</returns>
        public static Model Add(IItemContainer map, Vector3 position, Quaternion rotation,
            Token name, Token variant, Token look)
        {
            var model = Add<Model>(map, position);

            model.Node.Rotation = rotation;
            model.ModelName = name;
            model.Look = look;
            model.Variant = variant;

            return model;
        }
        public static Model Add(IItemContainer map, Vector3 position, float yRotation,
            Token name, Token variant, Token look)
        {
            var rotation = Quaternion.CreateFromYawPitchRoll(yRotation, 0, 0);
            return Add(map, position, rotation, name, variant, look);
        }

        public override void ReadFromStream(BinaryReader r)
        {
            base.ReadFromStream(r);

            ModelName = r.ReadToken();
            Look = r.ReadToken();
            Variant = r.ReadToken();

            AdditionalParts = ReadObjectList<Token>(r);

            Node = new UnresolvedNode(r.ReadUInt64());

            Scale = r.ReadVector3();

            TerrainMaterial = r.ReadToken();
            TerrainColor = r.ReadColor();
        }

        public override void WriteToStream(BinaryWriter w)
        {
            base.WriteToStream(w);

            w.Write(ModelName);
            w.Write(Look);
            w.Write(Variant);

            WriteObjectList(w, AdditionalParts);

            w.Write(Node.Uid);

            w.Write(Scale);

            w.Write(TerrainMaterial);
            w.Write(TerrainColor);
        }

    }
}
