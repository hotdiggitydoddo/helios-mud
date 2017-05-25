--local output = Output;
--local formatter = Formatter;
--function execute()
--	local str = formatter.Write("Welcome to <#yellow>Arcana Aeterna<#> <#cyan>LUA<#> style!\n-=-=-=-=-=-=-=-=-=-=");
--	output.SendText(str);
--end

function getTitle()
	return "Welcome to <#yellow>Arcana Aeterna<#> <#cyan><em>LUA<#> style!</em>\n" .. "Hold onto your hats!  It's gonna be fun!";
end

function motd()
	motd = "-=-=-=-=-=-=-=-=-=-=-\n" .. "Message of the Day...\n" .. "-=-=-=-=-=-=-=-=-=-=-\n";
	return motd;
end
