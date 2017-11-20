namespace SpendManagementApi.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Http;
    using Models.Common;
    using SpendManagementLibrary;
    using SpendManagementLibrary.Enumerators;
    using CurrencyType = SpendManagementLibrary.CurrencyType;

    // todo: Please note that whilst this is almost useless for SpendManagementElement (see Task 57716)
    // todo: Its useful to convert this to a list of existing greenlights, so the client is fully informed.

    /// <summary>
    /// Returns lists of the current enums in the system, in a key/value style.
    /// </summary>
    [RoutePrefix("Enums")]
    public class EnumsController : BaseApiController
    {
        /// <summary>
        /// Gets all of the available Enum end points.
        /// </summary>
        /// <returns>A list of available resource Links</returns>
        [HttpOptions, Route("")]
        public List<Link> Options()
        {
            return Links("Options");
        }

        /// <summary>
        /// Init (nothing needs to happen here)
        /// </summary>
        protected override void Init() { }

        /// <summary>
        /// Gets all SpendManagementElement enum parts as a KeyValuePair.
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route("SpendManagementElement")]
        public EnumResponse GetSpendManagementElement()
        {
            return Generate<SpendManagementElement>();
        }

        /// <summary>
        /// Gets all MileageUOM enum parts as a KeyValuePair.
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route("MileageUOM")]
        public EnumResponse MileageUOM()
        {
            return Generate<MileageUOM>();
        }

        /// <summary>
        /// Gets all CurrencyType enum parts as a KeyValuePair.
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route("CurrencyType")]
        public EnumResponse CurrencyType()
        {
            return Generate<CurrencyType>();
        }

        /// <summary>
        /// Gets all CustomEntityElementType enum parts as a KeyValuePair.
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route("CustomEntityElementType")]
        public EnumResponse CustomEntityElementType()
        {
            return Generate<CustomEntityElementType>();
        }

        /// <summary>
        /// Gets all CalculationType enum parts as a KeyValuePair.
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route("CalculationType")]
        public EnumResponse CalculationType()
        {
            return Generate<CalculationType>();
        }
        
        /// <summary>
        /// Gets all DateRangeType enum parts as a KeyValuePair.
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route("DateRangeType")]
        public EnumResponse DateRangeType()
        {
            return Generate<DateRangeType>();
        }



        private EnumResponse Generate<TEnum>() where TEnum : struct, IConvertible, IComparable, IFormattable
        {
            if (!typeof(TEnum).IsEnum)
            {
                throw new ArgumentException("T must be an enumerated type");
            }

            var response = InitialiseResponse<EnumResponse>();

            var names = Enum.GetNames(typeof(TEnum));
            response.Elements = names.Select((value, key) => new { value, key })
                                     .ToDictionary(x => x.key + 1, x => x.value)
                                     .ToList();
            
            return response;
        }
    }

    /// <summary>
    /// The parts of the Enum enum, as a KeyValue pair.
    /// </summary>
    public class EnumResponse : ApiResponse
    {
        /// <summary>
        /// A list of all elements that represent The enum.
        /// </summary>
        public List<KeyValuePair<int, string>> Elements { get; set; }
    }
}
