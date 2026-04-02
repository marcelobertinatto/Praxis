namespace Praxis.Features.Stories
{
    public interface IStoryService
    {
        Task<List<StoryResponse>> GetBestStories(int n);
    }
}
