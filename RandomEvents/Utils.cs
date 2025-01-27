using System;

namespace RandomEvents;

public static class Utils
{
	public static readonly Random Random = new Random();

	public static readonly bool[] BoolRandom = new bool[2] { true, false };

	public static bool PickBool(int trueChance)
	{
		return BoolRandom[PickIndex(100, 2, (int index) => (!BoolRandom[index]) ? (100 - trueChance) : trueChance)];
	}

	public static int PickIndex(int total, int size, Func<int, int> picker)
	{
		int num = Random.Next(0, total);
		int num2 = 0;
		for (int i = 0; i < size; i++)
		{
			int num3 = picker(i);
			for (int j = num2; j < num3 + num2; j++)
			{
				if (j >= num)
				{
					return i;
				}
			}
			num2 += num3;
		}
		return 0;
	}
}
