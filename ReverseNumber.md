using System;
using System.Collections.Generic;

class Program
{
    static void Main()
    {
        int num = 1234;
        List<int> digits = new List<int>();

        // Step 1: store digits (from last to first)
        while (num > 0)
        {
            digits.Add(num % 10);
            num /= 10;
        }

        // Step 2: convert digits list → number
        int rev = 0;
        int power = digits.Count - 1;

        foreach (int d in digits)
        {
            rev += d * (int)Math.Pow(10, power);
            power--;
        }

        Console.WriteLine(rev); // Output: 4321
    }
}

/*
#better approach as discussed in the meeting
using System;

class Program
{
    static void Main()
    {
        int num = 1234;
        int rev = 0;

        while (num > 0)
        {
            int digit = num % 10;     // get last digit
            rev = rev * 10 + digit;   // shift left and add last digit
            num /= 10;                // remove last digit
        }

        Console.WriteLine(rev); // 4321
    }
}

*/












