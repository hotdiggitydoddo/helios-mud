using System.Collections.Generic;
using Helios.Domain.Contracts;
using Helios.Engine.Objects;
using Helios.Domain.Models;
using System;
using System.Linq;

namespace Helios.Engine.Factories
{
    public interface IEntityFactory
    {
        MudEntity CreateEntity(string name, Dictionary<string, string> traits = null);
        MudEntity CreatePlayerCharacter(int accountId, MudEntity model);
        MudEntity GetEntityById(int id);
        MudEntity SaveEntity(MudEntity mob);
        List<MudEntity> LoadCharactersFromAccount(int accountId);
    }

    public class EntityFactory : IEntityFactory
    {
        private readonly IRepository<Entity> _entities;
        private readonly IRepository<Trait> _traits;
        private readonly IRepository<Account> _accounts;

        public EntityFactory(IRepository<Entity> entities, IRepository<Trait> traits, IRepository<Account> accounts)
        {
            _entities = entities;
            _traits = traits;
            _accounts = accounts;
        }

        public MudEntity CreateEntity(string name = null, Dictionary<string, string> traits = null)
        {
            var e = new Entity { Name = name, Traits = new List<Trait>() };
            e.Id = _entities.Add(e);

            var dto = new MudEntity(e.Id, e.Name);

            if (traits == null)
                return dto;

            foreach (var t in traits)
            {
                e.Traits.Add(new Trait { EntityId = e.Id, Name = t.Key, Value = t.Value });
                dto.Traits.Add(t.Key, t.Value);
            }

            _entities.Update(e);

            return dto;
        }

        public MudEntity CreatePlayerCharacter(int accountId, MudEntity model)
        {
            var e = new Entity
            { 
                Name = model.Name, 
                Traits = new List<Trait>(model.Traits.GetAll().Select(t => new Trait { Name = t.Name, Value = t.Value})) 
            };
            e.Id = _entities.Add(e);

            var account = _accounts.GetById(accountId);
            if (account.Characters == null) 
                account.Characters = new List<Account_Entity> { new Account_Entity { AccountId = accountId, EntityId = e.Id } };
            else 
            {
                account.Characters.Add(new Account_Entity { AccountId = accountId, EntityId = e.Id });
            }
            _accounts.Update(account);

            var newChar = new MudEntity(e.Id, e.Name);
            foreach (var t in e.Traits)
                newChar.Traits.Add(t.Name, t.Value);

            return newChar;
        }

        public MudEntity GetEntityById(int id)
        {
            var e = _entities.GetById(id, x => x.Traits);

            var dto = new MudEntity(e.Id);

            foreach (var trait in e.Traits)
                dto.Traits.Add(trait.Name, trait.Value);

            return dto;
        }

        public MudEntity SaveEntity(MudEntity entity)
        {
            var existing = _entities.GetById(entity.Id, x => x.Traits);
            existing.Traits.Clear();

             foreach (var trait in entity.Traits.GetAll())
                 existing.Traits.Add(new Trait { EntityId = entity.Id, Name = trait.Name, Value = trait.Value });
            
            existing.Components = entity.Components.GetAll().Select(x => new Entity_Component { EntityId = entity.Id, ComponentName = x.Name }).ToList();
            _entities.Update(existing);

            return entity;
        }

        public List<MudEntity> LoadCharactersFromAccount(int accountId)
        {
            var account = _accounts.GetById(accountId, x => x.Characters);
            var existing = _entities.Find(x => account.Characters.Select(c => c.EntityId).Contains(x.Id), o => o.Traits, o => o.Components).ToList();
            var retVal = new List<MudEntity>();

            foreach (var entity in existing)
            {   
                var e = new MudEntity(entity.Id, entity.Name);

                foreach (var trait in entity.Traits)
                    e.Traits.Add(trait.Name, trait.Value);
                foreach(var cmp in entity.Components)
                    ComponentManager.Instance.AssignComponent(e, cmp.ComponentName);
                
                if (entity.Traits.Count != e.Traits.Count)
                    SaveEntity(e);
                    
                retVal.Add(e);
            }
            return retVal;
        }
    }
}