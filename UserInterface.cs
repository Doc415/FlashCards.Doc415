using FlashCards.Doc415.Models;
using Spectre.Console;
using static FlashCards.Doc415.Enums;
namespace FlashCards.Doc415;

internal class UserInterface
{
    internal DataAccess dataAccess = new();

    public void InitializeMenu()
    {
        while (true)
        {
            AnsiConsole.Write(new FigletText("Flashcards").Color(Color.DarkMagenta));
            var selection = AnsiConsole.Prompt(new SelectionPrompt<MainMenuSelections>()
                .Title("Welcome, what would you like to do?")
                .AddChoices(
                    MainMenuSelections.ManageFlashcards,
                    MainMenuSelections.ManageStacks,
                    MainMenuSelections.StudyArea,
                    MainMenuSelections.Quit
                ));

            switch (selection)
            {
                case MainMenuSelections.ManageFlashcards:
                    ManageFlashcards();
                    Console.Clear();
                    break;
                case MainMenuSelections.ManageStacks:
                    ManageStacks();
                    Console.Clear();
                    break;
                case MainMenuSelections.StudyArea:
                    StudyArea();
                    Console.Clear();
                    break;
                case MainMenuSelections.Quit:
                    AnsiConsole.Write(new FigletText("Goodby!").Color(Color.Green3));
                    Environment.Exit(0);
                    break;
            }
        }
    }

    void ManageFlashcards()
    {
        Console.Clear();
        while (true)
        {
            AnsiConsole.Write(new FigletText("Flashcards").Color(Color.DarkMagenta));
            var selection = AnsiConsole.Prompt(new SelectionPrompt<FlashcardSelections>()
                 .Title("[Green]----Manage Flashcards----[/]\n What would you like to do?")
                 .AddChoices(
                     FlashcardSelections.ViewFlashcards,
                     FlashcardSelections.AddFlashcard,
                     FlashcardSelections.DeleteFlashcard,
                     FlashcardSelections.UpdateFlashcard,
                     FlashcardSelections.ReturnToMainMenu
                 ));

            switch (selection)
            {
                case FlashcardSelections.ViewFlashcards:
                    ViewFlashcards();
                    Console.Clear();
                    break;
                case FlashcardSelections.AddFlashcard:
                    AddFlashcard();
                    Console.Clear();
                    break;
                case FlashcardSelections.DeleteFlashcard:
                    DeleteFlashcard();
                    Console.Clear();
                    break;
                case FlashcardSelections.UpdateFlashcard:
                    UpdateFlashcard();
                    Console.Clear();
                    break;
                case FlashcardSelections.ReturnToMainMenu:
                    Console.Clear();
                    InitializeMenu();
                    break;
            }
        }
    }


    void ManageStacks()
    {
        Console.Clear();
        while (true)
        {
            AnsiConsole.Write(new FigletText("Flashcards").Color(Color.DarkMagenta));
            var selection = AnsiConsole.Prompt(new SelectionPrompt<StackSelections>()
                 .Title("[Green]----Manage Stacks----[/]\n What would you like to do?")
                 .AddChoices(
                     StackSelections.ViewStacks,
                     StackSelections.AddStack,
                     StackSelections.DeleteStack,
                     StackSelections.UpdateStack,
                     StackSelections.ReturnToMainMenu
                 ));

            switch (selection)
            {
                case StackSelections.ViewStacks:
                    ViewStacks();
                    Console.Clear();
                    break;
                case StackSelections.AddStack:
                    AddStack();
                    Console.Clear();
                    break;
                case StackSelections.DeleteStack:
                    DeleteStack();
                    Console.Clear();
                    break;
                case StackSelections.UpdateStack:
                    UpdateStack();
                    Console.Clear();
                    break;
                case StackSelections.ReturnToMainMenu:
                    Console.Clear();
                    InitializeMenu();
                    break;
            }
        }
    }

    void StudyArea()
    {

    }

    //****Manage Flashcard methods*****

    void ViewFlashcards()
    {
        var stacks=dataAccess.GetAllStacks();  // I dont want to query the db for each flashcard to get the name of stack it belongs to
        var flashcards = dataAccess.GetAllFlashcards();
        string header=string.Format("[deeppink1_1]{0}[/][indianred_1]{1}{2}{3}[/]\n","Id".PadRight(10),"Question".PadRight(30),"Answer".PadRight(30),"Inside stack");
        AnsiConsole.Markup(header);
        foreach (var card in flashcards)
        {
            var stackName = stacks.Single(s => s.Id == card.StackId).Name;
            Console.WriteLine(string.Format("{0}{1}{2}#{3}{4}", 
                                                          card.Id.ToString().PadRight(10), 
                                                          card.Question.PadRight(30), 
                                                          card.Answer.PadRight(30), 
                                                          card.StackId.ToString().PadRight(6), 
                                                          stackName
                                                          ));
        }
        Console.WriteLine("Press Enter to continue...");
        Console.ReadLine();
    }

    void UpdateFlashcard() { }

    void AddFlashcard()
    {
        Flashcard flashcard = new();

        flashcard.StackId = ChooseStack();

        flashcard.Question = AnsiConsole.Ask<string>("Enter Question");
        while (string.IsNullOrEmpty(flashcard.Question))
        {
            flashcard.Question = AnsiConsole.Ask<string>("Question can't be empty. Try again");
        }

        flashcard.Answer = AnsiConsole.Ask<string>("Enter Answer");
        while (string.IsNullOrEmpty(flashcard.Answer))
        {
            flashcard.Answer = AnsiConsole.Ask<string>("Answer can't be empty. Try again");
        }

        dataAccess.AddFlashcard(flashcard);
    }

    int ChooseStack()
    {
        var stacks = dataAccess.GetAllStacks();
        var stackNames = stacks.Select(x => x.Name).ToArray();

        if (stackNames.Length < 1)
        {
            Console.Write("There is no Stack.");
        }

        var selection = AnsiConsole.Prompt(new SelectionPrompt<string>()
                                            .Title("Choose Stack")
                                            .AddChoices(stackNames));
        var stackId = stacks.Single(x => x.Name == selection).Id;
        return stackId;
    }

    void DeleteFlashcard() { }


    //*****Manage Stack methods*****

    void ViewStacks()
    {
        var stacks = dataAccess.GetAllStacks();
        AnsiConsole.Markup("[deeppink1_1]Id[/]     [indianred_1]Stack name[/]\n-----------------\n");
        foreach (var stack in stacks)
        {
            Console.WriteLine(string.Format("{1} {0}", stack.Name, stack.Id.ToString().PadRight(6)));
        }
        Console.WriteLine("Press Enter to continue...");
        Console.ReadLine();
    }

    void UpdateStack() { }

    void AddStack()
    {
        CardStack stack = new CardStack();
        stack.Name = AnsiConsole.Ask<string>("Enter Stack name:");

        while (string.IsNullOrEmpty(stack.Name))
        {
            stack.Name = AnsiConsole.Ask<string>("Stack name can't be empty. Try again:");
        }
        DataAccess dataAccess = new DataAccess();
        dataAccess.AddStack(stack);
    }
    void DeleteStack() { }

 
}

