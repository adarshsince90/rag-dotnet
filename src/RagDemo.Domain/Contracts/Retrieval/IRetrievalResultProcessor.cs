using RagDemo.Domain.Models;

namespace RagDemo.Domain.Contracts;
public interface IRetrievalResultProcessor
{
    RetrievalProcessingResult Process(
        IEnumerable<RetrievalResult> results);
}