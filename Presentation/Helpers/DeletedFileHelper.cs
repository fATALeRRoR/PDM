using PDM.Client.Dto;

namespace PDM.Client.Helpers
{
    public static class DeletedFileHelper
    {
        private const string Keyword = "#deleted";

        public static void AddDeletedKeyword(Document document)
        {
            if (!document.Keywords.Contains(Keyword))
            {
                document.Keywords.Add(Keyword);
            }
        }

        public static void RemoveDeletedKeyword(Document document)
        {
            while (document.Keywords.Contains(Keyword))
            {
                document.Keywords.Remove(Keyword);
            }
        }
    }
}
