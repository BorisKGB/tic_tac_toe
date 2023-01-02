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
    if (value != 2)
    {
        Console.WriteLine("This version implement only 'human vs human' game mode, enable it for you");
        value = 2;
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

void PrintGameField(char[][] Field, string headerNames)
{
    string header = "   |";
    for (int i=0;i<Field.Length;i++)
    {
        if (i == Field.Length -1)
        {
            header += $" {headerNames[i]}";
        }
        else
        {
            header += $" {headerNames[i]} |";
        }
    }
    string table = "";
    for (int i = 0; i<Field.Length;i++)
    {
        table += $" {i+1} |";
        for (int j =0; j<Field[i].Length; j++)
        {
            if (j == Field[i].Length-1)
            {
                table += $" {Field[i][j]}";
            }
            else
            {
                table += $" {Field[i][j]} |";
            }
        }
        table += "\n";
    }
    Console.WriteLine(header);
    Console.WriteLine(table);
}

void AddMark(char[][] GameField, char Mark, int positionX, int positionY)
{
    GameField[positionX][positionY] = Mark;
}

bool AbleToAddMark(char[][] GameField, int[] result)
{
    if (result[0] < 0 || result[1] < 0)
    {
        return false;
    }
    else if (result[0] > GameField.Length || result[1] > GameField.Length)
    {
        return false;
    }
    else if (GameField[result[0]][result[1]] != ' ')
    {
        return false;
    }
    return true;
}

int[] AskPlayerForCoordinates(char[][] GameField, string columnNames, string PlayerName, char PlayerMark)
{
    int [] result = new int[2] {-1, -1};
    string input;
    while (result[0] < 0 || result[1] < 0)
    {
        Console.Write($"{PlayerName} turn, input coordinates to place '{PlayerMark}'> ");
        input = Console.ReadLine();
        result[1] = columnNames.IndexOf(input[0]);
        int.TryParse(input.Substring(1), out result[0]);
        // decrease resultX by one cause we counting from 0, but displaing from 1
        result[0] = result[0] - 1;
        if (!AbleToAddMark(GameField, result))
        {
            Console.WriteLine($"Coordinates {input} incorrect, try again");
            result[0] = -1;
        }
    }
    return result;

}

// This limit possible Game field size to 26, or to fieldHeaderNames.Length
string fieldHeaderNames = "abcdefghijklmnopqrstuvwxyz";

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
Console.WriteLine($"You need to create a row of {Marks2Win} elements to win");

char[][] GameState = CreateGameField(FieldSize);

PrintGameField(GameState, fieldHeaderNames);

string[] players = new String[2] {"player1", "player2"};
string playerMarks = "xo";
int currentPlayer = 0;
bool gameEnd = false;
// game result:
// -2 default value, will be like that untill game ends
// -1 draw
// 0/1 number of player who win
int gameResult = -2;
int[] moveCoordinates;
while (!gameEnd)
{
    // determine who moves (human or robot) to get mark coordinates
    if (players[currentPlayer] == "player1" || players[currentPlayer] == "player2")
    {
        moveCoordinates = AskPlayerForCoordinates(GameState, fieldHeaderNames, players[currentPlayer], playerMarks[currentPlayer]);
    }
    else
    {
        // currently not used. no robot players implement for now
        moveCoordinates = new int[2] {0, 0};
    }
    AddMark(GameState, playerMarks[currentPlayer], moveCoordinates[0], moveCoordinates[1]);

    // for now game newer end
    //gameResult = CheckGameStatus(GameState, playerMarks);
    if (gameResult >= -1 && gameResult <= 1)
    {
        gameEnd = true;
    }
    PrintGameField(GameState, fieldHeaderNames);
    currentPlayer = (currentPlayer + 1)%2;
}
