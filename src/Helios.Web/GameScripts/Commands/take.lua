--TAKE (attempts to take an item)

function checkUsage(args)
    return args[1] != "";
end

function getUsage()
    return "Usage: <#white>take<#> [name of item]"
end

function execute(entityId, args)
    if checkUsage(args) then
        room = Game.Instance.GetRoomWithEntity(entityId);
        Game.Instance.DoAction(MudAction.__new("attemptreceive", entityId, room.Id, 0, args[1]));
    else
        Game.Instance.DoAction(MudAction.__new("infotoplayer", entityId, 0, getUsage()));
    end
end

function tablelength(T)
    local count = 0;
    for _ in pairs(T) do count = count + 1 end
    print(count)
    return count;
end