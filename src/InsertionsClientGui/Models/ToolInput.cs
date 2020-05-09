// Copyright (c) Microsoft. All rights reserved.

using System;
using System.Runtime.Serialization;

namespace Microsoft.Net.Insertions.Clients.Models
{
    [DataContract]
    internal struct ToolInput : IEquatable<ToolInput>
    {
        [DataMember(Name = "default.config")]
        internal string DefaultConfig { get; set; }

        [DataMember(Name = "manifest.json")]
        internal string ManifestJson { get; set; }

        [DataMember(Name = "propsdirectory")]
        internal OptionalInput PropsDirectory { get; set; }

        [DataMember(Name = "nuget.pat")]
        internal OptionalInput NuGetPat { get; set; }

        [DataMember(Name = "ignore.file")]
        internal OptionalInput IgnoreFile { get; set; }

        [DataMember(Name = "ignoredefaultasms")]
        internal bool IgnoreBlackListAssemblies { get; set; }


        public bool Equals(ToolInput other)
        {
            return DefaultConfig == other.DefaultConfig
                && ManifestJson == other.ManifestJson
                && PropsDirectory.Equals(other.PropsDirectory)
                && NuGetPat.Equals(other.NuGetPat)
                && IgnoreFile.Equals(other.IgnoreFile)
                && IgnoreBlackListAssemblies == other.IgnoreBlackListAssemblies;
        }
    }
}