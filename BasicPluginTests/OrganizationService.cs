using System;
using Microsoft.Xrm.Sdk;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xrm.Sdk.Query;

namespace BasicPlugin.Tests
{
    internal class OrganizationService : IOrganizationService
    {
        private List<Entity> _entities = new List<Entity>();
        private List<Entity> _conflictingEntities = new List<Entity>();

        public void AddToConflictingEntity(Entity entity)
        {
            _conflictingEntities.Add(entity);
        }

        public IReadOnlyCollection<Entity> GetConflictingEntities()
        {
            return _conflictingEntities;
        }

        public IReadOnlyCollection<Entity> GetCreatedEntities()
        {
            return _entities;
        }

        public void Associate(string entityName, Guid entityId, Relationship relationship, EntityReferenceCollection relatedEntities)
        {
            throw new NotImplementedException();
        }

        public Guid Create(Entity entity)
        {
            var id = Guid.NewGuid();
            entity.Id = id;
            _entities.Add(entity);
            return id;
        }

        public void Delete(string entityName, Guid id)
        {
            throw new NotImplementedException();
        }

        public void Disassociate(string entityName, Guid entityId, Relationship relationship, EntityReferenceCollection relatedEntities)
        {
            throw new NotImplementedException();
        }

        public OrganizationResponse Execute(OrganizationRequest request)
        {
            throw new NotImplementedException();
        }

        public Entity Retrieve(string entityName, Guid id, ColumnSet columnSet)
        {
            throw new NotImplementedException();
        }

        public EntityCollection RetrieveMultiple(QueryBase query)
        {
            return new EntityCollection(_conflictingEntities); ;
        }

        public void Update(Entity entity)
        {
            throw new NotImplementedException();
        }
    }
}
