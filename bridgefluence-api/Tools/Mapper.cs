using Mapster;

namespace bridgefluence_api.Tools;

public static class Mapper
{
    public static TDestination Map<TDestination>(this object source) => source.Adapt<TDestination>(); 
}