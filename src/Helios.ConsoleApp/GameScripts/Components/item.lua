local this = MudComponent;

-- Traits
local isStackable = MudTrait;
local quantity = MudTrait;


function init (entity, script, defaults)
	this = MudComponent.__new(entity, "item", script);

    isStackable = this.Owner.Traits.Get("isStackable");
    quantity = this.Owner.Traits.Get("quantity");

    if (isStackable == nil) then
       isStackable = defaults["isStackable"];
       quantity = defaults["quantity"];
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
