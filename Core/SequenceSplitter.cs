namespace Core;

public static class SequenceSplitter
{
	public static (List<string> output, HashSet<string> existingEndings) SplitSequenceGreedy(string sequence, int parts, int maxLengthDifference, HashSet<string>? existingEndings = null)
	{
		const int basesCount = 4;

		if (string.IsNullOrEmpty(sequence) || parts <= 0 || sequence.Length < parts * basesCount)
			throw new ArgumentException();

		List<int> boundaries = new() { 0 };

		int baseLength = sequence.Length / parts;
		int remainder = sequence.Length % parts;

		int position = 0;

		for (int i = 0; i < parts; i++)
		{
			position += baseLength + (i < remainder ? 1 : 0);
			boundaries.Add(position);
		}

		boundaries[^1] = sequence.Length;

		HashSet<string> endings = [.. existingEndings ?? []];

		for (int boundaryIndex = 1; boundaryIndex < boundaries.Count - 1; boundaryIndex++)
		{
			int ideal = boundaries[boundaryIndex];
			int previousBoundary = boundaries[boundaryIndex - 1];
			int nextBoundary = boundaries[boundaryIndex + 1];

			bool found = false;

			for (int distance = 0; distance <= maxLengthDifference && !found; distance++)
			{
				foreach (int candidate in distance == 0 ? new[] { ideal } : new[] { ideal - distance, ideal + distance })
				{
					if (candidate < previousBoundary + basesCount)
						continue;

					if (candidate > nextBoundary - basesCount)
						continue;

					string ending = sequence.Substring(candidate - basesCount, basesCount);

					if (endings.Contains(ending))
						continue;

					boundaries[boundaryIndex] = candidate;
					endings.Add(ending);
					found = true;
					break;
				}
			}

			if (!found)
				throw new InvalidOperationException(string.Format("Unable to find unique ending for part {0}.", boundaryIndex));
		}

		string lastEnding = sequence.Substring(sequence.Length - basesCount, basesCount);

		if (endings.Contains(lastEnding))
			throw new InvalidOperationException(string.Format("Unable to find unique ending for part {0}.", parts));

		endings.Add(lastEnding);

		List<string> result = new();

		for (int i = 0; i < parts; i++)
			result.Add(sequence.Substring(boundaries[i], boundaries[i + 1] - boundaries[i]));

		return (result, endings);
	}

	public static (List<string> output, HashSet<string> existingEndings) SplitSequenceGreedy_OffsetBy4(string sequence, int parts, int maxLengthDifference, HashSet<string>? existingEndings = null)
	{
		const int basesCount = 4;

		if (string.IsNullOrEmpty(sequence) || parts <= 0 || sequence.Length < parts * basesCount)
			throw new ArgumentException();

		List<int> boundaries = new() { 0 };

		int firstExtra = basesCount;
		int remainingLength = sequence.Length - firstExtra;
		int normalPartLength = remainingLength / parts;
		int remainder = remainingLength % parts;

		int position = 0;

		for (int i = 0; i < parts; i++)
		{
			int partLength = normalPartLength + (i < remainder ? 1 : 0);

			if (i == 0)
				partLength += firstExtra;

			position += partLength;
			boundaries.Add(position);
		}

		boundaries[^1] = sequence.Length;

		HashSet<string> endings = [.. existingEndings ?? []];

		for (int boundaryIndex = 1; boundaryIndex < boundaries.Count - 1; boundaryIndex++)
		{
			int ideal = boundaries[boundaryIndex];
			int previousBoundary = boundaries[boundaryIndex - 1];
			int nextBoundary = boundaries[boundaryIndex + 1];

			bool found = false;

			for (int distance = 0; distance <= maxLengthDifference && !found; distance++)
			{
				foreach (int candidate in distance == 0 ? new[] { ideal } : new[] { ideal - distance, ideal + distance })
				{
					if (candidate < previousBoundary + basesCount)
						continue;

					if (candidate > nextBoundary - basesCount)
						continue;

					string ending = sequence.Substring(candidate - basesCount, basesCount);

					if (endings.Contains(ending))
						continue;

					boundaries[boundaryIndex] = candidate;
					endings.Add(ending);
					found = true;
					break;
				}
			}

			if (!found)
				throw new InvalidOperationException(string.Format("Unable to find unique ending for part {0}.", boundaryIndex));
		}

		string lastEnding = sequence.Substring(sequence.Length - basesCount, basesCount);

		if (endings.Contains(lastEnding))
			throw new InvalidOperationException(string.Format("Unable to find unique ending for part {0}.", parts));

		endings.Add(lastEnding);

		List<string> result = new();

		for (int i = 0; i < parts; i++)
			result.Add(sequence.Substring(boundaries[i], boundaries[i + 1] - boundaries[i]));

		return (result, endings);
	}

	public static List<string> ManuallyAddBases(List<string> sequences)
	{
		string lastEnding = sequences[0].Substring(sequences[0].Length - 4, 4);
		for (int i = 1; i < sequences.Count; i++)
		{
			sequences[i] = string.Format("{0}{1}", lastEnding, sequences[i]);
			lastEnding = sequences[i].Substring(sequences[i].Length - 4, 4);
		}

		return sequences;
	}
}
