using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpendManagementLibrary
{
    [Serializable]
    public class cDocumentMergeAssociation
    {
        public int DocMergeAssociationId  { get; set; }
        public int DocumentId { get; set; }
        public int EntityId {get; set;}
        public int RecordId { get; set; }
        public DateTime CreatedDate { get; set; }
        public int CreatedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public int? ModifiedBy { get; set; } 
    }
}
