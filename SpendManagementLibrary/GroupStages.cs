namespace SpendManagementLibrary
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;

    /// <summary>
    /// The sign-off stages for a specific group.
    /// </summary>
    [Serializable]
    public class GroupStages : IEnumerable<cStage>
    {
        /// <summary>
        /// The private dictionary of stages.
        /// </summary>

        private readonly SerializableDictionary<int, cStage> _stages;

        /// <summary>
        /// Initialises a new instance of the <see cref="GroupStages"/> class.
        /// </summary>
        /// <param name="stages">
        /// The group stages to store.
        /// </param>
        public GroupStages(SerializableDictionary<int, cStage> stages)
        {
            this._stages = stages;
        }

        /// <summary>
        /// Gets the stages as a read-only collection.
        /// </summary>
        public ReadOnlyCollection<cStage> Values
        {
            get
            {
                return this._stages != null ? this._stages.Values.OrderBy(s => s.stage).ToList().AsReadOnly() : new ReadOnlyCollection<cStage>(new cStage[0]);
            }
        }

        /// <summary>
        /// Gets the count of the stages.
        /// </summary>
        public int Count
        {
            get
            {
                return this._stages.Count;
            }
        }

        /// <summary>
        /// Get a specific item from the list of stages.
        /// </summary>
        /// <param name="signoffId">
        /// The signoff id.
        /// </param>
        /// <returns>
        /// The <see cref="cStage"/>.
        /// </returns>
        public cStage this[int signoffId]
        {
            get
            {
                return this._stages[signoffId];
            }
        }

        /// <summary>
        /// Do the group stages contain a specific sign off ID.
        /// </summary>
        /// <param name="signOffId">
        /// The sign off ID to check.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool ContainsKey(int signOffId)
        {
            return this._stages.ContainsKey(signOffId);
        }

        /// <summary>
        /// Add a stage to the sign off group.
        /// </summary>
        /// <param name="signOffStage">
        /// The sign off stage.
        /// </param>
        /// <param name="stage">
        /// The stage.
        /// </param>
        public void Add(byte signOffStage, cStage stage)
        {
            this._stages.Add(signOffStage, stage);
        }

        /// <summary>
        /// Get the enumerator.
        /// </summary>
        /// <returns>
        /// The <see cref="IEnumerator"/>.
        /// </returns>
        public IEnumerator<cStage> GetEnumerator()
        {
            return this._stages.Values.GetEnumerator();
        }

        /// <summary>
        /// Get the enumerator.
        /// </summary>
        /// <returns>
        /// The <see cref="IEnumerator"/>.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}