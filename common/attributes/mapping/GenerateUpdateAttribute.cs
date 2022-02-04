using System;
using Mapster;

//Thanks to Mapster for this from Sample.Codegen.
namespace SS.Common.attributes.mapping
{
    public sealed class GenerateUpdateDto : AdaptFromAttribute
    {
        public GenerateUpdateDto() : base("Update[name]Dto")
        {
            Initialize();
        }

        public GenerateUpdateDto(Type type) : base(type)
        {
            Initialize();
        }

        private void Initialize()
        {
            IgnoreAttributes = new[]
            {
                typeof(ExcludeFromAddAndUpdateDtoAttribute),
                typeof(ExcludeFromUpdateDtoAttribute)
            };
            MapType = MapType.MapToTarget;
            ShallowCopyForSameType = true;
        }
    }
}
