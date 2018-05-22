namespace InternalApi
{
    using System.Collections.Generic;
    using System.Linq;

    using BusinessLogic.Announcements;
    using BusinessLogic.Employees.AccessRoles;
    using BusinessLogic.Validator;

    using InternalApi.DTO;

    /// <summary>
    /// Enables the mapping of object to object conversion
    /// </summary>
    public static class MapObjects
    {
        /// <summary>
        /// Creates mapping config for use when the application intializes
        /// </summary>
        public static void Create()
        {
            AutoMapper.Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<IEnumerable<AnnouncementDto>, IEnumerable<Announcement>>().ConstructUsing(an => new List<Announcement>()).ReverseMap().ConstructUsing(a => new List<AnnouncementDto>());
                    cfg.CreateMap<AnnouncementDto, Announcement>().ReverseMap();
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