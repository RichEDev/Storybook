namespace Expenses_Reports
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Xml;
    using SpendManagementLibrary;
    using SpendManagementLibrary.Definitions.JoinVia;
    using SpendManagementLibrary.Logic_Classes.Fields;
    public class JoinToJoinViaFactory
    {
        /// <summary>
        /// an instance of <see cref="JoinVias"/>
        /// </summary>
        private readonly JoinVias _joinVias;

        /// <summary>
        /// An instance of <see cref="IFields"/>
        /// </summary>
        private readonly IFields _fields;

        /// <summary>
        /// An instance of <see cref="cTables"/>
        /// </summary>
        private readonly cTables _tables;

        /// <summary>
        /// An instance of <see cref="cJoins"/>
        /// </summary>
        private readonly cJoins _joins;

        /// <summary>
        /// An instance of <see cref="DebugLogger"/>
        /// </summary>
        private readonly DebugLogger _debugLogger;

        /// <summary>
        /// The path of the default joins XML file, derived from the applicaton install path
        /// </summary>
        private readonly string _path;

        /// <summary>
        /// The <see cref="XmlDocument"/> read from the path file.
        /// </summary>
        private readonly XmlDocument _document;

        /// <summary>
        /// Create a new instance of <see cref="JoinToJoinViaFactory"/>
        /// </summary>
        /// <param name="joinVias">An instance of <see cref="JoinVias"/></param>
        /// <param name="fields">An instance of <see cref="IFields"/></param>
        /// <param name="tables">An instance of <see cref="cTables"/></param>
        /// <param name="joins">An instance of <see cref="cJoins"/></param>
        /// <param name="debugLogger">An instance of <see cref="DebugLogger"/></param>
        public JoinToJoinViaFactory(JoinVias joinVias, IFields fields, cTables tables, cJoins joins, DebugLogger debugLogger)
        {
            this._joinVias = joinVias;
            this._fields = fields;
            this._tables = tables;
            this._joins = joins;
            this._debugLogger = debugLogger;

            var assembly = Assembly.GetAssembly(this.GetType());
            if (assembly != null && !string.IsNullOrEmpty(assembly.Location))
            {
                this._path = assembly.Location.Replace(assembly.ManifestModule.Name, "DefaultJoinVia.Xml");
            }
            else
            {
                this._path = "DefaultJoinVia.Xml";
            }
            try
            {
                this._document = new XmlDocument();
                this._document.Load(this._path);
            }
            catch (Exception e)
            {
                this._debugLogger.Log("JoinToJoinViaFactory", e.Message, e.StackTrace);
                throw;
            }
            
        }

        /// <summary>
        /// Convert Join Tables join to a single <see cref="JoinVia"/>
        /// </summary>
        /// <param name="tableId">The <see cref="Guid"/>ID of the target table</param>
        /// <param name="baseTableId">The <see cref="Guid"/>ID of the starting table</param>
        /// <returns>Anin stance of <see cref="JoinVia"/>contains the join information.</returns>
        public JoinVia Convert(Guid tableId, Guid baseTableId)
        {
            var result = this.UseDefaultJoinVia(tableId, baseTableId);
            if (result != null)
            {
                return result;
            }

            return this.CreateJoinViaFromJoinTable(tableId, baseTableId);
        }

        /// <summary>
        /// Read the "default" join via for this join and use it if possible.
        /// </summary>
        /// <param name="tableId"></param>
        /// <param name="baseTableId"></param>
        /// <returns></returns>
        private JoinVia UseDefaultJoinVia(Guid tableId, Guid baseTableId)
        {
            JoinVia joinVia = null;
            try
            {
                var root = this._document.DocumentElement;
                if (root != null)
                    foreach (XmlNode join in root.ChildNodes)
                    {
                        if (@join.Attributes != null)
                        {
                            var baseTable = @join.Attributes["BaseTable"];
                            var table = join.Attributes["Table"];
                            var description = join.Attributes["Description"];
                            if (new Guid(baseTable.Value) == baseTableId && new Guid(table.Value) == tableId)
                            {
                                joinVia = new JoinVia(0, description.Value, Guid.NewGuid(), this.ExtractJoinSteps(join));
                                var result = this._joinVias.SaveJoinVia(joinVia);
                                return this._joinVias.GetJoinViaByID(result);
                            }
                        }
                    }
            }
            catch (Exception e)
            {
                this._debugLogger.Log("JoinToJoinViaFactory", e.Message, e.StackTrace);
                throw;
            }


            return null;
        }

        /// <summary>
        /// Extract the join steps from the <see cref="XmlDocument"/> containing the default join
        /// </summary>
        /// <param name="join">An instance of <see cref="XmlNode"/>Containing the Join information.</param>
        /// <returns>A <see cref="SortedList{TKey,TValue}"/>containg the Order and <seealso cref="JoinViaPart"/></returns>
        private SortedList<int, JoinViaPart> ExtractJoinSteps(XmlNode @join)
        {
            var result = new SortedList<int, JoinViaPart>();
            var order = 0;
            foreach (XmlNode joinStep in join.ChildNodes)
            {
                if (joinStep.Attributes != null)
                {
                    Guid related;
                    int idType =  0;
                    if (Guid.TryParse(joinStep.Attributes["RelatedId"].Value, out related) && int.TryParse(joinStep.Attributes["RelatedType"].Value, out idType))
                    {
                        var step = new JoinViaPart(related, (JoinViaPart.IDType) idType, JoinViaPart.JoinType.LEFT);                        
                        result.Add(order, step);
                        order++;
                    }
                }
            }

            return result;
        }


        /// <summary>
        /// Create/get a join via based on the old style jointables.
        /// </summary>
        /// <param name="tableId">The current table ID</param>
        /// <param name="baseTableId">The base table ID</param>
        /// 
        /// <returns>The new / curernt join via for the join between the two given tables.</returns>
        private JoinVia CreateJoinViaFromJoinTable(Guid tableId, Guid baseTableId)
        {
            var join = this._joins.GetJoin(baseTableId, tableId);
            if (join == null)
            {
                throw new Exception($"Join missing from '{baseTableId}' to '{tableId}'");
            }

            var baseIsCustomEntity = this._tables.GetTableByID(baseTableId).TableSource == cTable.TableSourceType.CustomEntites;

            var joinVia = new JoinVia(0, @join.description, Guid.NewGuid());
            var index = 0;
            foreach (cJoinStep joinStep in @join.Steps.Steps.Values)
            {
                Guid destinationKey = joinStep.destinationkey;
                if (destinationKey == Guid.Empty && joinStep.joinkey != Guid.Empty)
                {
                    destinationKey = joinStep.joinkey;
                }

               
                var joinType = JoinViaPart.IDType.Field;
               
                if (!baseIsCustomEntity)
                {
                    var destinationField = this._fields.GetFieldByTableAndFieldName(joinStep.destinationtableid, this._fields.GetFieldByID(destinationKey).FieldName);
                    if (destinationField == null)
                    {
                        destinationField = this._fields.GetFieldByTableAndFieldName(joinStep.destinationtableid,
                            this._fields.GetFieldByID(joinStep.joinkey).FieldName);
                    }
                    var sourceKey = this._fields.GetFieldByTableAndFieldName(joinStep.sourcetableid,  this._fields.GetFieldByID(joinStep.joinkey).FieldName);
                    if (sourceKey == null)
                    {
                        sourceKey = this._fields.GetFieldByTableAndFieldName(joinStep.sourcetableid, this._fields.GetFieldByID(joinStep.destinationkey).FieldName);
                    }
                    if (destinationField == null)
                    {
                        continue;
                    }

                    destinationKey = destinationField.FieldID;
                    if (sourceKey.IsForeignKey && (!destinationField.IsForeignKey || destinationField.RelatedTableID == Guid.Empty))
                    {
                        destinationKey = sourceKey.FieldID;
                    }
                }
                var destinationtable = this._tables.GetTableByID(joinStep.destinationtableid);
                var joinTypeForTable = destinationtable.JoinType == 1 ? JoinViaPart.JoinType.INNER : JoinViaPart.JoinType.LEFT;
                var joinViaPart = new JoinViaPart(destinationKey, joinType, joinTypeForTable);
                joinVia.JoinViaList.Add(index, joinViaPart);
                index++;
            }

            var result = this._joinVias.SaveJoinVia(joinVia);
            return this._joinVias.GetJoinViaByID(result);
        }

    }
}
