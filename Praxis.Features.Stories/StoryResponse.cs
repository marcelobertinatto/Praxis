namespace Praxis.Features.Stories
{
    public record StoryResponse(
    string Title,
    string Uri,
    string PostedBy,
    DateTime Time,
    int Score,
    int CommentCount
);
}
