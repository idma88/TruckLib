﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;
using System.IO;
using System.Collections;

namespace ScsReader.ScsMap
{
    /// <summary>
    /// A map node.
    /// </summary>
    public class Node : IItemReferences, IMapObject
    {
        /// <summary>
        /// The UID of this node.
        /// </summary>
        public ulong Uid { get; set; }

        /// <summary>
        /// The sectors this node is in.
        /// </summary>
        public List<Sector> Sectors = new List<Sector>();

        /// <summary>
        /// Position of the node. Note that this will be converted to fixed length floats.
        /// </summary>
        public Vector3 Position = new Vector3();

        /// <summary>
        /// Rotation of the node.
        /// </summary>
        public Quaternion Rotation = new Quaternion(0f, 0f, 0f, 1f);

        /// <summary>
        /// The backward item belonging to this node. 
        /// </summary>
        public IMapObject BackwardItem;

        /// <summary>
        /// The forward item belonging to this node. 
        /// </summary>
        public IMapObject ForwardItem;

        public BitArray Flags = new BitArray(32);

        /// <summary>
        /// Determines if this node is red or green.
        /// </summary>
        public bool IsRed
        {
            get => Flags[0];
            set => Flags[0] = value;
        }

        /// <summary>
        /// If true, the game will use whichever rotation is specified without
        /// reverting to its default rotation when the node is updated.
        /// </summary>
        public bool FreeRotation
        {
            get => Flags[2];
            set => Flags[2] = value;
        }

        /// <summary>
        /// Defines if this node is a country border.
        /// </summary>
        public bool IsCountryBorder
        {
            get => Flags[1];
            set => Flags[1] = value;
        }

        /// <summary>
        /// The country of the backward item if this node is a country border.
        /// </summary>
        public byte BackwardCountry
        {
            get => Flags.GetByte(2);
            set => Flags.SetByte(2, value);
        }

        /// <summary>
        /// The country of the forward item if this node is a country border.
        /// </summary>
        public byte ForwardCountry
        {
            get => Flags.GetByte(1);
            set => Flags.SetByte(1, value);
        }

        public Node()
        {
            Uid = Utils.GenerateUuid();
        }

        private const float positionFactor = 256f;

        /// <summary>
        /// Reads the node from a BinaryReader.
        /// </summary>
        /// <param name="sector"></param>
        /// <param name="r"></param>
        public void ReadFromStream(Sector sector, BinaryReader r)
        {            
            // Uid
            Uid = r.ReadUInt64();

            // Position
            Position.X = r.ReadInt32() / positionFactor;
            Position.Y = r.ReadInt32() / positionFactor;
            Position.Z = r.ReadInt32() / positionFactor;

            /* TODO: Figure out why I needed this code to begin with,
             * if I needed it at all
            // Sector
            // not all nodes in a sector file are actually in that sector,
            // so we'll check for each of them
            var sectorCoords = Map.GetSectorOfCoordinate(Position);
            if(!sector.Map.Sectors.ContainsKey(sectorCoords)) {
                sector.Map.AddSector(sectorCoords);
            }
            Sector = sector.Map.Sectors[sectorCoords];
            */
            Sectors.Add(sector);

            // rotation as quaternion
            Rotation = r.ReadQuaternion();

            // The item attached to the node in backward direction.
            // This reference will be resolved in Map.UpdateItemReferences()
            // once all sectors are loaded
            var bwItemUid = r.ReadUInt64();
            BackwardItem = bwItemUid == 0 ? null : new UnresolvedItem(bwItemUid);

            // The item attached to the node in forward direction.
            // This reference will be resolved in Map.UpdateItemReferences()
            // once all sectors are loaded
            var fwItemUid = r.ReadUInt64();
            ForwardItem = fwItemUid == 0 ? null : new UnresolvedItem(fwItemUid);

            Flags = new BitArray(r.ReadBytes(4));
        }

        /// <summary>
        /// Writes the node.
        /// </summary>
        /// <param name="w"></param>
        public void WriteToStream(BinaryWriter w)
        {
            // UID
            w.Write(Uid);

            // Position
            w.Write((int)(Position.X * positionFactor));
            w.Write((int)(Position.Y * positionFactor));
            w.Write((int)(Position.Z * positionFactor));

            // Rotation
            w.Write(Rotation.W);
            w.Write(Rotation.X);
            w.Write(Rotation.Y);
            w.Write(Rotation.Z);

            // Backward UID
            w.Write(BackwardItem is null ? 0UL : BackwardItem.Uid);

            // Forward UID
            w.Write(ForwardItem is null ? 0UL : ForwardItem.Uid);

            w.Write(Flags.ToUInt());
        }

        public override string ToString()
        {
            //return $"{Uid:X16} (B: {BackwardUid:X16}; F: {ForwardUid:X16})";
            return $"{Uid:X16} ({Position.X}|{Position.Y}|{Position.Z})";
        }

        /// <summary>
        /// Searches a list of all items for the items referenced by uid in this node
        /// and adds references to them in the node's MapItem fields.
        /// </summary>
        /// <param name="allItems">A list containing all map items.</param>
        public void UpdateItemReferences(Dictionary<ulong, MapItem> allItems)
        {
            if (ForwardItem is UnresolvedItem && allItems.ContainsKey(ForwardItem.Uid))
            {
                ForwardItem = allItems[ForwardItem.Uid];
            }
            if (BackwardItem is UnresolvedItem && allItems.ContainsKey(BackwardItem.Uid))
            {
                BackwardItem = allItems[BackwardItem.Uid];
            }
        }

    }
}