namespace Match3.Core.GameDataExtraction
{
    public interface IExtractedData
    {
        IExtractedData GetClone();
        void Aggregate(IExtractedData other);
        void Clear();
    }
}