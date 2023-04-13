using System;
using System.Collections.Generic;
using System.IO;

class QuizProgram
{
    private const int SCORE = 20;

    static void Main(string[] args)
    {
        Console.WriteLine("Welcome to the Quiz Program!");

        //creating an empty list of question objects
        List<Question> questions = new List<Question>();

        // ask user for questions and answers
        bool isAddingQuestions = true;
        while (isAddingQuestions)
        {
            Console.WriteLine("Enter a question (or 'quit' to finish):");
            string questionText = Console.ReadLine().Trim();

            if (Quit(questionText))
            {
                break;
            }

            Console.WriteLine("Enter the multiple choice answers separated by commas:");
            string[] answers = Console.ReadLine().Split(',');

            Console.WriteLine("Enter the index(es) of the correct answer(s) separated by commas:");
            string[] correctAnswers = Console.ReadLine().Split(',');

            Question question = new Question(questionText, answers, correctAnswers);
            questions.Add(question);
        }

        // write questions to file
        using (StreamWriter writer = new StreamWriter("questions.txt"))
        {
            foreach (Question question in questions)
            {
                writer.WriteLine(question.ToString());
            }
        }

        // read questions from file
        List<Question> questionsFromFile = new List<Question>();

        using (StreamReader reader = new StreamReader("questions.txt"))
        {
            while (!reader.EndOfStream)
            {
                string line = reader.ReadLine();
                string[] parts = line.Split('|');

                string questionText = parts[0].Trim();
                string[] answers = parts[1].Split(',');
                string[] correctAnswers = parts[2].Split(',');

                Question question = new Question(questionText, answers, correctAnswers);
                questionsFromFile.Add(question);
            }
        }

        // play quiz
        int score = 0;
        Console.WriteLine("Let's play the quiz!");

        while (questionsFromFile.Count > 0)
        {
            if (questionsFromFile.Count == 0)
            {
                Console.WriteLine("There are no more questions.");
                break;
            }

            // Generate a random index for the next question
            int questionIndex = new Random().Next(questionsFromFile.Count);
            Question question = questionsFromFile[questionIndex];

            // Remove the question from the list to avoid asking the same question again
            questionsFromFile.RemoveAt(questionIndex);

            Console.WriteLine(question.Text);

            for (int i = 0; i < question.Answers.Length; i++)
            {
                Console.WriteLine($"{i + 1}. {question.Answers[i]}");
            }

            Console.Write("Enter your answer(s) separated by commas: ");
            string answerInput = Console.ReadLine().Trim();

            if (Quit(answerInput))
            {
                break;
            }

            string[] userAnswers = answerInput.Split(',');

            bool isCorrect = true;

            foreach (string userAnswer in userAnswers)
            {
                int index = int.Parse(userAnswer.Trim()) - 1;

                if (!question.IsCorrect(index))
                {
                    isCorrect = false;
                    break;
                }
            }

            if (isCorrect)
            {
                Console.WriteLine("Correct!");
                score += SCORE;
            }
            else
            {
                Console.WriteLine("Incorrect.");
            }

            Console.WriteLine($"Your score is {score}.");

            Console.WriteLine("Press enter to continue to the next question or type 'quit' to end the quiz.");
            string continueInput = Console.ReadLine().Trim();

            if (Quit(continueInput))
            {
                break;
            }

            Console.WriteLine();
        }

        Console.WriteLine($"Your score is {score}.");

        Console.WriteLine("Thanks for playing the Quiz Program!");
    }

    static bool Quit(string input)
    {
        return input.ToLower() == "quit";
    }
}

class Question
{
    public string Text { get; }
    public string[] Answers { get; }
    private bool[] correctAnswers;

    public Question(string text, string[] answers, string[] correctAnswerIndices)
    {
        Text = text;
        Answers = answers;
        correctAnswers = new bool[answers.Length];

        foreach (string index in correctAnswerIndices)
        {
            int i = int.Parse(index.Trim()) - 1;
            correctAnswers[i] = true;
        }
    }

    public bool IsCorrect(int index)
    {
        return correctAnswers[index];
    }

    public override string ToString()
    {
        string answerString = string.Join(",", Answers);
        string correctAnswerString = string.Join(",", GetCorrectAnswerIndices());

        return $"{Text} | {answerString} | {correctAnswerString}";
    }

    private List<int> GetCorrectAnswerIndices()
    {
        List<int> indices = new List<int>();

        for (int i = 0; i < correctAnswers.Length; i++)
        {
            if (correctAnswers[i])
            {
                indices.Add(i + 1);
            }
        }

        return indices;
    }
}

