// Copyright (c) Microsoft. All rights reserved.

using System;
using System.Runtime.Serialization;

namespace Microsoft.Net.Insertions.Clients.Models
{
    [DataContract]
    internal struct OptionalInput : IEquatable<OptionalInput>
    {
        [DataMember(Name = "ischosen")]
        internal bool IsChosen { get; set; }

        [DataMember(Name = "value")]
        internal string Value { get; set; }


        public bool Equals(OptionalInput other)
        {
            return IsChosen == other.IsChosen && Value == other.Value;
        }

        public override string ToString()
        {
            return $"Value is {(IsChosen ? string.Empty : " NOT ")} selected";
        }
    }
}