namespace SpendManagementApi.Controllers.Mobile.V1
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Http;
    using System.Web.Http.Description;

    using SpendManagementApi.Attributes;
    using SpendManagementApi.Attributes.Mobile;

    using SpendManagementLibrary;

    using Spend_Management;

    using Category = SpendManagementLibrary.Mobile.Category;
    using CategoryResult = SpendManagementLibrary.Mobile.CategoryResult;

    /// <summary>
    /// The controller to handle all <see cref="cCategories">cCategories</see> for mobile users.
    /// </summary>
    [Version(1)]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class MobileCategoriesV1Controller : BaseMobileApiController
    {
        /// <summary>
        /// Gets all expense item categories
        /// </summary>
        /// <returns>A <see cref="CategoryResult"/> containing all of the mobile users' categories.</returns>
        [HttpGet]
        [MobileAuth]
        [Route("mobile/categories")]
        public CategoryResult GetAll()
        {
            CategoryResult result = new CategoryResult { FunctionName = "GetExpenseItemCategories", ReturnCode = this.ServiceResultMessage.ReturnCode };

            if (this.ServiceResultMessage.ReturnCode == MobileReturnCode.Success) 
            {
                cCategories clsCats = new cCategories(this.PairingKeySerialKey.PairingKey.AccountID);

                List<Category> categories = clsCats.CachedList().Select(c => new Category
                    {
                        category = c.category,
                        categoryid = c.categoryid,
                        createdby = c.createdby,
                        createdon = c.createdon,
                        description = c.description,
                        modifiedby = c.modifiedby,
                        modifiedon = c.modifiedon
                    }).ToList();

                result.List = categories;   
            }
            
            return result;
        }
    }
}
