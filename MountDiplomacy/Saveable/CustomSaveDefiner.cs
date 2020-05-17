using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.SaveSystem;

namespace Wang.Saveable
{
    public class CustomSaveDefiner : SaveableTypeDefiner
    {
        // use a big number and ensure that no other mod is using a close range
        public CustomSaveDefiner() : base(1516880805) { }

        protected override void DefineClassTypes()
        {
            // The Id's here are local and will be related to the Id passed to the constructor
            AddClassDefinition(typeof(CanvassSave), 1);
            AddClassDefinition(typeof(CompanionHeroSave), 2);
        }

        protected override void DefineContainerDefinitions()
        {
            ConstructContainerDefinition(typeof(List<CanvassSave>));
            ConstructContainerDefinition(typeof(List<CompanionHeroSave>));
        }
    }

}
