namespace SpendManagementApi.Repositories
{
    using System;
    using System.Collections.Generic;

    using SpendManagementApi.Interfaces;
    using SpendManagementLibrary.Addresses;

    using Spend_Management;

    using Favourite = SpendManagementApi.Models.Types.Favourite;

    /// <summary>
    /// The favourites repository.
    /// </summary>
    internal class FavouritesRepository : BaseRepository<Favourite>, ISupportsActionContext
    {

        private readonly IActionContext _actionContext;

        /// <summary>
        /// Creates a new FavouritesRepository.
        /// </summary>
        /// <param name="user">The Current User.</param>
        /// <param name="actionContext">The action context.</param>
        public FavouritesRepository(ICurrentUser user, IActionContext actionContext)
            : base(user, x => x.FavouriteId, x => x.AddressFriendlyName)
        {
            this._actionContext = actionContext;
        }

        /// <summary>
        /// Gets list of address favourites
        /// </summary>
        /// <param name="id">The Id</param>
        /// <param name="filter">The filter to apply</param>
        /// <returns>An IEnumerable of <see cref="Favourite">Favourite</see></returns>
        public IEnumerable<Favourite> GetFavourites(int id, AddressFilter filter)
        {

            var favourites = new List<Favourite>();
            List<SpendManagementLibrary.Addresses.Favourite> addressFavourites = ActionContext.Favourites.Get(User, filter, id);

            foreach (var favourite in addressFavourites)
            {
                favourites.Add(new Favourite().From(favourite, ActionContext));
            }

            return favourites;
        }

        /// <summary> 
        /// Gets an employees address favourites by its Id
        /// </summary>
        /// <param name="id">The Id</param>
        /// <returns>An Expense Category.</returns>
        public override Favourite Get(int id)
        {
            throw new NotImplementedException();
        }


        /// <summary>
        /// Gets all the favourite addresses in the system
        /// </summary>
        /// <returns>An I List of <see cref="Favourite">Favourite</see></returns>
        public override IList<Favourite> GetAll()
        {
            throw new NotImplementedException();
        }

    }
}