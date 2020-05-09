﻿// Copyright (c) Microsoft. All rights reserved.

using System.Collections.Generic;

namespace Microsoft.Net.Insertions.Common.Constants
{
    public static class InsertionConstants
    {
        public const string DefaultConfigFile = "default.config";

        public const string ManifestFile = "manifest.json";

        public static HashSet<string> DefaultDevUxTeamPackages { get; } = new HashSet<string>
        {
            "Microsoft.VisualStudio.LiveShare",
            "System.Reflection.Metadata",
            "VS.ExternalAPIs.MSBuild",
            "VS.Tools.Net.Core.SDK.Resolver",
            "VS.Tools.Net.Core.SDK.x86"
        };

        internal const string DefaultNugetFeed = "https://pkgs.dev.azure.com/devdiv/_packaging/VS-CoreXtFeeds/nuget/v3/index.json";
    }
}