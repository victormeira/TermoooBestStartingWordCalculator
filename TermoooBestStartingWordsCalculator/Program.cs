using System;
using WeCantSpell;
using WeCantSpell.Hunspell;

namespace TermoooBestStartingWordsCalculator
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Dictionary<string, double> rankingOfScorePerFiveLetterWord = new Dictionary<string, double>();

            List<string> fiveLetterWords = GenerateFiveLetterWordsDictionary();
            List<string> termoooWords = File.ReadAllLines("./AuxiliaryFiles/palavrasCertas.txt").ToList();

            foreach(string word in fiveLetterWords)
            {
                rankingOfScorePerFiveLetterWord.Add(word, CalculateAverageTermoooScoreForWord(word, termoooWords));
            }

            Console.WriteLine("Best start words:");
            foreach(var bestWordAndScore in rankingOfScorePerFiveLetterWord.OrderByDescending(i => i.Value).Take(20))
            {
                Console.WriteLine(bestWordAndScore.Key + " - " + bestWordAndScore.Value.ToString("#.##"));
            }
        }


        static string GenerateNextWord(string currentWord)
        {
            string nextWord = "";
            char[] reversedWord = currentWord.Reverse().ToArray();

            bool shouldCarryOne = true;
            for(int i = 0; i < currentWord.Length; i++)
            {
                char currentChar = reversedWord[i];

                if (shouldCarryOne)
                {
                    if(currentChar == 'z')
                    {
                        currentChar = 'a';
                        shouldCarryOne = true;
                    }
                    else
                    {
                        currentChar = (char) (currentChar + 1);
                        shouldCarryOne = false;
                    }
                }

                nextWord = currentChar + nextWord;
            }

            return nextWord;
        }

        static double CalculateAverageTermoooScoreForWord(string guessWord, List<string> listOfFiveLetterWords)
        {
            double totalScore = 0;
            foreach(string possibleFiveLetterWord in listOfFiveLetterWords)
            {
                totalScore += CalculateTermooScoreForWord(guessWord, possibleFiveLetterWord);
            }

            return totalScore / listOfFiveLetterWords.Count;
        }

        static double CalculateTermooScoreForWord(string guessWord, string correctWord)
        {
            double totalScore = 0;
            for(int i = 0; i < guessWord.Length; i++)
            {
                if(guessWord[i] == correctWord[i])
                {
                    totalScore += 1;
                }
                else if(correctWord.Contains(guessWord[i]))
                {
                    totalScore += 0.2;
                }
            }

            return totalScore;
        }

        static List<string> GenerateFiveLetterWordsDictionary()
        {
            if (File.Exists("./AuxiliaryFiles/PalavrasDe5Letras.txt"))
            {
                Console.WriteLine("Loaded five letter words from file.");
                return File.ReadAllLines("./AuxiliaryFiles/PalavrasDe5Letras.txt").ToList();
            }

            StreamWriter sw = new StreamWriter("./AuxiliaryFiles/PalavrasDe5Letras.txt");

            var dictionary = WordList.CreateFromFiles(@"./AuxiliaryFiles/pt_BR.dic");
            List<string> fiveLetterWords = new List<string>();

            int totalProcessed = 0;
            long totalPossibleWords = 26 * 26 * 26 * 26 * 26;
            string currentWord = "aaaaa";
            while (!currentWord.Equals("zzzzz"))
            {
                if (++totalProcessed % 10000 == 0)
                {
                    Console.WriteLine("Total Processed: " + (totalProcessed * 100.0 / totalPossibleWords).ToString("#.##") + "% and found a total of " + fiveLetterWords.Count + " words.");
                }

                if (dictionary.Check(currentWord))
                {
                    fiveLetterWords.Add(currentWord);
                    sw.WriteLine(currentWord);
                }

                currentWord = GenerateNextWord(currentWord);
            }
            sw.Close();
            return fiveLetterWords;
        }
    }
}