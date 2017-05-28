-- EAST (attempts to move the mob to a portal in the room who has an entry with direction EAST)

function checkUsage(args)
    return args[1] == "";
end

function execute(entityId, args)
    if checkUsage(args) then
        room = Game.Instance.GetRoomWithEntity(entityId);
        portal = room.GetPortalWithDirection("east");
        if (portal == nil) then
            Game.Instance.DoAction(MudAction.__new("infotoplayer", entityId, 0, "There is no exit in that direction."));
        else
            Game.Instance.DoAction(MudAction.__new("attemptenterportal", entityId, portal.Id, 0, "east"));
        end
    else
        Game.Instance.DoAction(MudAction.__new("infotoplayer", entityId, 0, "No args needed for 'east' command."));
    end
end