namespace RagDemo.Domain.Interfaces;

public interface IRetrievalResultProcessor
{
    RetrievalProcessingResult Process(
        IEnumerable<RetrievalResult> results);
}