namespace SpendManagementLibrary.Employees
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;

    using SpendManagementLibrary.Interfaces;

    using Utilities.DistributedCaching;

    [Serializable]
    public class EmployeeViews
    {
        private readonly IDictionary<UserView, cUserView> _backingCollection;

        private readonly int _accountID;

        private readonly int _employeeID;

        public const string CacheArea = "employeeViews";

        public EmployeeViews(int accountID, int employeeID)
        {
            this._accountID = accountID;
            this._employeeID = employeeID;
            this._backingCollection = this.Get();
        }

        public int Count
        {
            get
            {
                return this._backingCollection.Count;
            }
        }

        public void Add(UserView userView, cUserView view) 
        {
            if (this._backingCollection.ContainsKey(userView))
            {
                this._backingCollection.Remove(userView);
            }

            this._backingCollection.Add(userView, view);

            this.CacheDelete();
        }

        public void Remove(UserView viewType)
        {
            if (this._backingCollection.ContainsKey(viewType))
            {
                this._backingCollection.Remove(viewType);
            }

            this.CacheDelete();
        }

        public cUserView GetBy(UserView view)
        {
            cUserView userView;
            this._backingCollection.TryGetValue(view, out userView);
            return userView;
        }

        private Dictionary<UserView, cUserView> Get()
        {
            return new Dictionary<UserView, cUserView>();
        }

        private void CacheAdd(Dictionary<UserView, cUserView> views)
        {
            Cache cache = new Cache();
            cache.Add(this._accountID, CacheArea, this._employeeID.ToString(CultureInfo.InvariantCulture), views);
        }

        private Dictionary<UserView, cUserView> CacheGet()
        {
            Cache cache = new Cache();
            return (Dictionary<UserView, cUserView>)cache.Get(this._accountID, CacheArea, this._employeeID.ToString(CultureInfo.InvariantCulture));
        }

        private void CacheDelete()
        {
            Cache cache = new Cache();
            cache.Delete(this._accountID, CacheArea, this._employeeID.ToString(CultureInfo.InvariantCulture));
        }
    }
}
