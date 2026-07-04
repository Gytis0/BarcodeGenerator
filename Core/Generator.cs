using Core.Extensions;

namespace Core;

public static class Generator
{
	private static readonly char[] bases = { 'A', 'C', 'G', 'T' };
	private static readonly Random random = new();

	public static string[] GenerateSequences(SequenceOptions options)
	{
		string[] result = new string[options.Count];
		string currentSequence;

		for (int i = 0; i < options.Count;)
		{
			currentSequence = GenerateRandomSequence(options.Length);
			if (!IsValid(currentSequence, options))
				continue;

			result[i] = currentSequence;
			i++;
		}

		return result;
	}

	private static bool IsValid(string sequence, SequenceOptions options)
	{
		return MustStartEndWithGorC(sequence) &&
			MustSatisfyPercentage(sequence, options) &&
			MustNotComplementStartAndEnd(sequence, options);
	}

	private static bool MustStartEndWithGorC(string sequence)
	{
		return (sequence[0] == 'G' || sequence[0] == 'C') &&
			(sequence[sequence.Length - 1] == 'G' || sequence[sequence.Length - 1] == 'C');
	}

	private static bool MustSatisfyPercentage(string sequence, SequenceOptions options)
	{
		int gcCount = 0;

		for (int i = 0; i < options.Length; i++)
			if (sequence[i] == 'G' || sequence[i] == 'C') gcCount++;

		return gcCount >= options.Length * (options.Percentage / 100f);
	}

	private static bool MustNotComplementStartAndEnd(string sequence, SequenceOptions options)
	{
		string start = sequence.Substring(0, options.AntiComplementaryLength);
		string end = sequence.Substring(sequence.Length - options.AntiComplementaryLength, options.AntiComplementaryLength);

		string startComplemented = start.Complement();
		return !startComplemented.Equals(end);
	}

	private static string GenerateRandomSequence(int length)
	{
		char[] buffer = new char[length];

		for (int i = 0; i < buffer.Length; i++)
			buffer[i] = bases[random.Next(4)];

		return new string(buffer);
	}
}
