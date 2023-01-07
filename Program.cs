int GetPlayMode(string helpText, int defaultValue)
{
    int value;
    Console.Write(helpText);
    string _string = Console.ReadLine();
    if (! int.TryParse(_string, out value))
    {
        value = _string == "" ? defaultValue : 0;
    }
    if (value < 1 || value > 4)
    {
        Console.WriteLine($"Incorrect input '{_string}', game mode will be set to '{defaultValue}'");
        value = defaultValue;
    }
    // if (value != 3)
    // {
    //     Console.WriteLine("This version implement only 'human vs human' game mode, enable it for you");
    //     value = 3;
    // }
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
    else if (result[0] > GameField.Length-1 || result[1] > GameField.Length-1)
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
        input = Console.ReadLine().ToLower();
        if (input.Length < 2)
        {
            input = "  ";
        }
        result[1] = columnNames.IndexOf(input[0]);
        int.TryParse(input.Substring(1), out result[0]);
        // decrease resultX by one cause we counting from 0, but displaing from 1
        result[0] = result[0] - 1;
        if (!AbleToAddMark(GameField, result))
        {
            Console.WriteLine($"Coordinates '{input}' incorrect, try again. Example: 'a1'");
            result[0] = -1;
        }
    }
    return result;
}

void markEmptyPoints(char[][] Field, int[][] CounterField, int[][] rowPoints)
{
    int pointX, pointY;
    for (int point=0; point<rowPoints.Length;point++)
        {
            pointX = rowPoints[point][0];
            pointY = rowPoints[point][1];
            if (Field[pointX][pointY] == ' ')
            {
                CounterField[pointX][pointY]++;
            }
        }
}

int[] robotMove(char[][] Field, string columnNames, string playerMarks, string[] players, int currentPlayer, int Marks2Win)
{
    int[] result = new int[2] {-1, -1};
    // 1. search if we in one step from victory -> do it
    // 2. search if other player has almost complete rows -> try to prevent opponent victory
    // 3. search for point which can create most number of rows

    // count number of available rows, for each point
    // so we can atleast pick point which can create maximum lines
    int[][] RowsPerPoint = new int[Field.Length][];
    for (int i=0;i<Field[0].Length;i++)
    {
        RowsPerPoint[i] = new int[Field[0].Length];
    }
    
    int tmpLineStatus;
    int[][] rowPoints;
    int[] rowInfo;
    for (int row=0; row<Field.Length; row++)
    {
        // check first axis row

        // Always check horizontal lines
        rowPoints = GetRowPoints(new int[] {row, 0}, 1, Marks2Win);
        tmpLineStatus = CheckMarksInRow(Field, rowPoints);
        if (tmpLineStatus == 1)
        {
            rowInfo = getRowInfo(Field, rowPoints, playerMarks);
            // if we one step from win, do it
            // also if we one step from loose
            if (rowInfo[1] == 1)
            {
                return new int[2] {rowInfo[2], rowInfo[3]};
            }
            // else if we can continue this row, mark its points
            else if (rowInfo[0] == currentPlayer)
            {
                markEmptyPoints(Field, RowsPerPoint, rowPoints);
            }
        }
        if (tmpLineStatus == 0)
        {
            markEmptyPoints(Field, RowsPerPoint, rowPoints);
        }
        // If it make sense check oblique lines
        if (row == 0)
        {
            rowPoints = GetRowPoints(new int[] {row, 0}, 2, Marks2Win);
            tmpLineStatus = CheckMarksInRow(Field, rowPoints);
            if (tmpLineStatus == 1)
            {
                rowInfo = getRowInfo(Field, rowPoints, playerMarks);
                // if we one step from win, do it
                // also if we one step from loose
                if (rowInfo[1] == 1)
                {
                    return new int[2] {rowInfo[2], rowInfo[3]};
                }
                // else if we can continue this row, mark its points
                else if (rowInfo[0] == currentPlayer)
                {
                    markEmptyPoints(Field, RowsPerPoint, rowPoints);
                }
            }
            if (tmpLineStatus == 0)
            {
                markEmptyPoints(Field, RowsPerPoint, rowPoints);
            }
        }
        if (row == Field.Length-1)
        {
            rowPoints = GetRowPoints(new int[] {row, 0}, 3, Marks2Win);
            tmpLineStatus = CheckMarksInRow(Field, rowPoints);
            if (tmpLineStatus == 1)
            {
                rowInfo = getRowInfo(Field, rowPoints, playerMarks);
                // if we one step from win, do it
                // also if we one step from loose
                if (rowInfo[1] == 1)
                {
                    return new int[2] {rowInfo[2], rowInfo[3]};
                }
                // else if we can continue this row, mark its points
                else if (rowInfo[0] == currentPlayer)
                {
                    markEmptyPoints(Field, RowsPerPoint, rowPoints);
                }
            }
            if (tmpLineStatus == 0)
            {
                markEmptyPoints(Field, RowsPerPoint, rowPoints);
            }
        }
        // check second axis row

        // Always check vertical lines
        rowPoints = GetRowPoints(new int[] {0, row}, 0, Marks2Win);
        tmpLineStatus = CheckMarksInRow(Field, rowPoints);
        if (tmpLineStatus == 1)
        {
            rowInfo = getRowInfo(Field, rowPoints, playerMarks);
            // if we one step from win, do it
            // also if we one step from loose
            if (rowInfo[1] == 1)
            {
                return new int[2] {rowInfo[2], rowInfo[3]};
            }
            // else if we can continue this row, mark its points
            else if (rowInfo[0] == currentPlayer)
            {
                markEmptyPoints(Field, RowsPerPoint, rowPoints);
            }
        }
        if (tmpLineStatus == 0)
        {
            markEmptyPoints(Field, RowsPerPoint, rowPoints);
        }
        // do not need to check oblique lines in this part
    }
    // if we get to this point, then we check all field and did not find any easy solution
    // but mark each field with possible row counter in {RowsPerPoint} variable
    // so we just need to go through all field again and find point with max number
    int maxX=0, maxY=0;
    for (int x=0;x<RowsPerPoint.Length;x++)
    {
        for (int y=0;y<RowsPerPoint[0].Length;y++)
            if (RowsPerPoint[x][y]>RowsPerPoint[maxX][maxY])
            {
                maxX = x;
                maxY = y;
            }
    }
    result[0] = maxX;
    result[1] = maxY;
    return result;
}

