﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace ScsReader.ScsMap
{
    public class CutPlane : MapItem
    {
        public override ItemType ItemType => ItemType.CutPlane;

        public override ItemFile DefaultItemFile => ItemFile.Base;

        protected override ushort DefaultViewDistance => KdopItem.ViewDistanceClose;

        public List<Node> Nodes = new List<Node>();

        /// <summary>
        /// Determines if the cut plane is active in one direction only
        /// </summary>
        public bool OneSideOnly
        {
            get => Flags[0];
            set => Flags[0] = value;
        }

        /// <summary>
        /// Adds a cut plane to the map.
        /// </summary>
        /// <param name="map"></param>
        /// <param name="nodePositions"></param>
        /// <param name="oneSideOnly"></param>
        /// <returns></returns>
        public static CutPlane Add(IItemContainer map, Vector3[] nodePositions, bool oneSideOnly = false)
        {
            var cutPlane = new CutPlane();
            cutPlane.OneSideOnly = oneSideOnly;

            // the nodes have a forward node except for the last one
            // which just links to itself, so I'm creating the nodes in reverse
            // to create the references
            var posReverse = nodePositions.Reverse().ToArray(); 
            var nodes = new List<Node>();
            for(int i = 0; i < posReverse.Count(); i++)
            {
                var node = map.AddNode(posReverse[i]);
                node.ForwardItem = cutPlane;
                if(i == 0)
                {
                    node.ForwardItem = node;
                    node.IsRed = true;
                }
                else
                {
                    node.ForwardItem = nodes[i - 1];
                }
                nodes.Add(node);
            }

            nodes.Reverse();
            cutPlane.Nodes = nodes.ToList();

            // the item is added to the sector the first node is in.
            // if you store it somewhere else, it will still work, but 
            // it can't be deleted until you update it
            map.AddItem(cutPlane, nodes.Last());
            return cutPlane;
        }

        /// <summary>
        /// Appends a new node to the cut plane.
        /// </summary>
        /// <param name="position"></param>
        public void Append(Vector3 position)
        {
            var node = Nodes[0].Sectors[0].Map.AddNode(position);
            node.ForwardItem = this;
            node.ForwardItem = node;
            Nodes.Last().ForwardItem = node;
            Nodes.Add(node);
        }

        public override void ReadFromStream(BinaryReader r)
        {
            base.ReadFromStream(r);

            Nodes = ReadNodeRefList(r);
        }

        public override void WriteToStream(BinaryWriter w)
        {
            base.WriteToStream(w);

            WriteNodeRefList(w, Nodes);
        }

        public override void UpdateNodeReferences(Dictionary<ulong, Node> allNodes)
        {
            for (int i = 0; i < Nodes.Count; i++)
            {
                if (Nodes[i] is UnresolvedNode && allNodes.ContainsKey(Nodes[i].Uid))
                {
                    Nodes[i] = allNodes[Nodes[i].Uid];
                }
            }
        }
    }
}