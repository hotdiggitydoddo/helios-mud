local this = MudComponent;

-- Traits
local race = MudTrait;
local strength = MudTrait;
local agility = MudTrait;
local intellect = MudTrait;
local luck = MudTrait;

function init (entity, script, defaults)
	this = MudComponent.__new(entity, "mobStats", script);

    race = this.Owner.Traits.Get("race");
    strength = this.Owner.Traits.Get("strength");
    agility = this.Owner.Traits.Get("agility");
    intellect = this.Owner.Traits.Get("intellect");
    luck = this.Owner.Traits.Get("luck");

    if (race == nil and defaults["race"] != nil) then
        race = this.Owner.Traits.Add("race", defaults["race"]);
    end

    --could also get the values by looking up default min/max for race from elsewhere and mob level?
    if (strength == nil and defaults["strength"] != nil) then
        strength = this.Owner.Traits.Add("strength", defaults["strength"]);
    end
    if (agility == nil and defaults["agility"] != nil) then
        agility = this.Owner.Traits.Add("agility", defaults["agility"]);
    end
    if (intellect == nil and defaults["intellect"] != nil) then
        intellect = this.Owner.Traits.Add("intellect", defaults["intellect"]);
    end
    if (luck == nil and defaults["luck"] != nil) then
        luck = this.Owner.Traits.Add("luck", defaults["luck"]);
    end
	return this;
end
