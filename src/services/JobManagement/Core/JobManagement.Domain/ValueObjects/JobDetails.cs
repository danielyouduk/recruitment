namespace JobManagement.Domain.ValueObjects;

public record JobDetails
{
    public string Title { get; init; }
    public string? Description { get; init; }

    public JobDetails(string title, string? description = null)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Title cannot be null or empty", nameof(title));

        Title = title;
        Description = description;
    }
}