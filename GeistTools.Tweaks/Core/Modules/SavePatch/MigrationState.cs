﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeistTools.Tweaks.Core.Modules.SavePatch
{
    internal enum MigrationState
    {
        Unknown,
        AlreadyMigrated,
        NeedsFirstMigration,
    }
}