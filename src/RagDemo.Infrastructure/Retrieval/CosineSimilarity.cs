namespace RagDemo.Infrastructure.Retrieval;

public static class CosineSimilarity
{
    public static double Calculate(
        float[] vector1,
        float[] vector2)
    {
        if (vector1.Length != vector2.Length)
        {
            throw new ArgumentException(
                "Vectors must have same dimensions.");
        }

        double dotProduct = 0;
        double magnitude1 = 0;
        double magnitude2 = 0;

        for (int i = 0; i < vector1.Length; i++)
        {
            dotProduct += vector1[i] * vector2[i];
            magnitude1 += vector1[i] * vector1[i];
            magnitude2 += vector2[i] * vector2[i];
        }

        if (magnitude1 == 0 || magnitude2 == 0)
        {
            return 0;
        }

        return dotProduct /
            (Math.Sqrt(magnitude1) *
             Math.Sqrt(magnitude2));
    }
}