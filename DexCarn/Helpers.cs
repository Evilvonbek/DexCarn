using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Kingmaker.Blueprints;
using TabletopTweaks.Core.Utilities;

namespace DexCarn.Utils
{
    internal static class Helpers
    {
        public static TBlueprint CloneBlueprint<TBlueprint>(TBlueprint blueprint, BlueprintGuid guid, string? name = null, bool addToLibrary = true)
        where TBlueprint : SimpleBlueprint
        {
            blueprint = (ObjectDeepCopier.Clone(blueprint) as TBlueprint)!;

            if (blueprint is BlueprintScriptableObject bso)
            {
                foreach (var c in bso.Components)
                {
                    c.OwnerBlueprint = bso;
                }
            }

            blueprint.AssetGuid = guid;

            if (name is not null) blueprint.name = name;

            if (addToLibrary) ResourcesLibrary.BlueprintsCache.AddCachedBlueprint(blueprint.AssetGuid, blueprint);

            return blueprint;
        }
    }
}