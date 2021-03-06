using System;
using System.Xml;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;

/*
Given a string x of digits, 0-9 of length 2 - 10 and an int y that is between -10^4 and 10^4
determine all possible solutions to get y, via inserting the operators
*, +, and/or - into x

e.g. 
string x = "123"
int y = 6

"1+2+3" and "1*2*3" are valid solutions
"1-2*3" is an invalid solution

*/
public class targetIntViaString
{
	public static void Entry()
	{
		string digits = "1052"; //0-9 , 2 ≤ digits.length ≤ 10
		int target = 5; // -10^4 ≤ target ≤ 10^4
		List<string> allTestStrings = getAllTestStrings(digits);
		List<string> validExpressions = parseTestStrings(allTestStrings, target);
		Console.WriteLine("Digits = {0}, Target = {1}, validExpressions.Count = {2}", digits, target, validExpressions.Count);
		foreach (string expression in validExpressions)
		{
			Console.WriteLine(expression);
		}
        Console.ReadLine();
	}

	//given a string "123" return a list of strings containing all possible ints, keeping order
	//e.g. "123","1 23", "12 3", "1 2 3"
	static List<string> getAllTestStrings(string testString)
	{
		List<string> allTestStrings = new List<string>();
		int spaceCount = 1;
		allTestStrings.Add(testString);
		while (spaceCount < testString.Length)
		{
			insertSpaces(testString, spaceCount, 1, ref allTestStrings);
			spaceCount++;
		}

		return allTestStrings;
	}

	static void insertSpaces(string testString, int spaceCount, int index, ref List<string> allTestStrings)
	{
		string workingString = testString;
		for (int i = index; i < testString.Length; i++)
		{
			workingString = testString.Insert(i, " ");
			//insert additional spaces if neccesary
			if (spaceCount > 1)
			{
				spaceCount--;
				//using the newly transformed string, iterate again, passing over the newly added space
				insertSpaces(workingString, spaceCount, i + 2, ref allTestStrings);
			}

			if (!allTestStrings.Contains(workingString))
			{
				allTestStrings.Add(workingString);
			}
		}
	}

	static List<string> parseTestStrings(List<string> allTestStrings, int target)
	{
		List<string> validExpressions = new List<string>();
		//foreach test string
		foreach (string testString in allTestStrings)
		{
			//split string into int segments
			string[] segments = testString.Split(' ');
			//initialize array to hold ints in testString
			double[] doubles = new double[segments.Length];
			//get int[] from testString segments
			for (int i = 0; i < segments.Length; i++)
			{
				doubles[i] = double.Parse(segments[i]);
			}

			//generate each test operation
			string[] expressions = generateExpressions(doubles);
			//evaluate each test operation
			foreach (string expression in expressions)
			{
				//if test operation resolves to the target, 
				if (ValidateExpression(expression, target))
				{
					//add operation to final list
					validExpressions.Add(expression);
				}
			}
		}

		return validExpressions;
	}

	static string[] generateExpressions(double[] doubles)
	{
		List<string> expressions = new List<string>();
		string[] possibleOperators = getPossibleOperators(doubles.Length - 1);
		foreach (string operatorSet in possibleOperators)
		{
			string expression = "";
			//create string alternating between ints[i] and operatorSet[i]
			//e.g. for ints == {1,2} and operatorSet == {+}, expression should resolve to "1+2"
			for (int i = 0; i < doubles.Length; i++)
			{
				expression += doubles[i];
				if (i < operatorSet.Length)
				{
					expression += operatorSet[i];
				}
			}

			expressions.Add(expression);
		}

		string[] allOperations = expressions.ToArray();
		return allOperations;
	}

	//using operatorCount, create all permutations of operators 
	//e.g. for operatorCount == 1, string[] possibleOperators = new string[]{+, -, *}
	//	   for operatorCount == 2, string[] possibleOperators = new string[]{++, +-, +*,--, -+, -*, **, *-,*+}	 
	static string[] getPossibleOperators(int operatorCount)
	{
		//get upper limit
		int maxExpressionCount = (int)Math.Pow(3, operatorCount);
		List<string> operatorSet = new List<string>();
		List<int> operatorsAsInts = new List<int>();
		char[] staticOperators = {'+', '-', '*'};
		//generate operator order
		//given operatorCount x, create int such that int.ToString().Length == x, with each digit == 1
		//e.g. operatorCount == 4, int y == 1111;
		int currentOperator = 1;
		for (int i = currentOperator.ToString().Length; i < operatorCount; i++)
		{
			currentOperator = int.Parse(currentOperator.ToString() + "1");
		}

		if (operatorCount > 0)
		{
			operatorsAsInts.Add(currentOperator);
			//int to array
			int[] workingInts = Array.ConvertAll(operatorsAsInts[0].ToString().ToArray(), x => (int)x - 48);
			//we can then get all permutations of an int with length x, using digits 1,2,3 
			//we will later translate 1 == '+', 2 == '-', 3 == '*'	
			getOperatorPermutations(maxExpressionCount, ref workingInts, ref operatorsAsInts);
		}

		foreach (int temp in operatorsAsInts)
		{
			string trans = temp.ToString().Replace('1', '+').Replace('2', '-').Replace('3', '*');
			operatorSet.Add(trans);
		}

		string[] possibleOperators = operatorSet.ToArray();
		return possibleOperators;
	}

