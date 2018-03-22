namespace SpendManagementLibrary
{
    using System;

    /// <summary>
    /// cAttachmentAttribute class which inherits cAttribute
    /// </summary>
    public class cAttachmentAttribute : cAttribute
    {
        private AttributeFormat afFormat;

        /// <summary>
        /// cAttachmentAttribute constructor
        /// </summary>
        /// <param name="attributeid"></param>
        /// <param name="attributename"></param>
        /// <param name="displayname"></param>
        /// <param name="description"></param>
        /// <param name="tooltip"></param>
        /// <param name="mandatory"></param>
        /// <param name="fieldtype"></param>
        /// <param name="createdon"></param>
        /// <param name="createdby"></param>
        /// <param name="modifiedon"></param>
        /// <param name="modifiedby"></param>
        /// <param name="fieldid"></param>
        /// <param name="format"></param>
        /// <param name="isauditidentity"></param>
        /// <param name="isunique"></param>
        /// <param name="includeImageLibrary">Specifies whether a user will have access to the image library when uploading an attachment</param>
        /// <param name="allowedit"></param>
        /// <param name="allowdelete"></param>
        /// <param name="displayInMobile">Whether the attribute should be displayed on mobile.</param>
        /// <param name="system_attribute"></param>
        public cAttachmentAttribute(int attributeid, string attributename, string displayname, string description, string tooltip, bool mandatory, FieldType fieldtype, DateTime createdon, int createdby, DateTime? modifiedon, int? modifiedby, Guid fieldid, AttributeFormat format, bool isauditidentity, bool isunique, bool includeImageLibrary, bool allowedit, bool allowdelete, bool displayInMobile, bool builtIn, bool system_attribute = false)
            : base(
                attributeid, attributename, displayname, description, tooltip, mandatory, fieldtype, createdon,
                createdby, modifiedon, modifiedby, fieldid, false, isauditidentity, isunique, allowedit,
                allowdelete, displayInMobile, builtIn, includeImageLibrary, system_attribute, false)
        {
            this.afFormat = format;
        }
        #region properties
        /// <summary>
        /// Gets format type of text attribute
        /// </summary>
        public AttributeFormat format
        {
            get { return this.afFormat; }
        }

        public bool IncludeImageLibrary
        {
            get { return base.BoolAttribute; }
        }
        #endregion
    }
}