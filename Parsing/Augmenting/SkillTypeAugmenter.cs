﻿/*///////////////////////////////////////////////////////////////////
<EasyFarm, general farming utility for FFXI.>
Copyright (C) <2013>  <Zerolimits>

This program is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
*/
///////////////////////////////////////////////////////////////////

using Parsing.Mapping;
using Parsing.Types;

namespace Parsing.Augmenting
{
    public class SkillTypeAugmenter : SpecializedTypeAugmenter<SkillType>
    {
        public SkillTypeAugmenter(string attributeName, string variableName) :
            base(attributeName, variableName)
        {
            Mappers.Add(new ObjectMapper<string, SkillType>("HealingMagic", SkillType.HealingMagic));
            Mappers.Add(new ObjectMapper<string, SkillType>("DivineMagic", SkillType.DivineMagic));
            Mappers.Add(new ObjectMapper<string, SkillType>("EnfeeblingMagic", SkillType.EnfeeblingMagic));
            Mappers.Add(new ObjectMapper<string, SkillType>("EnhancingMagic", SkillType.EnhancingMagic));
            Mappers.Add(new ObjectMapper<string, SkillType>("ElementalMagic", SkillType.ElementalMagic));
            Mappers.Add(new ObjectMapper<string, SkillType>("DarkMagic", SkillType.DarkMagic));
            Mappers.Add(new ObjectMapper<string, SkillType>("SummoningMagic", SkillType.SummoningMagic));
            Mappers.Add(new ObjectMapper<string, SkillType>("Ninjutsu", SkillType.Ninjutsu));
            Mappers.Add(new ObjectMapper<string, SkillType>("Singing", SkillType.Singing));
            Mappers.Add(new ObjectMapper<string, SkillType>("BlueMagic", SkillType.BlueMagic));
            Mappers.Add(new ObjectMapper<string, SkillType>("Geomancy", SkillType.Geomancy));
            Mappers.Add(new ObjectMapper<string, SkillType>("Ability", SkillType.Ability));
        }
    }
}