using Newtonsoft.Json;
using System;

namespace WorldDiabetesFoundation.Core.ViewModel
{
    public class DashBoardViewModel
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("key")]
        public string Key { get; set; }

        [JsonProperty("createDate")]
        public DateTime CreateDate { get; set; }

        [JsonProperty("updateDate")]
        public DateTime UpdateDate { get; set; }

        [JsonProperty("published")]
        public bool Published { get; set; }

        [JsonProperty("publishDate")]
        public DateTime? PublishDate { get; set; }

        [JsonProperty("publishTemplateId")]
        public string PublishTemplateId { get; set; }

        [JsonProperty("publishName")]
        public string PublishName { get; set; }
    }
}