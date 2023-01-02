int GetPlayMode(string helpText, int defaultValue)
{
    int value;
    Console.Write(helpText);
    string _string = Console.ReadLine();
    if (! int.TryParse(_string, out value))
    {
        value = _string == "" ? defaultValue : 0;
    }
    if (value < 1 || value > 3)
    {
        Console.WriteLine($"Incorrect input '{_string}', game mode will be set to '{defaultValue}'");
        value = defaultValue;
    }
    return value;
}

char[][] CreateGameField(int size = 3, string initState = "")
{
    char[][] Field = new char[size][];
    // Create empty field
    for (int i=0; i < size; i++)
    {
        Field[i] = new char[size];
        for (int j=0; j < size; j++)
        {
            Field[i][j] = ' ';
        }
    }
    if (initState != "")
    {
        // WARN, no initState string validation, assume that it is always correct
        for (int i=0; i< initState.Length; i++)
        {
            Field[i/size][i%size] = initState[i];
        }
    }
    return Field;
}

int FieldSize = 3;
int Marks2Win = FieldSize;

int PlayModeDefault = 1;
string PlayModeHelp = @$"Game mod can be
1) robot vs human (default)
2) human vs human
3) robot vs robot
Please input game mode number (default:{PlayModeDefault}): ";
int PlayMode = GetPlayMode(PlayModeHelp, PlayModeDefault);

Console.WriteLine($"Current game mode {PlayMode} | Current field size {FieldSize}");
// Console.WriteLine($"Current field size {FieldSize}");
Console.WriteLine($"You need to create a row of {Marks2Win} elements to win");

char[][] GameState = CreateGameField(FieldSize);

//Console.WriteLine($"Currect play state {GameState}");
for (int i = 0; i<GameState.Length;i++)
{
    for (int j =0; j<GameState[i].Length; j++)
    {
        Console.Write($"'{GameState[i][j]}' \t");
    }
    Console.Write("\n");
}
