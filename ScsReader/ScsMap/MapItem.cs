﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScsReader.ScsMap
{
    /// <summary>
    /// Base class for all map items.
    /// </summary>
    public abstract class MapItem : IMapObject, IMapSerializable
    {
        /// <summary>
        /// The item_type used as identifier in the map format.
        /// </summary>
        public abstract ItemType ItemType
        {
            get; 
        }

        /// <summary>
        /// Whether the item goes to .base or .aux.
        /// </summary>
        protected ItemFile itemFile;
        public virtual ItemFile ItemFile
        {
            get => itemFile;

            // Signs can be in base or aux depending on the model.
            // It's the only item that behaves like this, so for all other
            // items, the user of the library can't change the itemFile field.
            private set => itemFile = value;
        }

        /// <summary>
        /// The default location for the item type.
        /// </summary>
        public abstract ItemFile DefaultItemFile
        {
            get;
        }

        private KdopItem KdopItem;

        /// <summary>
        /// The UID of this item. 
        /// </summary>
        public ulong Uid
        {
            get => KdopItem.Uid;
            set => KdopItem.Uid = value;
        }

        /// <summary>
        /// The k-DOP bounding box which is used for rendering and collision detection.
        /// <para>Note that recomputing these values is out of scope for this library, so if you
        /// create or modify an object, don't forget to recompute the map in the editor.</para>
        /// </summary>
        public KdopBounds BoundingBox
        {
            get => KdopItem.BoundingBox;
            set => KdopItem.BoundingBox = value;
        }

        protected BitArray Flags
        {
            get => KdopItem.Flags;
            set => KdopItem.Flags = value;
        }

        /// <summary>
        /// View distance of an item in meters.
        /// </summary>
        protected ushort ViewDistance
        {
            get => KdopItem.ViewDistance;
            set => KdopItem.ViewDistance = value;
        }

        protected abstract ushort DefaultViewDistance
        {
            get;
        }

        /// <summary>
        /// Creates a new item and generates a UID for it.
        /// </summary>
        public MapItem()
        {
            KdopItem = new KdopItem(Utils.GenerateUuid());
            if (!(this is UnresolvedItem))
            {
                KdopItem.ViewDistance = DefaultViewDistance;
                itemFile = DefaultItemFile;
            }
        }

        /// <summary>
        /// Searches a list of all nodes for the nodes referenced by UID in this map item
        /// and adds references to them in the item's Node fields.
        /// </summary>
        /// <param name="allNodes">A dictionary of all nodes in the entire map.</param>
        public abstract void UpdateNodeReferences(Dictionary<ulong, Node> allNodes);

        private const int viewDistanceFactor = 10;

        /// <summary>
        /// Reads the item from a stream of a .base or .aux file
        /// whose position is at the start of the item.
        /// </summary>
        /// <param name="r">The reader.</param>
        public virtual void ReadFromStream(BinaryReader r)
        {
            // UID
            KdopItem.Uid = r.ReadUInt64();

            // kDOP bounding box
            KdopItem.BoundingBox.ReadFromStream(r);

            // Flags
            KdopItem.Flags = new BitArray(r.ReadBytes(4));

            // View distance
            KdopItem.ViewDistance = (ushort)((int)r.ReadByte() * viewDistanceFactor);
        }

        /// <summary>
        /// Writes the item to a stream.
        /// </summary>
        /// <param name="w">The writer.</param>
        public virtual void WriteToStream(BinaryWriter w)
        {
            // UID
            w.Write(KdopItem.Uid);

            // kDOP bounding box
            KdopItem.BoundingBox.WriteToStream(w);

            // Flags
            w.Write(KdopItem.Flags.ToUint());

            // View distance
            w.Write((byte)(KdopItem.ViewDistance / viewDistanceFactor));
        }

        /// <summary>
        /// Reads a list of objects.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="r"></param>
        /// <returns></returns>
        protected List<T> ReadObjectList<T>(BinaryReader r) where T : new()
        {
            var list = new List<T>();
            var count = r.ReadUInt32();
            if (typeof(IMapSerializable).IsAssignableFrom(typeof(T))) // scsreader objects
            {
                for (int i = 0; i < count; i++)
                {
                    var obj = new T();
                    (obj as IMapSerializable).ReadFromStream(r);
                    list.Add(obj);
                }
            }
            else if (typeof(IComparable).IsAssignableFrom(typeof(T))) // int, float etc.
            {
                for (int i = 0; i < count; i++)
                {
                    var val = r.Read<T>();
                    // copy-pasting methods suddenly doesn't seem so bad anymore
                    var Tval = (T)Convert.ChangeType(val, typeof(T)); 
                    list.Add(Tval);
                }
            }
            else
            {
                throw new NotImplementedException($"Don't know what to do with {typeof(T).Name}");
            }

            return list;
        }

        protected void WriteObjectList<T>(BinaryWriter w, List<T> list)
        {
            if(list is null)
            {
                w.Write(0);
                return;
            }

            w.Write((uint)list.Count);
            if (typeof(IMapSerializable).IsAssignableFrom(typeof(T))) // scsreader objects
            {
                foreach (var obj in list)
                {
                    (obj as IMapSerializable).WriteToStream(w);
                }
            }
            else if(typeof(IComparable).IsAssignableFrom(typeof(T))) // int, float etc.
            {
                foreach (var value in list)
                {
                    WriteListValue(w, value);
                }
            }
            else
            {
                throw new NotImplementedException($"Don't know what to do with {typeof(T).Name}");
            }
        }

        private static void WriteListValue<T>(BinaryWriter w, T value)
        {
            // dont @ me.
            if (value is bool _bool)
            {
                w.Write(_bool);
            }
            else if (value is byte _byte)
            {
                w.Write(_byte);
            }
            else if (value is sbyte _sbyte)
            {
                w.Write(_sbyte);
            }
            else if (value is char _char)
            {
                w.Write(_char);
            }
            else if (value is double _double)
            {
                w.Write(_double);
            }
            else if (value is float _float)
            {
                w.Write(_float);
            }
            else if (value is short _short)
            {
                w.Write(_short);
            }
            else if (value is int _int)
            {
                w.Write(_int);
            }
            else if (value is long _long)
            {
                w.Write(_long);
            }
            else if (value is ushort _ushort)
            {
                w.Write(_ushort);
            }
            else if (value is uint _uint)
            {
                w.Write(_uint);
            }
            else if (value is ulong _ulong)
            {
                w.Write(_ulong);
            }
            else
            {
                throw new NotImplementedException($"Don't know what to do with {typeof(T).Name}");
            }
        }

        protected List<Node> ReadNodeRefList(BinaryReader r)
        {
            var list = new List<Node>();
            var count = r.ReadUInt32();
            for (int i = 0; i < count; i++)
            {
                list.Add(new UnresolvedNode(r.ReadUInt64()));
            }
            return list;
        }

        protected List<MapItem> ReadItemRefList(BinaryReader r)
        {
            return ReadObjectList<UnresolvedItem>(r).Cast<MapItem>().ToList();
        }

        protected void WriteNodeRefList(BinaryWriter w, List<Node> nodeList)
        {
            if (nodeList is null)
            {
                w.Write(0);
                return;
            }

            w.Write(nodeList.Count);
            foreach (var node in nodeList)
            {
                w.Write(node.Uid);
            }
        }

        protected void WriteItemRefList(BinaryWriter w, List<MapItem> itemList)
        {
            if (itemList is null)
            {
                w.Write(0);
                return;
            }

            w.Write(itemList.Count);
            foreach (var item in itemList)
            {
                w.Write(item.Uid);
            }
        }
    }
}
