﻿using System;
using System.Collections.Generic;
using System.Text;

namespace ScsReader.Model.Pmd
{
    public class PartAttribute
    {
        public int Type { get; set; }

        public Token Tag { get; set; }

        public uint Value { get; set; }

        public override string ToString()
        {
            return $"{Tag.String}: {Value}";
        }
    }
}
