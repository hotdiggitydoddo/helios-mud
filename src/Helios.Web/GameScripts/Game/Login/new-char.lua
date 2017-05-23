function enter()
    return "<#darkcyan>Character Creation<#>\n-=-=-=-=-=-=-=-=-=-=-=-=-=-=-\n" .. "Press \"<#white>Q<#>\" to return to the main menu."
end

function printRaces()
    local a = "Choose a race:\n";
    local b = "-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-\n";
    local c = "(<#white>1<#>) - Human\n";
    local d = "(<#white>2<#>) - Elf\n";
    local e = "(<#white>3<#>) - Dwarf\n\n";
    return a .. b .. c .. d .. e;
end;
