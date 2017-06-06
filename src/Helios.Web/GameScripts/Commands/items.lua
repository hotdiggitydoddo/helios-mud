--ITEMS (displays an entity's inventory)

function checkUsage(args)
    return args[1] == "";
end

function getUsage()
    return "Usage: <#white>items<#>"
end

function execute(entityId, args)
    if checkUsage(args) then
        Game.Instance.DoAction(MudAction.__new("listinventory", entityId, 0));
    else
        Game.Instance.DoAction(MudAction.__new("infotoplayer", entityId, 0, "No args needed for 'items' command."));
    end
end