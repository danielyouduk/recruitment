namespace JobManagement.Domain.ValueObjects;

public record JobDetails
{
    public const int MaxTitleLength = 200;
    public const int MaxDescriptionLength = 2000;
    
    public string Title { get; init; }
    public string? Description { get; init; }

    public JobDetails(string title, string? description = null)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Title cannot be null or empty", nameof(title));

        if (title.Length > MaxTitleLength)
            throw new ArgumentException($"Title cannot exceed {MaxTitleLength} characters", nameof(title));
            
        if (description?.Length > MaxDescriptionLength)
            throw new ArgumentException($"Description cannot exceed {MaxDescriptionLength} characters", nameof(description));

        Title = title.Trim();
        Description = description?.Trim();
    }
}