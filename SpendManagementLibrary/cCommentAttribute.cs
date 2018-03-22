namespace SpendManagementLibrary
{
    using System;

    /// <summary>
    /// cCommentAttribute class which inherits cAttribute
    /// </summary>
    public class cCommentAttribute : cAttribute
    {
        private string sCommentText;

        /// <summary>
        /// cCommentAttribute constructor
        /// </summary>
        /// <param name="attributeid"></param>
        /// <param name="attributename"></param>
        /// <param name="displayname"></param>
        /// <param name="description"></param>
        /// <param name="tooltip"></param>
        /// <param name="mandatory"></param>
        /// <param name="createdon"></param>
        /// <param name="createdby"></param>
        /// <param name="modifiedon"></param>
        /// <param name="modifiedby"></param>
        /// <param name="commentText"></param>
        /// <param name="fieldid"></param>
        /// <param name="isauditidentity"></param>
        /// <param name="isunique"></param>
        /// <param name="allowedit"></param>
        /// <param name="allowdelete"></param>
        /// <param name="displayInMobile">Whether the attribute should be displayed on mobile.</param>
        /// <param name="system_attribute"></param>
        public cCommentAttribute(int attributeid, string attributename, string displayname, string description, string tooltip, bool mandatory, DateTime createdon, int createdby, DateTime? modifiedon, int? modifiedby, string commentText, Guid fieldid, bool isauditidentity, bool isunique, bool allowedit, bool allowdelete, bool displayInMobile, bool builtIn, bool system_attribute )
            : base(attributeid, attributename, displayname, description, tooltip, mandatory, FieldType.Comment, createdon, createdby, modifiedon, modifiedby, fieldid, false, isauditidentity, isunique, allowedit, allowdelete, displayInMobile, builtIn,false, system_attribute, false)
        {
            this.sCommentText = commentText;

            this.nMaxLabelTextLength = 50;
            this.sCssClass = "onecolumnpanel customentitycomment";
        }

        #region properties

        /// <summary>
        /// The information to be shown for the comment panel text
        /// </summary>
        public string commentText
        {
            get { return this.sCommentText; }
        }

        #endregion
    }
}