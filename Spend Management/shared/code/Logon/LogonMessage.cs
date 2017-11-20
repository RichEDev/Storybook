namespace Spend_Management.shared.code.Logon
{
    using System;
    using System.Collections.Generic;
    /// <summary>
    /// Class to represent details about the Logon Message.
    /// </summary>

    [Serializable]
    public class LogonMessage
    {
      
        /// <summary>
        ///Logon message identifier.
        /// </summary>
        public int MessageId { get; set; }

        /// <summary>
        /// Category title
        /// </summary>
        public string CategoryTitle { get; set; }

        /// <summary>
        /// Category Title colour code.
        /// </summary>
        public string CategoryTitleColourCode { get; set; }

        /// <summary>
        /// Header text for Message
        /// </summary>
        public string HeaderText { get; set; }

        /// <summary>
        /// Header text colour code
        /// </summary>
        public string HeaderTextColourCode { get; set; }

        /// <summary>
        /// Body Text for Marketing panel
        /// </summary>
        public string BodyText { get; set; }

        /// <summary>
        /// Body Text colour code 
        /// </summary>
        public string BodyTextColourCode { get; set; }

        /// <summary>
        /// Back ground image for Marketing panel
        /// </summary>
        public string BackgroundImage { get; set; }

        /// <summary>
        ///Icon on Marketing panel
        /// </summary>
        public string Icon { get; set; }

        /// <summary>
        /// Button text on Marketing panel
        /// </summary>
        public string ButtonText { get; set; }

        /// <summary>
        ///Link for button
        /// </summary>
        public string ButtonLink { get; set; }

        /// <summary>
        /// Button Text colour
        /// </summary>
        public string ButtonForeColour { get; set; }

        /// <summary>
        ///Button Back Ground Colour
        /// </summary>
        public string ButtonBackGroundColour { get; set; }

        /// <summary>
        ///Archived Status
        /// </summary>
        public bool Archived { get; set; }
        /// <summary>
        /// Created employee id.
        /// </summary>
        public int? CreatedBy { get; set; }

        /// <summary>
        /// Modified employee id.
        /// </summary>
        public int? ModifiedBy { get; set; }

        /// <summary>
        /// Gets or sets the modules which this mesages can use
        /// </summary>
        public List<int> MessageModules { get; set; }

        /// <summary>
        /// Contructor for LogonMessage class
        /// </summary>
        /// <param name="messageId"></param>
        /// <param name="catogoryTitle"></param>
        /// <param name="categoryTitleColourCode"></param>
        /// <param name="headerText"></param>
        /// <param name="headerTextColourCode"></param>
        /// <param name="bodyText"></param>
        /// <param name="bodyTextColourCode"></param>
        /// <param name="backgroundImage"></param>
        /// <param name="icon"></param>
        /// <param name="buttonText"></param>
        /// <param name="buttonLink"></param>
        /// <param name="buttonForeColour"></param>
        /// <param name="buttonBackGroundColour"></param>
        /// <param name="archived"></param>
        /// <param name="moduleIds"></param>
        public LogonMessage(int messageId, string catogoryTitle, string categoryTitleColourCode, string headerText, string headerTextColourCode, string bodyText, string bodyTextColourCode, string backgroundImage, string icon, string buttonText, string buttonLink, string buttonForeColour, string buttonBackGroundColour, bool archived,List<int> moduleIds)
        {
            this.MessageId = messageId;
            this.CategoryTitle = catogoryTitle;
            this.CategoryTitleColourCode = categoryTitleColourCode;
            this.HeaderText = headerText;
            this.HeaderTextColourCode = headerTextColourCode;
            this.BodyText = bodyText;
            this.BodyTextColourCode = bodyTextColourCode;
            this.BackgroundImage = backgroundImage;
            this.Icon = icon;
            this.ButtonText = buttonText;
            this.ButtonLink = buttonLink;
            this.ButtonForeColour = buttonForeColour;
            this.ButtonBackGroundColour = buttonBackGroundColour;
            this.Archived = archived;
            this.MessageModules = moduleIds;
        }

        /// <summary>
        /// Default Constructor
        /// </summary>
        public LogonMessage()
        {

        }
    }


}