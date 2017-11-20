using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SpendManagementLibrary;
using System.Text.RegularExpressions;
using System.Collections.Specialized;

namespace Spend_Management
{
    public class cBreadCrumbs
    {
        public static SiteMapNode SetBreadCrumbs(Object sender, SiteMapResolveEventArgs e)
        {
            CurrentUser curUser = cMisc.GetCurrentUser();
            SiteMapNode currentNode = SiteMap.CurrentNode;
            if (currentNode == null)
            {
                return null;
            }
            currentNode = currentNode.Clone(true);
            if (currentNode != null)
            {
                try
                {
                    SiteMapNode tempNode = currentNode;
                    cAccountSubAccounts clsSubAccounts = new cAccountSubAccounts(curUser.AccountID);
                    cAccountProperties clsProperties = clsSubAccounts.getSubAccountById(curUser.CurrentSubAccountId).SubAccountProperties;

                    string pageName = e.Context.Request.Url.Segments[e.Context.Request.Url.Segments.Length - 1];
                    NameValueCollection pageQuery = e.Context.Request.QueryString;

                    if (pageName.Contains("supplier_details.aspx") == true)
                    {
                        tempNode.Title = clsProperties.SupplierPrimaryTitle;
                        if (tempNode.ParentNode != null)
                        {
                            if (clsProperties.SupplierPrimaryTitle.Substring(clsProperties.SupplierPrimaryTitle.Length - 1) == "s")
                            {
                                tempNode.ParentNode.Title = clsProperties.SupplierPrimaryTitle;
                            }
                            else
                            {
                                tempNode.ParentNode.Title = clsProperties.SupplierPrimaryTitle + "s";
                            }

                            tempNode.ParentNode.Url = cMisc.Path + "/shared/suppliers.aspx";


                            //if (string.IsNullOrEmpty(e.Context.Request.QueryString["returnURL"]) && e.Context.Request.QueryString["returnURL"].Contains("sid"))
                            //{
                            //}

                            /// If this is ever used replace the RawUrl check with the one above
                            if (e.Context.Request.RawUrl.Contains("sid%3d") == true)
                            {
                                Regex r = new Regex(@"sid\%3d(\d+)", RegexOptions.IgnoreCase);
                                Match m = r.Match(e.Context.Request.RawUrl, e.Context.Request.RawUrl.IndexOf("supplier_details.aspx"));

                                if (m.Success && m.Captures.Count > 0)
                                {
                                    int sID;
                                    int.TryParse(m.Captures[0].Value, out sID);
                                    if (sID > 0)
                                    {
                                        tempNode.Url = cMisc.Path + "/shared/supplier_details.aspx?sid=" + sID.ToString();
                                    }
                                }
                            }
                        }
                    }
                    else if (pageName.Contains("suppliers.aspx") == true)
                    {
                        if (clsProperties.SupplierPrimaryTitle.Substring(clsProperties.SupplierPrimaryTitle.Length - 1) == "s")
                        {
                            tempNode.Title = clsProperties.SupplierPrimaryTitle;
                        }
                        else
                        {
                            tempNode.Title = clsProperties.SupplierPrimaryTitle + "s";
                        }
                    }
                    else if (pageName.Contains("MenuMain.aspx") == true)
                    {
                        if (pageQuery.ToString().Contains("supplierinfo") == true)
                        {
                            tempNode.Title = clsProperties.SupplierPrimaryTitle + " Configuration";
                        }                                               
                    }
                    else if (pageName.Contains("basedefinitions.aspx") == true)
                    {
                        if (pageQuery.ToString().Contains("bdt=55") == true)
                        {
                            tempNode.Title = clsProperties.SupplierPrimaryTitle + " Status";
                            tempNode.ParentNode.Title = clsProperties.SupplierPrimaryTitle + " Configuration";
                        }
                        if (pageQuery.ToString().Contains("bdt=53") == true)
                        {
                            tempNode.Title = clsProperties.SupplierCatTitle;
                            tempNode.ParentNode.Title = clsProperties.SupplierPrimaryTitle + " Configuration";
                        }
                        if (pageQuery.ToString().Contains("bdt=109") == true)
                        {
                            tempNode.Title = clsProperties.ContractCategoryTitle;
                        }
                        if (pageQuery.ToString().Contains("bdt=129") == true)
                        {
                            tempNode.ParentNode.Title = clsProperties.SupplierPrimaryTitle + " Configuration";
                        }
                    }
                    else if (pageName.Contains("reassigncontractsupplier.aspx") == true)
                    {
                        tempNode.Title = "Reassign Contract " + clsProperties.SupplierPrimaryTitle;
                    }
                    else if (pageName.Contains("claimViewer.aspx"))
                    {
                        CurrentUser user = cMisc.GetCurrentUser();
                        cClaims claims = new cClaims(user.AccountID);
                        int claimId = 0;
                        Int32.TryParse(e.Context.Request.QueryString["claimid"], out claimId);
                        cClaim claim = claims.getClaimById(claimId);
                        if (claim != null)
                        {
                            switch (claim.ClaimStage)
                            {
                                case ClaimStage.Current:
                                    tempNode.Title = "Current Claims";
                                    break;
                                case ClaimStage.Submitted:
                                    tempNode.Title = "Submitted Claims";
                                    break;
                                case ClaimStage.Previous:
                                    tempNode.Title = "Previous Claims";
                                    break;
                            }
                        }
                        
                    }
                }
                catch (HttpException ex)
                {

                }
            }
            return currentNode;
        }
    }
}