	static bool ValidateExpression(string expression, int target)
	{
		bool isValid = false;
		//DataTable dt = new DataTable();
		//int testValue = (int)dt.Compute(expression, "");
		double testValue = Evaluate(expression);
		if (testValue == target)
		{
			isValid = true;
		}

		return isValid;
	}

	static int createOperatorSet(int[] workingInts)
	{
		int operatorSet = workingInts[0];
       /* int counter = 0;
        foreach (int a in workingInts)
        {
            Console.WriteLine("workingInts[{0}] == {1}", counter, a);
            counter++;
        }*/
        //append each entry in workingInts to current operator to create the full operatorSet
        Console.WriteLine("Attempting to parse operatorSet. Currently, operatorSet == {0}", operatorSet);

        for (int i = 1; i < workingInts.Length; i++)
		{
                operatorSet = int.Parse(operatorSet.ToString() + workingInts[i].ToString());
		}
        Console.WriteLine("Returning operatorSet == {0}", operatorSet);

        return operatorSet;
	}

	static void getOperatorPermutations(int maxExpressionCount, ref int[] workingInts, ref List<int> operatorsAsInts)
	{
		int operatorSet;
        Console.WriteLine("\nmaxExpressionCount == {0}, workingInts.Length = {1}, operatorsAsInts.Count == {2}",maxExpressionCount, workingInts.Length, operatorsAsInts.Count);
        Console.WriteLine("createOperatorSet from workingInts");
		operatorSet = createOperatorSet(workingInts);
		//add operatorSet to list
		if (!operatorsAsInts.Contains(operatorSet))
		{
			operatorsAsInts.Add(operatorSet);
		}

		//increase last digit by 1		
		workingInts[workingInts.Length - 1]++;
		//if last digit is out of scope, wrap it to 1.
		if (workingInts[workingInts.Length - 1] > 3)
		{
			workingInts[workingInts.Length - 1] = 1;
		}

        //update operatorSet
        Console.WriteLine("hit");
		operatorSet = createOperatorSet(workingInts);
		//add operatorSet to list
		if (!operatorsAsInts.Contains(operatorSet))
		{
			operatorsAsInts.Add(operatorSet);
		}

		if (workingInts.Length > 1)
		{
			//shift ints left
			int holder = workingInts[0];
			for (int i = 0; i < workingInts.Length - 1; i++)
			{
				workingInts[i] = workingInts[i + 1];
			}

			//add last digit to operator
			workingInts[workingInts.Length - 1] = holder;
			//update operatorSet
			operatorSet = createOperatorSet(workingInts);
			//add operator to list
			if (!operatorsAsInts.Contains(operatorSet))
			{
				operatorsAsInts.Add(operatorSet);
			}
		}

		//find more if they exist
		if (operatorsAsInts.Count < maxExpressionCount)
		{
			getOperatorPermutations(maxExpressionCount, ref workingInts, ref operatorsAsInts);
		}
	}

	static double Evaluate(string expression)
	{
		double result = 0;
		int operatorIndex = 0;
		string[] operators = new string[]{"*", "/", "+", "-"};
		string op1 = operators[operatorIndex];
		operatorIndex++;
		string op2 = operators[operatorIndex];
		List<string> expParams = new List<string>(Regex.Split(expression, @"([()*+\/-])", RegexOptions.Multiline));
		while (expParams.Count > 1)
		{
			//if expression does not contain either operator, try the next pair
			if (!expParams.Contains(op1) & !expParams.Contains(op2))
			{
				operatorIndex++;
				op1 = operators[operatorIndex];
				operatorIndex++;
				op2 = operators[operatorIndex];
			}

			//if either operator is present, continue
			if (expParams.Contains(op1) | expParams.Contains(op2))
			{
				//find first instance of valid operator
				for (int i = 0; i < expParams.Count; i++)
				{
					//when an operator is found, select ints on either side and process
					if (expParams[i] == op1 || expParams[i] == op2)
					{
						string op = expParams[i];
						double a = 0, b = 0;
						//get int prior to and after operator		
						try
						{
							a = Convert.ToDouble(expParams[i - 1]);
						}
						catch (Exception e)
						{
							Console.WriteLine("Failed to convert.ToDouble({0})",expParams[i-1]);
							throw;
						}

						try
						{							
							b = Convert.ToDouble(expParams[i + 1]);
						}
						catch (Exception e)
						{
							Console.WriteLine("Failed to convert.ToDouble({0})",expParams[i+1]);
							throw;
						}

						double c = 0;
						switch (op)
						{
							case "*":
							{
								c = (a * b);
								break;
							}

							case "/":
							{
								c = (a / b);
								break;
							}

							case "+":
							{
								c = (a + b);
								break;
							}

							case "-":
							{
								c = (a - b);
								break;
							}

							default:
							{
								continue;
							}
						}

						replaceOperationWithResult(i, c.ToString(), ref expParams);
						//set i to index of insertion
						i--;
					}
				}
			}
		}

		result = Convert.ToDouble(expParams[0]);
		return result;
	}

	//replace operator, previous and nexts ints with result of operation
	static void replaceOperationWithResult(int index, string result, ref List<string> expParams)
	{
		for (int x = 0; x < 3; x++)
		{
			expParams.RemoveAt(index - 1);
		}

		expParams.Insert(index - 1, result);
	}
}
