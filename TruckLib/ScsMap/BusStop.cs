﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace TruckLib.ScsMap
{
    public class BusStop : PrefabSlaveItem
    {
        public override ItemType ItemType => ItemType.BusStop;

        public Token CityName { get; set; }

        public static BusStop Add(IItemContainer map, Prefab parent, Vector3 position)
        {
            var busStop = Add<BusStop>(map, position);
            busStop.PrefabLink = parent;
            parent.SlaveItems.Add(busStop);
            return busStop;
        }

        public override void ReadFromStream(BinaryReader r)
        {
            base.ReadFromStream(r);

            CityName = r.ReadToken();
            PrefabLink = new UnresolvedItem(r.ReadUInt64());
            Node = new UnresolvedNode(r.ReadUInt64());
        }

        public override void WriteToStream(BinaryWriter w)
        {
            base.WriteToStream(w);

            w.Write(CityName);
            w.Write(PrefabLink.Uid);
            w.Write(Node.Uid);
        }
    }
}
