using BusinessLogic.DataConnections;
using BusinessLogic.P11DCategories;

namespace SpendManagementApi.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Interfaces;
    using Models.Common;
    using Models.Types;
    using SpendManagementLibrary;
    using Spend_Management;
    using ExpenseSubCategory = Models.Types.ExpenseSubCategory;
    using CalculationType = Common.Enums.CalculationType;
    using Models.Responses;
    using Utilities;
    using Models.Requests;

    using SpendManagementLibrary.Flags;

    internal class ExpenseSubCategoryRepository : BaseRepository<ExpenseSubCategory>, ISupportsActionContext
    {
        private cSubcats _cSubcats;   
        private readonly cCategories _cCategories;
        private readonly IDataFactory<IP11DCategory, int> _p11DCategories;

        public ExpenseSubCategoryRepository(ICurrentUser user, IActionContext actionContext = null)
            : base(user, actionContext, subcat => subcat.SubCatId, subcat => subcat.SubCat)
        {
            _cSubcats = this.ActionContext.SubCategories;
            _cCategories = this.ActionContext.Categories;
            this._p11DCategories = WebApiApplication.container.GetInstance<IDataFactory<IP11DCategory, int>>();
        }

        /// <summary>
        /// Gets all the Expense Item Sub Categories
        /// </summary>
        /// <returns>A list of <see cref="ExpenseSubCategory"></see>/></returns>
        public override IList<ExpenseSubCategory> GetAll()
        {
            return GetAllSubCategories();
        }

        /// <summary>
        /// Gets all the Expense Item Sub Categories relating to Mileage
        /// </summary>
        /// <returns>A list of <see cref="ExpenseSubCategory"></see>/></returns>
        public IList<ExpenseSubCategory> GetMileageSubCategories()
        {
            return ExpenseSubCategories(true);
        }


        /// <summary>
        /// Gets a specfic Expense Item Sub Category by its Id
        /// </summary>
        /// <param name="expenseSubCategoryId">The Id of the sub category</param>
        /// <returns>The <see cref="ExpenseSubCategory"></see>/></returns>
        public override ExpenseSubCategory Get(int expenseSubCategoryId)
        {
            var cSubcat = _cSubcats.GetSubcatById(expenseSubCategoryId);     
            var expenseSubCategory = cSubcat.Cast<ExpenseSubCategory>(_cCategories, User.AccountID, ActionContext);

            //Set limits for the Expense Subcategory
            List<SubcatItemRoleBasic> limits = _cSubcats.GetSubCatsByEmployeeItemRoles(this.User.EmployeeID, true);
            expenseSubCategory.ValidDates = new List<SubCatDates>();
            foreach (var subcatItemRoleBasic in limits.Where(l => l.SubcatId == expenseSubCategoryId))
            {
                var dates = new SubCatDates
                {
                    StartDate = subcatItemRoleBasic.StartDate,
                    EndDate = subcatItemRoleBasic.EndDate
                };
                expenseSubCategory.ValidDates.Add(dates);
            }

            SubcatItemRoleBasic roleSubcat = limits.OrderByDescending(x => x.Maximum).FirstOrDefault(subcat => subcat.SubcatId == expenseSubCategoryId);

            if (roleSubcat != null)
            {
                LimitFlag limitFlag = null;
                if (roleSubcat != null)
                {                 
                    limitFlag = 
                        this.ActionContext.FlagManagement.GetFlagByTypeRoleAndExpenseItem(
                            FlagType.LimitWithReceipt,
                            roleSubcat.ItemRoleID,
                            roleSubcat.SubcatId) as LimitFlag;
                }

                if (limitFlag != null && limitFlag.DisplayLimit && limitFlag.Active)
                {
                    expenseSubCategory.MaximumLimit = roleSubcat.Maximum;
                    expenseSubCategory.MaximumLimitWithoutReceipt = roleSubcat.ReceiptMaximum;

                    var employeeRepository = new EmployeeRepository(User, ActionContext);
                    var currencyRepository = new CurrencyRepository(User, ActionContext);

                    var currencyId = employeeRepository.DeterminePrimaryCurrenyId();

                    expenseSubCategory.CurrencySymbol = currencyRepository.DetermineCurrencySymbol(currencyId);

                }
            }
     
            return expenseSubCategory;
        }

        /// <summary>
        /// Gets a specfic Expense Item Sub Category by its Id
        /// </summary>
        /// <param name="expenseSubCategoryId">The Id of the sub category</param>
        /// <param name="employeeId"></param>
        /// <returns>The <see cref="ExpenseSubCategory"></see>/></returns>
        public ExpenseSubCategory GetSubCat(int expenseSubCategoryId, int employeeId)
        {
            var cSubcat = this._cSubcats.GetSubcatById(expenseSubCategoryId);
            var expenseSubCategory = cSubcat.Cast<ExpenseSubCategory>(this._cCategories, this.User.AccountID, this.ActionContext);

            //Set limits for the Expense Subcategory
            List<SubcatItemRoleBasic> limits = this._cSubcats.GetSubCatsByEmployeeItemRoles(employeeId, true);
            expenseSubCategory.ValidDates = new List<SubCatDates>();
            foreach (var subcatItemRoleBasic in limits.Where(l => l.SubcatId == expenseSubCategoryId))
            {
                var dates = new SubCatDates
                                {
                                    StartDate = subcatItemRoleBasic.StartDate,
                                    EndDate = subcatItemRoleBasic.EndDate
                                };
                expenseSubCategory.ValidDates.Add(dates);
            }

            SubcatItemRoleBasic roleSubcat = limits.OrderByDescending(x => x.Maximum).FirstOrDefault(subcat => subcat.SubcatId == expenseSubCategoryId);

            if (roleSubcat != null)
            {
                LimitFlag limitFlag = null;
                limitFlag =
                    this.ActionContext.FlagManagement.GetFlagByTypeRoleAndExpenseItem(
                        FlagType.LimitWithReceipt,
                        roleSubcat.ItemRoleID,
                        roleSubcat.SubcatId) as LimitFlag;

                if (limitFlag != null && limitFlag.DisplayLimit && limitFlag.Active)
                {
                    expenseSubCategory.MaximumLimit = roleSubcat.Maximum;
                    expenseSubCategory.MaximumLimitWithoutReceipt = roleSubcat.ReceiptMaximum;

                    var employeeRepository = new EmployeeRepository(this.User, this.ActionContext);
                    var currencyRepository = new CurrencyRepository(this.User, this.ActionContext);

                    var currencyId = employeeRepository.DeterminePrimaryCurrenyId();

                    expenseSubCategory.CurrencySymbol = currencyRepository.DetermineCurrencySymbol(currencyId);
                }
            }

            return expenseSubCategory;
        }

        /// <summary>
        /// Gets a list of the user's <see cref="SubcatItemRoleBasic">SubcatItemRoleBasic</see> .
        /// </summary>
        /// <param name="response">The <see cref="GetMySubCatsByItemRolesResponse">GetSubCatsByEmployeeItemRolesResponse</see>.</param>
        /// <returns>A list of the user's <see cref="SubcatItemRoleBasic">SubcatItemRoleBasic</see>.</returns>
        public List<ExpenseSubCategoryItemRoleBasic> GetByItemRoles(GetMySubCatsByItemRolesResponse response)       
        {
            var employees = ActionContext.Employees;
     
            if (employees.GetEmployeeById(this.User.EmployeeID) == null)
            {
                throw new ApiException(ApiResources.ApiErrorWrongEmployeeId,
                    ApiResources.ApiErrorWrongEmployeeIdMessage);
            }

            IEnumerable<SubcatItemRoleBasic> distinctSubcatItemRoleBasics = _cSubcats.GetSubCatsByEmployeeItemRoles(this.User.EmployeeID).DistinctBy(x => x.SubcatId);
            var subcatItemRoleBasics = new List<SubcatItemRoleBasic>();
        
            foreach (var subcat in distinctSubcatItemRoleBasics.Where(subcat => !subcatItemRoleBasics.Contains(subcat)))
            {
                subcatItemRoleBasics.Add(subcat);
            }

            response.List = subcatItemRoleBasics.Cast<List<ExpenseSubCategoryItemRoleBasic>>().ToList();
            return response.List;
        }

        /// <summary>
        /// Get the<see cref="ExpenseSubCategoryBasic">ExpenseSubCategoryBasic</see> for a subcat Id
        /// </summary>
        /// <param name="subcatId">The subcat Id</param>
        /// <returns></returns>
        public ExpenseSubCategoryBasic GetSubCatBasic(int subcatId)
        {
            SubcatBasic subcatBasic = _cSubcats.GetSubcatBasic(subcatId);

            if (subcatBasic == null)
            {
                throw new ApiException(ApiResources.ApiErrorWrongSubCatId,
                ApiResources.ApiErrorInvalidExpenseSubCategoryMessage);
            }

            return new ExpenseSubCategoryBasic().From(subcatBasic, ActionContext);   
        }

        /// <summary>
        /// Gets a list of <see cref="ExpenseSubCategoryNames">ExpenseSubCategoryNames</see> 
        /// </summary>
        /// <param name="expenseItemsubcatIds">The string of subcat Ids </param>
        /// <param name="response">The <see cref="GetSubCatNamesByIdsResponse">GetSubCatNamesByIdsResponse</see></param>
        /// <returns>A list of <see cref="ExpenseSubCategoryNames">ExpenseSubCategoryNames</see></returns>
        public List<ExpenseSubCategoryNames> GetBySubCatNamesByIds(string expenseItemsubcatIds, GetSubCatNamesByIdsResponse response)
        {
            string[] subcatIds = expenseItemsubcatIds.Split(',');
            var subcatIdsList = subcatIds.Select(subcatId => Convert.ToInt32(subcatId)).ToList();
   
            var request = new GetExpenseSubCategoryNamesByIdsRequest
            {
                SubCatIds = subcatIdsList
            };

            Dictionary<int, string> subcatNames = _cSubcats.GetSubcatNamesByIdList(request.SubCatIds);
            response.List = subcatNames.Cast<List<ExpenseSubCategoryNames>>().ToList();
            return response.List;
        }

        /// <summary>
        /// Gets a List of Subcats for the Category Id
        /// </summary>
        /// <param name="categoryId">The categoryId</param>
        /// <param name="isCorpCard">Whether it's for a corporate card reconciliation</param>
        /// <param name="isMobileJourney">Whether its for a mobile journey reconciliation</param>
        /// <returns>A list of <see cref="ListItemData">ListItemData</see></returns>
        public List<ListItemData> GetExpenseSubCatsForCategory(int categoryId, bool isCorpCard, bool isMobileJourney)
        {
            var subCategories = this.ActionContext.SubCategories;
            List<SubcatItemRoleBasic> subcatItemRoles = subCategories.GetSubCatsByEmployeeItemRoles(this.User.EmployeeID, true);
            SortedList<int, SubcatItemRoleBasic> subcatsForCategory = subCategories.GetExpenseSubCatsForCategory(categoryId, isCorpCard, isMobileJourney, this.User.EmployeeID, subcatItemRoles);

            return subcatsForCategory.Select(subcat => new ListItemData(subcat.Key, subcat.Value.Subcat)).ToList();
        }
      
        public override ExpenseSubCategory Add(ExpenseSubCategory expenseSubCategory)
        {
            base.Add(expenseSubCategory);

            if (_cSubcats.GetSubcatByString(expenseSubCategory.SubCat) != null)
            {
                throw new ApiException("Record Already Exists",
                                       "An Expense Sub Category with the specified name already exists");
            }

            if (expenseSubCategory.ParentCategoryId == 0 || _cCategories.CachedList().Where(cat => cat.categoryid == expenseSubCategory.ParentCategoryId) == null)
            {
                throw new ApiException(ApiResources.ApiErrorInvalidExpenseCategory, ApiResources.ApiErrorInvalidExpenseCategoryMessage);
            }

            if (expenseSubCategory.PdCatId.HasValue && expenseSubCategory.PdCatId.Value > 0 && String.IsNullOrEmpty(this._p11DCategories[expenseSubCategory.PdCatId.Value].Name))
            {
                throw new ApiException("Invalid P11d Category", "No P11d category exists with specified P11d category id");
            }

            if (expenseSubCategory.CalculationType == CalculationType.FixedAllowance &&
                expenseSubCategory.AllowanceAmount <= 0)
            {
                throw new ApiException("Invalid allowance amount", "Allowance amount specified is invalid for fixed allowance calculation type");
            }

            if (expenseSubCategory.VatApplicable && !expenseSubCategory.VatRates.Any())
            {
                    throw new ApiException("Missing VAT Rates","VAT rates not provided but VAT Applicable flag set");
            }

            if (expenseSubCategory.VatApplicable && !expenseSubCategory.VatRates.Any(rate=>rate.Validate()))
            {
                throw new ApiException("Invalid Vat Rate Date Ranges", "Invalid date range(s) provided for VAT rates");
            }

            UdfValidator.Validate(expenseSubCategory.AssociatedUdfs, this.ActionContext, "userdefinedExpenses", false);
            
            expenseSubCategory.UserDefined = UdfValidator.Validate(expenseSubCategory.UserDefined, this.ActionContext, "userdefinedSubcats");
            
            cSubcat cSubcat = expenseSubCategory.Cast<cSubcat>();
            
            var id = _cSubcats.SaveSubcat(cSubcat);
            expenseSubCategory.SubCatId = id;

            if (expenseSubCategory.VatRates != null && expenseSubCategory.VatRates.Any())
            {
                expenseSubCategory.VatRates.ForEach(rate => rate.SubCatId = cSubcat.subcatid);
                expenseSubCategory.VatRates.Select(v => v.Cast()).ToList().ForEach(vat => _cSubcats.SaveVatRate(vat));
            }

            if (expenseSubCategory.Split != null && expenseSubCategory.Split.Any())
            {
                List<int> subCatIds = expenseSubCategory.Split.Select(subcat => subcat.SubCatId).ToList();
                _cSubcats.AddSplit(cSubcat.subcatid, subCatIds);
            }

            this.ReinitialiseCache();

            _cSubcats = new cSubcats(User.AccountID);

            return Get(id);
        }

        public override ExpenseSubCategory Delete(int expenseSubCategoryId)
        {
            ExpenseSubCategory expenseSubCategory = Get(expenseSubCategoryId);
            if (expenseSubCategory == null)
            {
                throw new ApiException("Invalid Expense Sub Category Id",
                                       "No data available for specified expense sub category id");
            }

            int returnValue = _cSubcats.DeleteSubcat(expenseSubCategoryId);

            switch (returnValue)
            {
                case 1:
                    throw new ApiException("Expense Sub Category Delete Failed", "This expense item cannot be deleted because it is assigned to one or more items.");
                case 2:
                    throw new ApiException("Expense Sub Category Delete Failed",
                                           "This expense item cannot be deleted because it is assigned to one or more mobile expense items.");
                case 3:
                    throw new ApiException("Expense Sub Category Delete Failed",
                                           "The expense item cannot be deleted as it is associated with one or more flag rules.");
                case 4:
                    throw new ApiException("Expense Sub Category Delete Failed",
                                           "The expense item cannot be deleted as it is associated with one or more mobile journeys.");
                case -10:
                    throw new ApiException("Expense Sub Category Delete Failed", "This expense item cannot be deleted because it is assigned to one or more GreenLights or user defined fields.");
            }

            return expenseSubCategory;
        }

        public override ExpenseSubCategory Update(ExpenseSubCategory expenseSubCategory)
            {
            base.Update(expenseSubCategory);

            cSubcat origSubcat = _cSubcats.GetSubcatById(expenseSubCategory.SubCatId);

            if (origSubcat == null)
            {
                throw new ApiException("Invalid Sub Category Id", "No Sub Category record present to update");
            }

            int categoryId = expenseSubCategory.ParentCategoryId.Value;

            if (_cCategories.CachedList().Where(cat => cat.categoryid == categoryId) == null)
            {
                throw new ApiException("Invalid Category", "No category exists with specified category id");
            }

            if (expenseSubCategory.PdCatId.HasValue && expenseSubCategory.PdCatId.Value > 0 &&
                String.IsNullOrEmpty(this._p11DCategories[expenseSubCategory.PdCatId.Value].Name))
            {
                throw new ApiException("Invalid P11d Category",
                    "No P11d category exists with specified P11d category id");
            }

            if (expenseSubCategory.CalculationType == CalculationType.FixedAllowance &&
                expenseSubCategory.AllowanceAmount <= 0)
            {
                throw new ApiException("Invalid allowance amount", 
                    "Allowance amount specified is invalid for fixed allowance calculation type");
            }

            if (expenseSubCategory.VatApplicable && expenseSubCategory.VatRates.Count(v => !v.ForDelete) == 0)
            {
                throw new ApiException("Missing VAT Rates", "VAT rates not provided but VAT Applicable flag set");
            }

            if (expenseSubCategory.VatApplicable && !expenseSubCategory.VatRates.Any(rate => rate.Validate()))
            {
                throw new ApiException("Invalid Vat Rate Date Ranges", "Invalid date range(s) provided for VAT rates");
            }

            UdfValidator.Validate(expenseSubCategory.AssociatedUdfs, this.ActionContext, "userdefinedExpenses", false);
            expenseSubCategory.UserDefined = UdfValidator.Validate(expenseSubCategory.UserDefined, this.ActionContext, "userdefinedSubcats");
            
            cSubcat cSubcat = expenseSubCategory.Cast<cSubcat>(origSubcat);

            _cSubcats.SaveSubcat(cSubcat);

            if (expenseSubCategory.VatRates.Any())
            {
                //Records for update
                List<cSubcatVatRate> ratesToUpdate = origSubcat.vatrates.Join(expenseSubCategory.VatRates.Where(r => !r.ForDelete),
                    x => x.subcatid,
                    y => y.SubCatId,
                    (x, y) => y.Cast()).ToList();
                ratesToUpdate.ForEach(rate => UpdateVatRateSubCat(rate, origSubcat));

                //Records for delete
                List<cSubcatVatRate> ratesToDelete = origSubcat.vatrates.Join(expenseSubCategory.VatRates.Where(r => r.ForDelete),
                    x => x.subcatid,
                    y => y.SubCatId,
                    (x, y) => y.Cast()).ToList();
                ratesToDelete.ForEach(rate => UpdateVatRateSubCat(rate, origSubcat, true));

                //Records for add
                List<cSubcatVatRate> newVatRates = (from rate in expenseSubCategory.VatRates where origSubcat.subcatid == rate.SubCatId select rate.Cast()).ToList();

                foreach (var vatRate in newVatRates.ToList()) _cSubcats.SaveVatRate(vatRate);

            }

            if (origSubcat.subcatsplit.Any())
            {
                origSubcat.subcatsplit.ForEach(s => _cSubcats.DeleteSplit(cSubcat.subcatid, s));
            }

            List<int> subCatIds = expenseSubCategory.Split.Select(subcat => subcat.SubCatId).ToList();

            _cSubcats.AddSplit(cSubcat.subcatid, subCatIds);

            this.ReinitialiseCache();

            _cSubcats = new cSubcats(User.AccountID);
            expenseSubCategory = Get(origSubcat.subcatid);
            return expenseSubCategory;
        }

        private IList<ExpenseSubCategory> ExpenseSubCategories(bool mileageSubCatsOnly = false)
        {
            List<cSubcat> subcats = null;
            List<SubcatItemRoleBasic> subcatItemRoleBasics = _cSubcats.GetSubCatsByEmployeeItemRoles(this.User.EmployeeID).DistinctBy(x => x.SubcatId).ToList();

            if (mileageSubCatsOnly)
            {
                subcats = _cSubcats.GetMileageSubcats();

                if (subcats == null)
                {
                    throw new ApiException(ApiResources.APIErrorNoMileageSubCategoriesForAccount, ApiResources.APIErrorNoMileageSubCategoriesForAccountMessage);
                }

                //Builds up a list of mileage subcats that are related to the employee's item roles.
                var mileageSubcats = new List<ExpenseSubCategory>();
            
                foreach (var subcat in subcatItemRoleBasics)
                {
                    var item = subcats.FirstOrDefault(x => x.subcatid == subcat.SubcatId);

                    if (item != null)
                    {
                        mileageSubcats.Add(item.Cast<ExpenseSubCategory>(_cCategories, User.AccountID, ActionContext));
                    }
                }

                return mileageSubcats.ToList();
            }

           subcats =  _cSubcats.GetBigSortedList();
           var expenseSubCategories = new List<ExpenseSubCategory>();

            foreach (var subcat in subcatItemRoleBasics)
            {
                var item = subcats.FirstOrDefault(x => x.subcatid == subcat.SubcatId);

                if (item != null)
                {
                    expenseSubCategories.Add(item.Cast<ExpenseSubCategory>(_cCategories, User.AccountID, ActionContext));
                }
            }

            return expenseSubCategories.ToList();
        }

        /// <summary>
        /// Gets the employee's expense sub categories for mileage.
        /// </summary>
        /// <returns>
        /// A list of <see cref="ExpenseSubCategoriesMileageBasic">ExpenseSubCategoriesMileageBasic</see>
        /// </returns>
        /// <exception cref="ApiException">
        /// </exception>
        public IList<ExpenseSubCategoriesMileageBasic> EmployeeExpenseSubCategoriesForMileage()
        {
            List<cSubcat> subcats = null;
            List<SubcatItemRoleBasic> subcatItemRoleBasics = _cSubcats.GetSubCatsByEmployeeItemRoles(this.User.EmployeeID).DistinctBy(x => x.SubcatId).ToList();

            subcats = _cSubcats.GetMileageSubcats();

            if (subcats == null)
            {
                throw new ApiException(ApiResources.APIErrorNoMileageSubCategoriesForAccount, ApiResources.APIErrorNoMileageSubCategoriesForAccountMessage);
            }

            //Builds up a list of mileage subcats that are related to the employee's item roles.
            var mileageSubcats = new List<ExpenseSubCategoriesMileageBasic>();

            foreach (var subcat in subcatItemRoleBasics)
            {
                var item = subcats.FirstOrDefault(x => x.subcatid == subcat.SubcatId);

                if (item != null)
                {
                    mileageSubcats.Add(
                        new ExpenseSubCategoriesMileageBasic().From(item, ActionContext));
                }
            }

            return mileageSubcats.ToList();
        }

        private void UpdateVatRateSubCat(cSubcatVatRate vatRate, cSubcat subCat, bool forDelete = false)
        {
            cSubcatVatRate existingVatRate =
                subCat.vatrates.First(
                    rate => rate.subcatid == vatRate.subcatid && rate.vatrateid == vatRate.vatrateid);
            if (forDelete)
            {
                _cSubcats.DeleteVatRate(existingVatRate.vatrateid);
            }
            else
            {
                _cSubcats.SaveVatRate(vatRate);
            }
        }

        /// <summary>
        /// Gets all the <see cref="ExpenseSubCategory"/>.
        /// </summary>
        /// <returns>
        /// The a list of <see cref="ExpenseSubCategory"/>
        /// </returns>
        private IList<ExpenseSubCategory> GetAllSubCategories()
        {
            var subCats = this._cSubcats.GetBigSortedList();
            return subCats.Select(subCat => this.Get(subCat.subcatid)).ToList();
        }

        /// <summary>
        /// Create a new instance of <see cref="cSubcats">To ensure the cache has the correct Subcat data</see>
        /// </summary>
        private void ReinitialiseCache()
        {
            this.ActionContext.SubCategories = new cSubcats(this.User.AccountID);
        }

    }
}