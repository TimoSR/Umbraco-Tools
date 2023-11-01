using Newtonsoft.Json.Linq;

namespace WorldDiabetesFoundation.Core.Infrastructure.UmbracoServices.Searchers.GeneralSearch.Setup
{
    public class GeneralSearcher
    {
        private readonly ProjectSearcher _projectSearcher;
        private readonly NewsSearcher _newsSearcher;
        private readonly StorySearcher _storySearcher;
        private readonly PatronSearchers _patronSearcher;
        private readonly DonationSearcher _donationSearcher;
        private readonly ApplicationSearcher _applicationSearcher;
        private readonly BoardSearcher _boardSearcher;
        private readonly ContentPageSearcher _contentPageSearcher;

        public GeneralSearcher(
            ProjectSearcher projectSearcher, 
            NewsSearcher newsSearcher, 
            StorySearcher storySearcher,
            PatronSearchers patronSearcher,
            DonationSearcher donationSearcher,
            ApplicationSearcher applicationSearcher,
            BoardSearcher boardSearcher,
            ContentPageSearcher contentPageSearcher)
        {
            _projectSearcher = projectSearcher;
            _newsSearcher = newsSearcher;
            _storySearcher = storySearcher;
            _patronSearcher = patronSearcher;
            _donationSearcher = donationSearcher;
            _applicationSearcher = applicationSearcher;
            _boardSearcher = boardSearcher;
            _contentPageSearcher = contentPageSearcher;
        }
    
        public string Search(Dictionary<string, string> filters, string generalSearch, int maxResultLength = 20)
        {
            var projectSearchResult = _projectSearcher.SearchProjects(filters, generalSearch);
            var newsSearchResult = _newsSearcher.SearchNews(filters, generalSearch);
            var storiesSearchResult = _storySearcher.SearchStories(filters, generalSearch);
            var patronSearchResult = _patronSearcher.Search(filters, generalSearch);
            var donationSearchResult = _donationSearcher.Search(filters, generalSearch);
            var applicationSearchResult = _applicationSearcher.Search(filters, generalSearch);
            var boardSearchResult = _boardSearcher.Search(filters, generalSearch);
            var contentPageSearchResult = _contentPageSearcher.Search(filters, generalSearch);

            var combinedJsonArray = new JArray
            {
                JArray.Parse(projectSearchResult),
                JArray.Parse(newsSearchResult),
                JArray.Parse(storiesSearchResult),
                JArray.Parse(patronSearchResult),
                JArray.Parse(donationSearchResult),
                JArray.Parse(applicationSearchResult),
                JArray.Parse(boardSearchResult),
                JArray.Parse(contentPageSearchResult)
            };

            return combinedJsonArray.ToString();
        }
    }
}
