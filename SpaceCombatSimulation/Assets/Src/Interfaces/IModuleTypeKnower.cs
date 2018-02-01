﻿using Assets.Src.ModuleSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Src.Interfaces
{
    public interface IModuleTypeKnower: IGeneticConfigurable
    {
        List<ModuleType> Types { get; }
        float Cost { get; }
    }
}