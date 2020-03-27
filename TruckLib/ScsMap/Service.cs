﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace TruckLib.ScsMap
{
    /// <summary>
    /// Additional data for service prefabs (gas stations, weigh stations etc).
    /// </summary>
    public class Service : PrefabSlaveItem
    {
        public override ItemType ItemType => ItemType.Service;

        public List<Node> Nodes { get; set; } = new List<Node>();

        public ServiceType ServiceType
        {
            get => (ServiceType)Kdop.Flags.GetByte(0);
            set => Kdop.Flags.SetByte(0, (byte)value);
        }

        public static Service Add(IItemContainer map, Prefab parent, Vector3 position)
        {
            return PrefabSlaveItem.Add<Service>(map, parent, position);
        }

        internal override void MoveRel(Vector3 translation)
        {
            base.MoveRel(translation);

            foreach (var node in Nodes)
            {
                node.Move(node.Position + translation);
            }
        }

        public override void UpdateNodeReferences(Dictionary<ulong, Node> allNodes)
        {
            base.UpdateNodeReferences(allNodes);

            for (int i = 0; i < Nodes.Count; i++)
            {
                if (Nodes[i] is UnresolvedNode
                    && allNodes.TryGetValue(Nodes[i].Uid, out var resolvedNode))
                {
                    Nodes[i] = resolvedNode;
                }
            }
        }

    }
}
