public interface IQueryClassifier
{
    QueryClassificationResult Classify(
        string question);
}