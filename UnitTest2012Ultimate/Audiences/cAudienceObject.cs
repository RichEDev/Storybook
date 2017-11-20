using System;
using System.Collections.Generic;
using Spend_Management;
using SpendManagementLibrary;

namespace UnitTest2012Ultimate
{
    internal class cAudienceObject
    {
        public static cAudience New(List<int> employeeIDList = null, List<int> budgetHolderIDList = null, List<int> teamIDList = null)
        {
            ICurrentUser currentUser = Moqs.CurrentUser();
            cAudiences clsAudiences = new cAudiences(currentUser);
            int audRec = -1;

            try
            {
                audRec = clsAudiences.SaveAudience(0, "UT CE Audience " + DateTime.UtcNow.Ticks.ToString(), "An audience for a GreenLight");

                #region Add audience members
                if (employeeIDList == null && budgetHolderIDList == null && teamIDList == null)
                {
                    clsAudiences.SaveAudienceEmployees(audRec, new List<int> { currentUser.EmployeeID });
                }
                else
                {
                    if (employeeIDList != null && employeeIDList.Count > 0) clsAudiences.SaveAudienceEmployees(audRec, employeeIDList);
                    if (budgetHolderIDList != null && budgetHolderIDList.Count > 0) clsAudiences.SaveAudienceBudgetHolders(audRec, budgetHolderIDList);
                    if (teamIDList != null && teamIDList.Count > 0) clsAudiences.SaveAudienceTeams(audRec, teamIDList);
                }
                #endregion Add audience members

                clsAudiences = new cAudiences(currentUser);

                return clsAudiences.GetAudienceByID(audRec);
            }
            catch (Exception e)
            {
                try
                {
                    if (clsAudiences != null && audRec > 0)
                    {
                        clsAudiences.DeleteAudience(audRec);
                    }
                }
                finally
                {
                    throw new Exception("Error during setup of unit test dummy audience in <" + typeof(cAudienceObject).ToString() + ">.New", e);
                }
            }
        }

        public static bool TearDown(int entityID)
        {
            if (entityID > 0)
            {
                try
                {
                    ICurrentUser currentUser = Moqs.CurrentUser();
                    cAudiences clsAudiences = new cAudiences(currentUser);
                    int success = 0;
                    try
                    {
                        if (cAudienceRecordStatusObject._baseTablesUsed.ContainsKey(entityID) && cAudienceRecordStatusObject._baseTablesUsed[entityID] != null)
                        {
                            foreach (Guid tableID in cAudienceRecordStatusObject._baseTablesUsed[entityID])
                            {
                                cAudienceRecordStatusObject._DeleteAudienceRecords(entityID, tableID);
                                cAudienceRecordStatusObject._RemoveUsed(entityID, tableID);
                            }
                        }
                    }
                    finally
                    {
                        success = clsAudiences.DeleteAudience(entityID);
                    }

                    return (success > 0) ? true : false;
                }
                catch (Exception e)
                {
                    return false;
                }
            }

            return false;
        }
    }

    internal class cAudienceRecordStatusObject
    {
        public static SortedList<int, List<Guid>> _baseTablesUsed;

        public static void _AddUsed(int audienceID, Guid baseTableID)
        {
            if (_baseTablesUsed == null) _baseTablesUsed = new SortedList<int, List<Guid>> { { audienceID, new List<Guid> { baseTableID } } };
            else if (!_baseTablesUsed.ContainsKey(audienceID)) _baseTablesUsed.Add(audienceID, new List<Guid> { baseTableID });
            else if (!_baseTablesUsed[audienceID].Contains(baseTableID)) _baseTablesUsed[audienceID].Add(baseTableID);
        }

        public static void _RemoveUsed(int audienceID, Guid baseTableID)
        {
            if (_baseTablesUsed != null && _baseTablesUsed.ContainsKey(audienceID) && _baseTablesUsed[audienceID] != null && _baseTablesUsed[audienceID].Contains(baseTableID))
            {
                _baseTablesUsed[audienceID].Remove(baseTableID);
                if (_baseTablesUsed[audienceID].Count == 0) _baseTablesUsed.Remove(audienceID);
            }
        }

        public static cAudienceRecordStatus New(int relatedRecordID, Guid relatedAudienceTableID, List<int> audienceIDs, bool canView = true, bool canEdit = true, bool canDelete = true)
        {
            ICurrentUser currentUser = Moqs.CurrentUser();
            cAudiences clsAudiences = new cAudiences(currentUser);
            int recordID = 0;

            try
            {
                recordID = clsAudiences.SaveAudienceRecord(relatedRecordID, relatedAudienceTableID, audienceIDs, canView, canEdit, canDelete);
                foreach (int audienceID in audienceIDs) _AddUsed(audienceID, relatedAudienceTableID);

                clsAudiences = new cAudiences(currentUser);
                return clsAudiences.GetAudienceRecord(recordID, relatedAudienceTableID);
            }
            catch (Exception e)
            {
                try
                {
                    foreach (int audienceID in audienceIDs)
                    {
                        _DeleteAudienceRecords(audienceID, relatedAudienceTableID);
                        _RemoveUsed(audienceID, relatedAudienceTableID);
                    }
                }
                finally
                {
                    throw new Exception("Error during setup of unit test dummy audience record in <" + typeof(cAudienceRecordStatusObject).ToString() + ">.New", e);
                }
            }
        }

        /// <summary>
        /// Best not to use this one, use the cAudienceObject one
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static bool TearDown(cAudienceRecordStatus entity)
        {
            if (entity != null && entity.AudienceID.HasValue)
            {
                try
                {
                    ICurrentUser currentUser = Moqs.CurrentUser();
                    cAudiences clsAudiences = new cAudiences(currentUser);
                    foreach (Guid tableID in _baseTablesUsed[entity.AudienceID.Value])
                    {
                        _DeleteAudienceRecords(entity.AudienceID.Value, tableID);
                        _RemoveUsed(entity.AudienceID.Value, tableID);
                    }
                    return true;
                }
                catch (Exception e)
                {
                    return false;
                }
            }

            return false;
        }

        public static void _DeleteAudienceRecords(int audienceID, Guid baseTableID)
        {
            if (audienceID > 0 && baseTableID != null && baseTableID != Guid.Empty)
            {
                try
                {
                    ICurrentUser currentUser = Moqs.CurrentUser();
                    cTables clsTables = new cTables(currentUser.AccountID);
                    cTable baseTable = clsTables.GetTableByID(baseTableID);

                    if (baseTable != null)
                    {
                        DBConnection db = new DBConnection(cAccounts.getConnectionString(currentUser.AccountID));

                        string sql = "DELETE FROM [" + baseTable.TableName + "] WHERE [audienceid] = @audienceID";
                        db.sqlexecute.Parameters.AddWithValue("@audienceID", audienceID);
                        db.ExecuteSQL(sql);
                        db.sqlexecute.Parameters.Clear();
                    }
                }
                catch (Exception e)
                {
                    throw new Exception("An error occurred whilst trying to delete all audience records related to an audience in <" + typeof(cAudienceRecordStatusObject).ToString() + ">.DeleteAudienceRecords", e);
                }
            }
        }
    }
}
