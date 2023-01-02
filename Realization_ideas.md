# Realization ideas
Go through ideas before make them into code

## Content
1. Launch parameters
2. Game state

## 1. Launch parameters

At start we need to get from user:
1. (TODO)Field side size (default 3)
2. (TODO/rethink)Number of marks in a line to win
    1. By default must equal "Field side size"
    2. Or be less than it.
3. Type of play
    1. robot vs human (default)
    2. human vs human
    3. robot vs robot

I probably can   
"
(Short help about requested parameter)
Please input number or nothing for (default_parameter)"  
Then check if I can `int.TryParse` it unless set default variable.

## 2. Game state

Planned to store field as array of arrays.  
Field value will be stores as a char.

Trying to implement "Load field" functional inside CreateGameField function. For debug purposses.