int[][] GetRowPoints(int[] firstPoint, int direction, int rowLength)
// possible directions
//  0 Going through the first axis
//  1 Going through the second axis
//  2 Going obliquely up
//  3 Going obliquely down
{
    int[][] points = new int[rowLength][];
    switch (direction)
    {
        case 0:
            for (int i=0;i<rowLength;i++)
            {
                points[i] = new int[] {firstPoint[0]+i, firstPoint[1]};
            }
            break;
        case 1:
            for (int i=0;i<rowLength;i++)
            {
                points[i] = new int[] {firstPoint[0], firstPoint[1]+i};
            }
            break;
        case 2:
            for (int i=0;i<rowLength;i++)
            {
                points[i] = new int[] {firstPoint[0]+i, firstPoint[1]+i};
            }
            break;
        case 3:
            for (int i=0;i<rowLength;i++)
            {
                points[i] = new int[] {firstPoint[0]-i, firstPoint[1]+i};
            }
            break;
    }
    return points;
}


int[] getRowInfo(char[][] Field, int[][] rowPoints, string playerMarks)
//char getMarkCharFromRow(char[][] Field, int[][] rowPoints)
//return int[] of
// 1. number of last playerMark from row
// 2. number of empty fields
// 3. coordinateX of last empty field
// 4. coordinateY of last empty field
{
    int[] result = new int[4] {0, 0, 0, 0};
    int markNum = 0;
    char cell;
    for (int point=0; point<rowPoints.Length;point++)
    {
        cell = Field[rowPoints[point][0]][rowPoints[point][1]];
        markNum = playerMarks.IndexOf(cell);
        if (cell != ' ' && markNum >= 0)
        {
            result[0] = markNum;
        }
        else
        {
            result[1]++;
            result[2] = rowPoints[point][0];
            result[3] = rowPoints[point][1];
        }
    }
    return result;
}

int CheckMarksInRow(char[][] Field, int[][] points)
// Check if {rowLength} cells in {Field} in {direction} from(including) {pointCoordinates} are equal
// return
// 2 if all fields are equal (win strike)
// 1 if all fields are equal or empty (possible to do win strike)
// 0 if all fields are empty
// -1 if there are different symbols in row
{
    //int[][] points = GetRowPoints(pointCoordinates, direction, rowLength);
    char firstMark = Field[points[0][0]][points[0][1]];
    int equalCount = 0;
    for (int point=0;point<points.Length;point++)
    {
        if (firstMark == ' ' && Field[points[point][0]][points[point][1]] != ' ')
        {
            firstMark = Field[points[point][0]][points[point][1]];
        }
        if (Field[points[point][0]][points[point][1]] == ' ')
        {
            equalCount += 1;
        }
        else if (Field[points[point][0]][points[point][1]] == firstMark)
        {
            equalCount += 2;
        }
        else
        {
            equalCount = 0;
            break;
        }
    }
    if (equalCount == points.Length * 2)
    {
        return 2;
    }
    if (firstMark == ' ')
    {
        return 0;
    }
    if (equalCount > 0)
    {
        return 1;
    }
    else
    {
        return -1;
    }
}

