local this = MudComponent;

-- Traits
local inventoryItems = MudTrait;

function init (entity, script, defaults)
	this = MudComponent.__new(entity, "inventory", script);

    -- description = this.Owner.Traits.Get("description");
    -- weight = this.Owner.Traits.Get("weight");

    -- if (description == nil and defaults["description"] != nil) then
    --     description = this.Owner.Traits.Add("description", defaults["description"]);
    -- end
    -- if (weight == nil and defaults["weight"] != nil) then
    --     weight = this.Owner.Traits.Add("weight", defaults["weight"]);
    -- end
	return this;
end