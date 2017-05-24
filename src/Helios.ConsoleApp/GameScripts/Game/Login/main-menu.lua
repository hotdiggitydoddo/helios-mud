local characters = Characters;
local game = Game;
local accountId;

function enter(chars, aId)
    characters = chars;
    accountId = aId;
    printMenu();
end

function printMenu()
    option = "Please choose an option:\n-=-=-=-=-=-=-=-=-=-=-=-=\n";
    if (characters.Count > 0) then
        option = option .. "<tab>(<#white>P<#>)lay as one of your characters\n";
    end
    option = option .. "<tab>(<#white>C<#>)reate a new character\n";
    Game.Instance.SendMessage(accountId, option);
end

function printCharacters()
    header = "Select a character:\n-=-=-=-=-=-=-=-=-=-=-\n";
    chars = "";
    for i = 0, characters.Count-1 do
        chars = chars .. "<tab>(<#white>" .. i + 1 .. "<#>)<tab>" .. characters[i].Name .."\n";
    end
    Game.Instance.SendMessage(accountId, header .. chars);
end