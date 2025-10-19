namespace Nestelia.Domain.Common.ViewModels.Bot
{
    public class ChromaQueryResponseVM
    {
        public List<List<string>>? documents { get; set; }
        public List<List<float>>? distances { get; set; }
        public List<List<Dictionary<string, object>>>? metadatas { get; set; }
    }
}
