namespace PublicAPI
{
    using System.Collections.Generic;
    using BusinessLogic.ProjectCodes;
    using BusinessLogic.UserDefinedFields;
    using BusinessLogic.P11DCategories;
    using DTO;

    /// <summary>
    /// Enables the mapping of object to object conversion.
    /// </summary>
    public static class MapObjects
    {
        /// <summary>
        /// Creates mapping config for use when the application initializes.
        /// </summary>
        public static void Create()
        {
            AutoMapper.Mapper.Initialize(cfg =>
            {
                cfg.CreateMap<IEnumerable<KeyValuePair<int, object>>, UserDefinedFieldValueCollection>().ConvertUsing(x => new UserDefinedFieldValueCollection(x));
                cfg.CreateMap<UserDefinedFieldValueCollection, IEnumerable<KeyValuePair<int, object>>>().ConvertUsing(x => new List<KeyValuePair<int, object>>(x));
                cfg.CreateMap<ProjectCodeDto, ProjectCodeWithUserDefinedFields>().ForMember(pc => pc.UserDefinedFieldValues, m => m.MapFrom(dto => dto.UserDefinedFieldValues));
                cfg.CreateMap<ProjectCodeWithUserDefinedFields, ProjectCodeDto>().ForMember(dto => dto.UserDefinedFieldValues, m => m.MapFrom(pc => pc.UserDefinedFieldValues));
                cfg.CreateMap<IEnumerable<ProjectCodeDto>, IEnumerable<ProjectCodeWithUserDefinedFields>>();
                cfg.CreateMap<IEnumerable<P11DCategoryDto>, IEnumerable<P11DCategory>>().ReverseMap();
                cfg.CreateMap<P11DCategoryDto, P11DCategory>().ReverseMap();
            });
        }

        /// <summary>
        /// Convert the mapped type of <paramref name="source"/> to the mapped type of <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The type <paramref name="source"/> should be converted to.</typeparam>
        /// <param name="source">The source object to convert.</param>
        /// <returns>An instance of <typeparamref name="T"/> converted from the supplied <paramref name="source"/>.</returns>
        public static T Map<T>(object source)
        {
            return source == null ? default(T) : AutoMapper.Mapper.Map<T>(source);
        }
    }
}