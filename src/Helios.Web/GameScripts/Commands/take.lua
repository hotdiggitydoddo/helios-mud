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
        entity = Game.Instance.FindEntityInRoom(args[1], room.Id);
        if entity == 0 then
            Game.Instance.DoAction(MudAction.__new("infotoplayer", entityId, 0, "That is not here."));
        else
            Game.Instance.DoAction(MudAction.__new("attemptreceive", room.Id, entityId, entity, ""));
        end
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