--QUIT (quits the game)

function checkUsage(args)
    return tablelength(args) > 0;
end

function getUsage()
    return "Usage: <#white>say<#> [whatever you want to say]"
end

function execute(entityId, args)
    if checkUsage(args) then
        room = Game.Instance.GetRoomWithEntity(entityId);
        print("room is " .. room.Name)       
        print("room is " .. args[1])       
         Game.Instance.DoAction(MudAction.__new("attemptsay", entityId, room.Id, 0, args[1]));
    else
        Game.Instance.DoAction(MudAction.__new("infotoplayer", entityId, 0, getUsage()));
    end
end

function tablelength(T)
    local count = 0;
    for _ in pairs(T) do count = count + 1 end
    return count;
end