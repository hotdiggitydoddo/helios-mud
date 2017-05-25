--QUIT (quits the game)

function checkUsage(args)
    return tablelength(args) == 0;
end

function execute(entityId, args)
    if checkUsage(args) then
        Game.Instance.DoAction(MudAction.__new("look", entityId));
    else
        Game.Instance.DoAction(MudAction.__new("infotoplayer", entityId, 0, "No args needed for 'quit' command."));
    end
end

function tablelength(T)
    local count = 0;
    for _ in pairs(T) do count = count + 1 end
    return count;
end