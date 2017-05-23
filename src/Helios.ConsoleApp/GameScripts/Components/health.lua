local this = MudComponent;

-- Traits
local maxHP = MudTrait;
local currHP = MudTrait;

function init (entity, script, defaults)
	this = MudComponent.__new(entity, "health", script);

    maxHP = this.Owner.Traits.Get("maxHP");
    currHP = this.Owner.Traits.Get("currHP");

    if (maxHP == nil and defaults["maxHP"] != nil) then
        maxHP = this.Owner.Traits.Add("maxHP", defaults["maxHP"]);
    end
    if (currHP == nil and defaults["currHP"] != nil) then
        currHP = this.Owner.Traits.Add("currHP", defaults["currHP"]);
    end
	return this;
end

-- function onAttach()
--         print(arguments);
--         maxHp = this.Owner.Traits.Get("MaxHp");
--         currHp = this.Owner.Traits.Get("CurrHP");
        
--         if (maxHp == nil or currHp == nil) then
--             math.randomseed(os.time());
--             local min = arguments["minHp"];
--             local max = arguments["maxHp"];
--             local hp = math.random(min, max);
--             print ("min: " .. min);
--             print ("max: " .. max);
--             print ("hp: " .. hp);
--             maxHp = this.Owner.Traits.Add("MaxHP", hp);
--             currHp = this.Owner.Traits.Add("CurrHP", hp);
--         end
-- end