// TIP, this function check only squared fields 
// and assume that win streak equal field size
int CheckGameStatus(char[][] GameField, string PlayerMarks, int Marks2Win)
// possible return values
// -2 game can continue
// -1 draw (no way to win for any player)
// 0/1 number of player who win, PlayerMarks char index in this case
{
    int result = -2;
    int possibleRows = 0;
    int emptyFields = 0;
    int tmpLineStatus;
    int[][] rowPoints;
    //for each PlayerMark
    for (int markNum=0; markNum<PlayerMarks.Length; markNum++)
    {
        for (int row=0; row<GameField.Length; row++)
        {
            // check first axis row
            if (GameField[row][0] == PlayerMarks[markNum])
            {
                // Always check horizontal lines
                rowPoints = GetRowPoints(new int[] {row, 0}, 1, Marks2Win);
                tmpLineStatus = CheckMarksInRow(GameField, rowPoints);
                if (tmpLineStatus == 2)
                {
                    return markNum;
                }
                else if (tmpLineStatus == 0 || tmpLineStatus == 1)
                {
                    possibleRows += 1;
                }
                // If it make sense check oblique lines
                if (row == 0)
                {
                    rowPoints = GetRowPoints(new int[] {row, 0}, 2, Marks2Win);
                    tmpLineStatus = CheckMarksInRow(GameField, rowPoints);
                    if (tmpLineStatus == 2)
                    {
                        return markNum;
                    }
                    else if (tmpLineStatus == 0 || tmpLineStatus == 1)
                    {
                        possibleRows += 1;
                    }
                }
                if (row == GameField.Length-1)
                {
                    rowPoints = GetRowPoints(new int[] {row, 0}, 3, Marks2Win);
                    tmpLineStatus = CheckMarksInRow(GameField, rowPoints);
                    if (tmpLineStatus == 2)
                    {
                        return markNum;
                    }
                    else if (tmpLineStatus == 0 || tmpLineStatus == 1)
                    {
                        possibleRows += 1;
                    }
                }
            }
            else if (GameField[row][0] == ' ')
            {
                emptyFields += 1;
            }
            // check second axis row
            if (GameField[0][row] == PlayerMarks[markNum])
            {
                // Always check vertical lines
                rowPoints = GetRowPoints(new int[] {0, row}, 0, Marks2Win);
                tmpLineStatus = CheckMarksInRow(GameField, rowPoints);
                if (tmpLineStatus == 2)
                {
                    return markNum;
                }
                else if (tmpLineStatus == 0 || tmpLineStatus == 1)
                {
                    possibleRows += 1;
                }
                // do not need to check oblique lines in this part
            }
            else if (GameField[row][0] == ' ')
            {
                emptyFields += 1;
            }
        }

    }
    // check if trere is still a point to continue game (draw)
    if (emptyFields < (GameField.Length + GameField[0].Length) && possibleRows == 0)
    {
        result = -1;
    }
    else
    {
        result = -2;
    }
    return result;
}

// This limit possible Game field size to 26, or to fieldHeaderNames.Length
string fieldHeaderNames = "abcdefghijklmnopqrstuvwxyz";
string playerMarks = "xo";

int FieldSize = 3;
int Marks2Win = FieldSize;

int PlayModeDefault = 1;
string PlayModeHelp = @$"Game mod can be
1) robot vs human, play as '{playerMarks[0]}' (default)
2) robot vs human, play as '{playerMarks[1]}'
3) human vs human
4) robot vs robot
---
For input coordinates put Letter, then number. 'A1' as example
Please input game mode number (default:{PlayModeDefault}): ";
int PlayMode = GetPlayMode(PlayModeHelp, PlayModeDefault);

Console.WriteLine($"Current game mode {PlayMode} | Current field size {FieldSize}");
Console.WriteLine($"You need to create a row of {Marks2Win} elements to win");

char[][] GameState = CreateGameField(FieldSize);

PrintGameField(GameState, fieldHeaderNames);

string[] players;
switch (PlayMode)
{
    case 1:
        players = new String[2] {"player", "robot"};
        break;
    case 2:
        players = new String[2] {"robot", "player"};
        break;
    case 3:
        players = new String[2] {"player1", "player2"};
        break;
    // case 4:
    default:
        players = new String[2] {"robot1", "robot2"};
        break;
}


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
    if (players[currentPlayer] == "player1" || players[currentPlayer] == "player2" || players[currentPlayer] == "player")
    {
        moveCoordinates = AskPlayerForCoordinates(GameState, fieldHeaderNames, players[currentPlayer], playerMarks[currentPlayer]);
    }
    else
    {
        // currently not used. no robot players implement for now
        //moveCoordinates = new int[] {0, 0};
        moveCoordinates = robotMove(GameState, fieldHeaderNames, playerMarks, players, currentPlayer, Marks2Win);
        Console.WriteLine($"{players[currentPlayer]} set '{playerMarks[currentPlayer]}' to {fieldHeaderNames[moveCoordinates[1]]}{moveCoordinates[0]+1}");
        System.Threading.Thread.Sleep(1000);
    }
    AddMark(GameState, playerMarks[currentPlayer], moveCoordinates[0], moveCoordinates[1]);

    // for now game newer end
    gameResult = CheckGameStatus(GameState, playerMarks, Marks2Win);
    if (gameResult >= -1 && gameResult <= 1)
    {
        gameEnd = true;
    }
    PrintGameField(GameState, fieldHeaderNames);
    currentPlayer = (currentPlayer + 1)%2;
}

if (gameResult == -1)
{
    Console.WriteLine("Draw");
}
else
{
    Console.WriteLine($"{players[gameResult]} wins");
}
