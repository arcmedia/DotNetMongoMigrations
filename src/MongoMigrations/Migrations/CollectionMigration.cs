using System;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Driver;

namespace MongoMigrations.Migrations
{
    /// <summary>
    /// A migration to be applied to each document inside a collection
    /// </summary>
    /// <seealso cref="Migration" />
    public abstract class CollectionMigration : Migration
    {
        /// <summary>
        /// The collection name
        /// </summary>
        private readonly string _collectionName;

        /// <summary>
        /// Initializes a new instance of the <see cref="CollectionMigration" /> class.
        /// </summary>
        /// <param name="version">The version.</param>
        /// <param name="description">The description.</param>
        /// <param name="collectionName">Name of the collection.</param>
        protected CollectionMigration(MigrationVersion version, string description, string collectionName)
            : base(version, description)
        {
            _collectionName = collectionName;
        }

        /// <summary>
        /// Filters this instance.
        /// </summary>
        /// <returns>Filter to apply to get the documents that should be updated</returns>
        public virtual FilterDefinition<BsonDocument> Filter()
        {
            return Builders<BsonDocument>
                .Filter
                .Empty;
        }

        /// <inheritdoc />
        public override void Update(IMongoDatabase database)
        {
            var collection = GetCollection(database);
            var documents = GetDocuments(collection);
            UpdateDocuments(collection, documents);
        }

        /// <summary>
        /// Updates the documents.
        /// </summary>
        /// <param name="collection">The collection.</param>
        /// <param name="documents">The documents.</param>
        public virtual void UpdateDocuments(
            IMongoCollection<BsonDocument> collection,
            IEnumerable<BsonDocument> documents)
        {
            foreach (var document in documents)
            {
                try
                {
                    UpdateDocument(collection, document);
                }
                catch (Exception exception)
                {
                    OnErrorUpdatingDocument(document, exception);
                }
            }
        }

        /// <summary>
        /// Updates the document.
        /// </summary>
        /// <param name="collection">The collection.</param>
        /// <param name="document">The document.</param>
        public abstract void UpdateDocument(IMongoCollection<BsonDocument> collection, BsonDocument document);

        /// <summary>
        /// Gets the collection.
        /// </summary>
        /// <param name="database">The database.</param>
        /// <returns>Collection of documents in the database</returns>
        protected virtual IMongoCollection<BsonDocument> GetCollection(IMongoDatabase database)
        {
            return database.GetCollection<BsonDocument>(_collectionName);
        }

        /// <summary>
        /// Called when there is an error updating a document.
        /// </summary>
        /// <param name="document">The document.</param>
        /// <param name="exception">The exception.</param>
        protected virtual void OnErrorUpdatingDocument(BsonDocument document, Exception exception)
        {
            var message =
                new
                {
                    Message = "Failed to update document",
                    CollectionName = _collectionName,
                    Id = document.TryGetDocumentId(),
                    MigrationVersion = Version,
                    MigrationDescription = Description
                };
            throw new MigrationException(message.ToString(), exception);
        }

        /// <summary>
        /// Gets the documents to be migrated.
        /// </summary>
        /// <param name="collection">The collection.</param>
        /// <returns>documents to be migrated</returns>
        protected virtual IEnumerable<BsonDocument> GetDocuments(IMongoCollection<BsonDocument> collection)
        {
            var query = Filter();
            return collection.Find(query).ToCursor().ToEnumerable();
        }
    }
}