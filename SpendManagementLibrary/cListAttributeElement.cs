namespace SpendManagementLibrary
{
    using System;

    [Serializable]
    public class cListAttributeElement
    {
        /// <summary>
        /// The n value.
        /// </summary>
        [NonSerialized]
        private int nValue;

        /// <summary>
        /// The s text.
        /// </summary>
        [NonSerialized]
        private string sText;

        /// <summary>
        /// The n sequence.
        /// </summary>
        [NonSerialized]
        private int nSequence;

        #region properties

        /// <summary>
        /// Gets the element value.
        /// </summary>
        public int elementValue
        {
            get { return this.nValue; }
        }

        /// <summary>
        /// Gets the element text.
        /// </summary>
        public string elementText
        {
            get { return this.sText; }
        }

        /// <summary>
        /// Gets the element order.
        /// </summary>
        public int elementOrder
        {
            get { return this.nSequence; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether archived.
        /// </summary>
        public bool Archived { get; set; }
        #endregion

        /// <summary>
        /// Initialises a new instance of the <see cref="cListAttributeElement"/> class.
        /// </summary>
        /// <param name="elementValue">
        /// The element value.
        /// </param>
        /// <param name="elementText">
        /// The element text.
        /// </param>
        /// <param name="sequence">
        /// The sequence.
        /// </param>
        /// <param name="Archived">
        /// The Archived flag.
        /// </param>
        public cListAttributeElement(int elementValue, string elementText, int sequence, bool Archived = false)
        {
            this.Archived = Archived;
            this.sText = elementText;
            this.nValue = elementValue;
            this.nSequence = sequence;
        }
    }
}