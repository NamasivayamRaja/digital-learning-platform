namespace DigitalLearningPlatform.Services.ContentService.Domain.Enums
{
    public enum FileProcessingStatus
    {
        Pending = 0,        // File upload is registered but not yet started
        Uploaded = 1,       // File has been uploaded to blob storage, awaiting processing
        Processing = 2,     // Background processing/validation/transcoding in progress
        Processed = 3,      // All processing complete, ready for association
        Failed = 4,         // Processing failed, see error message
        Associated = 5      // File is linked to a Section/SectionFile entry (finalized in course)
    }
}
