using Core.Extensions;

namespace Core;

public static class Generator
{
	private static readonly char[] bases = { 'A', 'C', 'G', 'T' };
	private static readonly Random random = new();

	public static List<string> GenerateAndAppendSequences(SequenceOptions options, string igSequences, List<string>? existingSequences = null)
	{
		List<string> generatedSequences;
		if(existingSequences != null)
			generatedSequences = new(existingSequences);
		else
		{
			existingSequences = [];
			generatedSequences = [];
		}

		string currentSequence;

		for (int i = 0; i < options.Count - existingSequences.Count;)
		{
			currentSequence = GenerateRandomSequence(options.Length);
			if (!IsValid(currentSequence, options, generatedSequences, igSequences))
				continue;

			generatedSequences.Add(currentSequence);
			i++;
		}

		return generatedSequences;
	}

	public static List<string> GenerateNewSequences(SequenceOptions options, string igSequences, List<string>? existingSequences = null)
	{
		List<string> generatedSequences = [];
		if (existingSequences == null)
			existingSequences = [];

		string currentSequence;

		for (int i = 0; i < options.Count;)
		{
			currentSequence = GenerateRandomSequence(options.Length);
			if (!IsValid(currentSequence, options, generatedSequences, igSequences, existingSequences))
				continue;

			generatedSequences.Add(currentSequence);
			i++;
		}

		return generatedSequences;
	}

	private static bool IsValid(string sequence, SequenceOptions options, List<string> generatedSequences, string igSequences, List<string>? existingSequences = null)
	{
		return MustStartEndWithGorC(sequence) &&
			MustSatisfyPercentage(sequence, options) &&
			MustNotComplementStartAndEnd(sequence, options) &&
			MustNotHaveReplicatesOf4Bases(sequence) &&
			MustNotRepeat(sequence, generatedSequences, igSequences, existingSequences);
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

	private static bool MustNotHaveReplicatesOf4Bases(string sequence)
	{
		if (sequence.Length < 4) return true;

		char[] chars = new char[4];

		for (int i = 0; i < sequence.Length - 3; i++)
		{
			chars[0] = sequence[i];
			chars[1] = sequence[i + 1];
			chars[2] = sequence[i + 2];
			chars[3] = sequence[i + 3];

			if (chars[0] == chars[1] && chars[0] == chars[2] && chars[0] == chars[3] &&
				chars[1] == chars[2] && chars[1] == chars[3] &&
				chars[2] == chars[3]) return false;
		}

		return true;
	}

	private static bool MustNotRepeat(string sequence, List<string> generatedSequences, string igSequences, List<string>? existingSequences)
	{
		foreach(string seq in generatedSequences)
			if (sequence.Equals(seq, StringComparison.OrdinalIgnoreCase)) return false;

		if(existingSequences != null)
			foreach (string seq in existingSequences)
				if (sequence.Equals(seq, StringComparison.OrdinalIgnoreCase)) return false;

		if (igSequences.Contains(sequence, StringComparison.OrdinalIgnoreCase)) return false;

		return true;
	}

	private static string GenerateRandomSequence(int length)
	{
		char[] buffer = new char[length];

		for (int i = 0; i < buffer.Length; i++)
			buffer[i] = bases[random.Next(4)];

		return new string(buffer);
	}
}
