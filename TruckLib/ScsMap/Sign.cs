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
    /// A traffic sign or navigation sign.
    /// </summary>
    public class Sign : SingleNodeItem
    {
        public override ItemType ItemType => ItemType.Sign;

        public override ItemFile DefaultItemFile => ItemFile.Aux;

        public new ItemFile ItemFile
        {
            get => itemFile;
            // Most signs go to aux, unless they have a traffic_rule attached to them
            // in the .sii definition, in which case they go to base.
            // This library has no way of knowing that, so it relies on the user
            // to deal with it.
            set => itemFile = value;
        }

        protected override ushort DefaultViewDistance => KdopItem.ViewDistanceClose;

        public new ushort ViewDistance
        {
            get => base.ViewDistance;
            set => base.ViewDistance = value;
        }

        /// <summary>
        /// The unit name of the sign model.
        /// </summary>
        public Token Model { get; set; }

        private const int signBoardCount = 3;
        /// <summary>
        /// Sign text for legacy navigation signs (e.g. be-navigation/board straight left right b).
        /// </summary>
        public SignBoard[] SignBoards { get; set; } = new SignBoard[signBoardCount];

        /// <summary>
        /// The sign template on this sign.
        /// </summary>
        /// <example>sign_templ.balt_40</example>
        public string SignTemplate { get; set; }

        /// <summary>
        /// The attribute overrides used on the sign template.
        /// </summary>
        public List<SignOverride> SignOverrides { get; set; } 
            = new List<SignOverride>();

        /// <summary>
        /// Adds a sign to the map.
        /// </summary>
        /// <param name="map"></param>
        /// <param name="position"></param>
        /// <param name="model"></param>
        /// <param name="template"></param>
        /// <returns></returns>
        public static Sign Add(IItemContainer map, Vector3 position, Token model, string template)
        {
            var sign = Add<Sign>(map, position);
            sign.Model = model;
            sign.SignTemplate = template;
            return sign;
        }

        public override void ReadFromStream(BinaryReader r)
        {
            base.ReadFromStream(r);

            Model = r.ReadToken();
            Node = new UnresolvedNode(r.ReadUInt64());

            // sign_boards
            // used for legacy signs.
            for (int i = 0; i < SignBoards.Length; i++)
            {
                SignBoards[i].Road = r.ReadToken();
                SignBoards[i].City1 = r.ReadToken();
                SignBoards[i].City2 = r.ReadToken();
            }

            // override_template
            SignTemplate = r.ReadPascalString();
            // if override_template is an empty string,
            // the file does not contain the sign override array
            if (SignTemplate == "") return;

            // sign_override
            SignOverrides = ReadObjectList<SignOverride>(r);
        }

        public override void WriteToStream(BinaryWriter w)
        {
            base.WriteToStream(w);

            w.Write(Model);
            w.Write(Node.Uid);

            foreach (var board in SignBoards)
            {
                w.Write(board.Road);
                w.Write(board.City1);
                w.Write(board.City2);
            }

            w.WritePascalString(SignTemplate);
            if (SignTemplate == "") return;

            WriteObjectList(w, SignOverrides);
        }

        /// <summary>
        /// Sign text struct for legacy navigation signs, 
        /// e.g. be-navigation->board straight left right b.
        /// </summary>
        public struct SignBoard
        {
            public Token Road;
            public Token City1;
            public Token City2;
        }

    }
}