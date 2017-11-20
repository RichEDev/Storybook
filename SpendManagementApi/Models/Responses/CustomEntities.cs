namespace SpendManagementApi.Models.Responses
{
    using System.Web.Http.Description;

    using Common;
    using Types;

    /// <summary>
    /// Response object for a single <see cref="CustomEntityForm">Custom Entity Form</see>
    /// </summary>
    [ApiExplorerSettings(IgnoreApi = true)]
    public class CustomEntityFormsResponse : ApiResponse<CustomEntityForm>
    {
    }

    /// <summary>
    /// Response object for a collection of <see cref="CustomEntityView">Custom Entity Views</see>
    /// </summary>
    [ApiExplorerSettings(IgnoreApi = true)]
    public class CustomEntityViewsResponse : GetApiResponse<CustomEntityView>
    {
    }

    /// <summary>
    /// Response object for a collection of <see cref="CustomEntityAttribute">Custom Entity Attributes</see>
    /// </summary>
    [ApiExplorerSettings(IgnoreApi = true)]
    public class CustomEntityAttributesResponse : GetApiResponse<CustomEntityAttribute>
    {
    }

    /// <summary>
    /// Response object for a single <see cref="CustomEntityRecord">Custom Entity Record</see>
    /// </summary>
    [ApiExplorerSettings(IgnoreApi = true)]
    public class CustomEntityRecordResponse : ApiResponse<CustomEntityRecord>
    {
    }

    /// <summary>
    /// Response object for a single <see cref="CustomEntityRecord">Custom Entity Record</see>
    /// </summary>
    [ApiExplorerSettings(IgnoreApi = true)]
    public class CustomEntityRecordsResponse : GetApiResponse<CustomEntityRecord>
    {
    }

    /// <summary>
    /// Response object for a single <see cref="Attachment">attachment</see>.
    /// </summary>
    [ApiExplorerSettings(IgnoreApi = true)]
    public class AttachmentResponse : ApiResponse<Attachment>
    {
    }
}