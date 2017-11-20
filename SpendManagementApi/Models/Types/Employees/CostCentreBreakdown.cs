namespace SpendManagementApi.Models.Types.Employees
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    
    using SpendManagementLibrary;
    using Interfaces;
    using Utilities;

    internal static class CostCentreBreakdownConversion
    {
        public static List<cDepCostItem> Cast<TResult>(this List<CostCentreBreakdown> costCentres)
            where TResult : List<cDepCostItem>, new()
        {
            return
                costCentres.Select(
                    cc => new cDepCostItem(cc.DepartmentId ?? 0, cc.CostCodeId ?? 0, cc.ProjectCodeId ?? 0, cc.Percentage)).ToList();
        }

        public static TResult Cast<TResult>(this IList<cDepCostItem> costCentreBreakdowns)
              where TResult: List<CostCentreBreakdown>, new()
        {
            List<CostCentreBreakdown> ccs =
                costCentreBreakdowns.Select(
                    cc =>
                    new CostCentreBreakdown
                        {
                            CostCodeId = cc.costcodeid,
                            DepartmentId = cc.departmentid,
                            ProjectCodeId = cc.projectcodeid,
                            Percentage = cc.percentused
                        }).ToList();
            return (TResult) ccs;
        }
    }


    /// <summary>
    /// Cost Centre Percentage.
    /// </summary>
    public class CostCentreBreakdown : BaseExternalType, IRequiresValidation, IEquatable<CostCentreBreakdown>
    {
        /// <summary>
        /// Gets or sets the code for the element to which the percentage is allocated (the project code type)
        /// </summary>
        public int? DepartmentId { get; set; }

        /// <summary>
        /// Gets or sets the department description.
        /// </summary>
        public string DepartmentDescription { get; set; }

        /// <summary>
        /// Gets or sets the code for the element to which the percentage is allocated (the project code type)
        /// </summary>
        public int? CostCodeId { get; set; }

        /// <summary>
        /// Gets or sets the cost code description.
        /// </summary>
        public string CostCodeDescription { get; set; }

        /// <summary>
        /// Gets or sets the code for the element to which the percentage is allocated (the project code type)
        /// </summary>
        public int? ProjectCodeId { get; set; }

        /// <summary>
        /// Gets or sets the project code description.
        /// </summary>
        public string ProjectCodeDescription { get; set; }

        /// <summary>
        /// Gets or sets the percentage of allocation
        /// </summary>
        public int Percentage { get; set; }

        /// <summary>
        /// Validates this object using an actionContext.
        /// </summary>
        /// <param name="actionContext">The action context.</param>
        /// <exception cref="InvalidDataException">Any data errors that arise.</exception>
        public void Validate(IActionContext actionContext)
        {
            if (Percentage <= 0 || Percentage > 100)
            {
                throw new InvalidDataException(ApiResources.ApiErrorCostCentrePercentageOutOfBounds);
            }

            // check these exist and aren't archived.
            if (DepartmentId.HasValue && DepartmentId > 0)
            {
                var items = actionContext.Departments;
                var item = items.GetDepartmentById(DepartmentId.Value);

                if (item == null)
                {
                    throw new InvalidDataException(ApiResources.ApiErrorCostCentreNonExistentDepartment + DepartmentId);
                }

                if (item.Archived)
                {
                    throw new InvalidDataException(ApiResources.ApiErrorCostCentreArchivedDepartment + DepartmentId);
                }

            }

            if (CostCodeId.HasValue && CostCodeId > 0)
            {
                var items = actionContext.CostCodes;
                var item = items.GetCostcodeById(CostCodeId.Value);

                if (item == null)
                {
                    throw new InvalidDataException(ApiResources.ApiErrorCostCentreNonExistentCostCode + CostCodeId);
                }

                if (item.Archived)
                {
                    throw new InvalidDataException(ApiResources.ApiErrorCostCentreArchivedCostCode + CostCodeId);
                }
            }

            if (ProjectCodeId.HasValue && ProjectCodeId > 0)
            {
                var items = actionContext.ProjectCodes;
                var item = items.getProjectCodeById(ProjectCodeId.Value);

                if (item == null)
                {
                    throw new InvalidDataException(ApiResources.ApiErrorCostCentreNonExistentProjectCode + ProjectCodeId);
                }

                if (item.archived)
                {
                    throw new InvalidDataException(ApiResources.ApiErrorCostCentreArchivedProjectCode + ProjectCodeId);
                }
            }
        }

        public bool Equals(CostCentreBreakdown other)
        {
            if (other == null)
            {
                return false;
            }
            return this.CostCodeId.Equals(other.CostCodeId ?? 0) && this.DepartmentId.Equals(other.DepartmentId ?? 0)
                   && this.Percentage.Equals(other.Percentage) && this.ProjectCodeId.Equals(other.ProjectCodeId ?? 0);
        }

        public override bool Equals(object obj)
        {
            return this.Equals(obj as CostCentreBreakdown);
        }
    }
}
