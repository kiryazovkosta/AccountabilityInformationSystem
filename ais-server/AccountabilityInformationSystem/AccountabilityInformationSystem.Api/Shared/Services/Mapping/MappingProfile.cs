using System.Reflection;
using Mapster;

namespace AccountabilityInformationSystem.Api.Shared.Services.Mapping;

public class MappingProfile : IRegister
{

    public void Register(TypeAdapterConfig config)
    {
        Type[] types = Assembly
            .GetExecutingAssembly()
            .GetTypes()
            .Where(t => !t.IsAbstract 
                && !t.IsInterface 
                && (t.IsAssignableTo(typeof(IMapFrom<>)) 
                    || t.IsAssignableTo(typeof(IMapTo<>)) 
                    || t.IsAssignableTo(typeof(IMapCustom))))
            .ToArray();

        foreach (Type type in types)
        {
            Console.WriteLine(type.Name);

            // IMapFrom<>
            var mapFromInterface = type.GetInterface(typeof(IMapFrom<>).Name);
            if (mapFromInterface is not null)
            {
                var source = mapFromInterface.GetGenericArguments()[0];
                config.NewConfig(source, type);
            }

            // IMapTo<>
            var mapToInterface = type.GetInterface(typeof(IMapTo<>).Name);
            if (mapToInterface is not null)
            {
                var destination = mapToInterface.GetGenericArguments()[0];
                config.NewConfig(type, destination);
            }

            // IMapCustom
            if (typeof(IMapCustom).IsAssignableFrom(type) && !type.IsInterface)
            {
                IMapCustom instance = (IMapCustom)Activator.CreateInstance(type);
                instance!.CreateMappings(config);
            }
        }
    }
}
